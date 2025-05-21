
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Math;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;
using PrimaryConnect.Models;
using System.Reflection.Emit;



namespace PrimaryConnect.Controllers
{
    [Route("api/Student")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        public StudentsController(AppDbContext appDbContext)
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;


        [HttpGet("GetAllStudents")]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            return await _PrimaryConnect_Db.Students.ToListAsync();
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}", Name = "GetStudentById")]
        public async Task<ActionResult<Administrator>> GetStudentById(int id)
        {
            var admin = await _PrimaryConnect_Db.Students.FindAsync(id);

            if (admin == null)
            {
                return NotFound("Student not found.");
            }

            return Ok(admin);
        }



        [HttpPost("AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Administrator>> AddStudent(Student_Dto student)
        {
            if (student == null)
            {
                return BadRequest("Student data is required.");
            }
            Student _student = student.ToStudent();
            _PrimaryConnect_Db.Students.Add(_student);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return Ok(student);
        }


        // DELETE: api/admins/{id}
        [HttpDelete("{id}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _PrimaryConnect_Db.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            _PrimaryConnect_Db.Students.Remove(student);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}", Name = "UpdateStudent")]
        public async Task<IActionResult> UpdateStudent(int id, Student_Dto updatedStudent)
        {
          

            Student? existingStudent = await _PrimaryConnect_Db.Students.FindAsync(id);
            if (existingStudent == null)
            {
                return NotFound("Student not found.");
            }
            existingStudent = updatedStudent.ToStudent();


            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }
       


        [HttpGet("download-student-template")]
        public IActionResult DownloadStudentTemplate()
        {
            // Example: Fetch from DB
            var parents = _PrimaryConnect_Db.Parents.Select(p => p.Name).ToList();
            var schools = _PrimaryConnect_Db.schools.Select(s => s.Name).ToList();
            var classes = _PrimaryConnect_Db.Classes.Select(c => c.name).ToList();

            var stream = UsefulFunctions.CreateExcelWithDropdowns(parents, schools, classes);

            return File(
                fileStream: stream,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "StudentTemplate.xlsx"
            );
        }

        [HttpPost("upload-students")]
        public async Task<IActionResult> UploadStudents(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var workbook = new XLWorkbook(stream);
            var ws = workbook.Worksheet("Students");

            if (ws == null)
                return BadRequest("Worksheet 'Students' not found.");

            // Cache reference data (Name -> ID)
            var parentDict = _PrimaryConnect_Db.Parents.ToDictionary(p => p.Name.Trim(), p => p.Id);
            var schoolDict = _PrimaryConnect_Db.schools.ToDictionary(s => s.Name.Trim(), s => s.Id);
            var classDict = _PrimaryConnect_Db.Classes.ToDictionary(c => c.name.Trim(), c => c.id);

            var students = new List<Student>();

            int row = 2; // Row 1 = headers

            while (!string.IsNullOrWhiteSpace(ws.Cell(row, 1).GetString()))
            {
                var name = ws.Cell(row, 1).GetString().Trim();
                var ageStr = ws.Cell(row, 2).GetString().Trim();
                var parentName = ws.Cell(row, 3).GetString().Trim();
                var schoolName = ws.Cell(row, 4).GetString().Trim();
                var className = ws.Cell(row, 5).GetString().Trim();

                if (!int.TryParse(ageStr, out int age))
                {
                    row++;
                    continue; // skip invalid age
                }

                // Match names to IDs
                if (!parentDict.TryGetValue(parentName, out var parentId) ||
                    !schoolDict.TryGetValue(schoolName, out var schoolId) ||
                    !classDict.TryGetValue(className, out var classId))
                {
                    row++;
                    continue; // skip if any lookup fails
                }

                students.Add(new Student
                {
                    Name = name,
                    Degree = age,
                    ParentId = parentId,
                    SchoolId = schoolId,
                    ClassId = classId
                });

                row++;
            }

            if (!students.Any())
                return BadRequest("No valid students found in the file.");

            // Save to DB
            _PrimaryConnect_Db.Students.AddRange(students);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return Ok(new { Added = students.Count });
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("ByParent/{Parentid}", Name = "GetStudentsByParenId")]
        public async Task<ActionResult<Student_Dto>> GetStudentsByParentId(int Parentid)
        {
            var admin = await _PrimaryConnect_Db.Students.Where(p => p.ParentId == Parentid).ToListAsync();

            if (admin == null)
            {
                return NotFound("There is no Students To this Teacher");
            }

            return Ok(admin);
        }




    }

}

   



