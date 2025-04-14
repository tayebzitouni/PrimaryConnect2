
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;
using PrimaryConnect.Models;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<Administrator>> AddStudent( Student_Dto student)
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
            if (id != updatedStudent.Id)
            {
                return BadRequest("ID mismatch.");
            }

          Student? existingStudent= await _PrimaryConnect_Db.Students.FindAsync(id);
            if (existingStudent == null)
            {
                return NotFound("Student not found.");
            }
            existingStudent=updatedStudent.ToStudent();


            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        [HttpPost("upload-students")]
        public async Task<IActionResult> UploadStudents(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var students = new List<Student>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Start from row 2 to skip header
                    {
                        var name = worksheet.Cells[row, 1].Text; 
                        var degree = int.Parse(worksheet.Cells[row, 2].Text);
                        var parentName = worksheet.Cells[row, 3].Text;
                        var Email = worksheet.Cells[row, 4].Text;
                        var Password = worksheet.Cells[row, 5].Text;
                        var phoneNumber = worksheet.Cells[row, 6].Text;
                        var schoolName = worksheet.Cells[row, 4].Text;
                        var className = worksheet.Cells[row, 5].Text;

                        // Handle Parent creation or retrieval
                        var parent = await _PrimaryConnect_Db.Parents.FirstOrDefaultAsync(p => p.Name == parentName);
                        if (parent == null)
                        {
                            // Create a new parent if not found
                            parent = new Parent { Name = parentName };
                            _PrimaryConnect_Db.Parents.Add(parent);
                            await _PrimaryConnect_Db.SaveChangesAsync();
                        }

                        // Handle School creation or retrieval
                        var school = await _PrimaryConnect_Db.schools.FirstOrDefaultAsync(s => s.Name == schoolName);
                        if (school == null)
                        {
                            // Create a new school if not found
                            school = new School { Name = schoolName };
                            _PrimaryConnect_Db.schools.Add(school);
                            await _PrimaryConnect_Db.SaveChangesAsync();
                        }

                        


                        // Handle Class creation or retrieval
                        var myClass = await _PrimaryConnect_Db.Classes.FirstOrDefaultAsync(c => c.name == className);
                        if (myClass == null)
                        {
                            // Create a new class if not found
                            myClass = new Class { name = className };
                            _PrimaryConnect_Db.Classes.Add(myClass);
                            await _PrimaryConnect_Db.SaveChangesAsync();
                        }

                        // Create the Student and link Parent, School, and Class
                        var student = new Student
                        {
                            Name = name,
                            Degree = degree,
                            ParentId = parent.Id,
                            SchoolId = school.Id,
                            ClassId = myClass.id
                        };

                        students.Add(student);
                    }
                }
            }

            // Add all students to the context and save changes
                _PrimaryConnect_Db.Students.AddRange(students);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return Ok($"{students.Count} students uploaded successfully.");
        }





    }

}
