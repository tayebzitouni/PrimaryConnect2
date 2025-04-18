
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
            if (id != updatedStudent.Id)
            {
                return BadRequest("ID mismatch.");
            }

            Student? existingStudent = await _PrimaryConnect_Db.Students.FindAsync(id);
            if (existingStudent == null)
            {
                return NotFound("Student not found.");
            }
            existingStudent = updatedStudent.ToStudent();


            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }
        //ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        //[HttpGet("download-student-template")]
        //public IActionResult DownloadStudentTemplate()
        //{
        //    ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        //    using (var package = new ExcelPackage())
        //    {
        //        var sheet = package.Workbook.Worksheets.Add("Students");

        //        // Headers
        //        sheet.Cells[1, 1].Value = "Name";
        //        sheet.Cells[1, 2].Value = "Degree";
        //        sheet.Cells[1, 3].Value = "ParentName";
        //        sheet.Cells[1, 4].Value = "SchoolName";
        //        sheet.Cells[1, 5].Value = "ClassName";

        //        var options = package.Workbook.Worksheets.Add("Options");

        //        // === Fill Options Sheet ===
        //        var schools = _PrimaryConnect_Db.schools.Select(s => s.Name).ToList();
        //        var parents = _PrimaryConnect_Db.Parents.Select(p => p.Name).ToList();
        //        var classes = _PrimaryConnect_Db.Classes.Select(c => c.name).ToList();

        //        // School names in column A
        //        for (int i = 0; i < schools.Count; i++)
        //            options.Cells[i + 1, 1].Value = schools[i];
        //        options.Names.Add("SchoolList", options.Cells[$"A1:A{schools.Count}"]);

        //        // Parent names in column B
        //        for (int i = 0; i < parents.Count; i++)
        //            options.Cells[i + 1, 2].Value = parents[i];
        //        options.Names.Add("ParentList", options.Cells[$"B1:B{parents.Count}"]);

        //        // Class names in column C
        //        for (int i = 0; i < classes.Count; i++)
        //            options.Cells[i + 1, 3].Value = classes[i];
        //        options.Names.Add("ClassList", options.Cells[$"C1:C{classes.Count}"]);

        //        // === Add Dropdowns to Students Sheet ===
        //        var parentValidation = sheet.DataValidations.AddListValidation("C2:C100");
        //        parentValidation.Formula.ExcelFormula = "=ParentList";

        //        var schoolValidation = sheet.DataValidations.AddListValidation("D2:D100");
        //        schoolValidation.Formula.ExcelFormula = "=SchoolList";

        //        var classValidation = sheet.DataValidations.AddListValidation("E2:E100");
        //        classValidation.Formula.ExcelFormula = "=ClassList";

        //        // Hide Options Sheet
        //        options.Hidden = eWorkSheetHidden.VeryHidden;

        //        // Return file
        //        var stream = new MemoryStream();
        //        package.SaveAs(stream);
        //        stream.Position = 0;

        //        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentUploadTemplate.xlsx");
        //    }


        [HttpGet("download-student-template")]
        public IActionResult DownloadStudentTemplate()
        {
            var stream = UsefulFunctions.CreateExcelWithDropdowns();

            return File(
                fileStream: stream,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "StudentTemplate.xlsx"
            );
        }

    }

}

   



