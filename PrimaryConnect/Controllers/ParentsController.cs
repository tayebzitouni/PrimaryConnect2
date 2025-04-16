
using Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using OfficeOpenXml;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;
using PrimaryConnect.Models;
using System;
using static System.Net.Mime.MediaTypeNames;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParentsController : ControllerBase
    {
        public ParentsController(AppDbContext appDbContext)
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;

       
        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> FrogetPassword(string Email)
        {
            if (await _PrimaryConnect_Db.Parents.AnyAsync(t => t.Email == Email))
            {

                return Ok( UsefulFunctions.GenerateandSendKey(Email));
            }
            return NotFound();

        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> ChangePassword(string Email, string Password)
        {
            Parent? parent = await _PrimaryConnect_Db.Parents.SingleOrDefaultAsync(t => t.Email == Email);
            if (parent == null) { return NotFound(); }
            parent.Password = Password;
            _PrimaryConnect_Db.Parents.Update(parent);
            await _PrimaryConnect_Db.SaveChangesAsync();
            return Ok();

        }

        
        [HttpPost("AddParent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddParent(Parent_Dto parent)
        {
            if (parent == null)
            {
                return BadRequest("parent data is required.");
            }
            Parent _parent=parent.ToParent();
            _PrimaryConnect_Db.Parents.Add(_parent);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllParents), new { id = parent.Id }, parent);
        }

        [Authorize]
        // DELETE: api/admins/{id}
        [HttpDelete("{id}", Name = "DeleteParent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteParent(int id)
        {
            var Parent = await _PrimaryConnect_Db.Parents.FindAsync(id);
            if (Parent == null)
            {
                return NotFound("Admin not found.");
            }

            _PrimaryConnect_Db.Parents.Remove(Parent);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}", Name = "UpdateParent")]
        public async Task<IActionResult> UpdateParent(int id, Parent_Dto updatedAdmin)
        {
            if (id != updatedAdmin.Id)
            {
                return BadRequest("ID mismatch.");
            }

            var existingAdmin = await _PrimaryConnect_Db.Parents.FindAsync(id);
            if (existingAdmin == null)
            {
                return NotFound("Admin not found.");
            }

            // Mettre à jour les champs nécessaires
            existingAdmin.Name = updatedAdmin.Name;
            existingAdmin.Email = updatedAdmin.Email;
            existingAdmin.PhoneNumber = updatedAdmin.PhoneNumber;
            existingAdmin.Password = updatedAdmin.Password;


            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}", Name = "GetParentById")]
        public async Task<ActionResult<Administrator>> GetParentById(int id)
        {
            var admin = await _PrimaryConnect_Db.Parents.FindAsync(id);

            if (admin == null)
            {
                return NotFound("parent not found.");
            }

            return Ok(admin);
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Parent>>> GetAllParents()
        {
            return await _PrimaryConnect_Db.Parents.ToListAsync();
        }

        //[HttpPost("upload")]
        //public async Task<ActionResult> ImportStudentsFromExcel(IFormFile file)
        //{
        //    // List of supported Excel extensions
        //    var supportedExtensions = new[] { ".xlsx", ".xls", ".xlsm", ".xlsb" };
        //    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        //    if (string.IsNullOrEmpty(fileExtension) || !supportedExtensions.Contains(fileExtension))
        //    {
        //        return BadRequest("Supported Excel formats: .xlsx, .xls, .xlsm, .xlsb");
        //    }

        //    ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

        //    using var stream = new MemoryStream();
        //    await file.CopyToAsync(stream);

        //    // For .xls files (older format), we need to convert to .xlsx first
        //    if (fileExtension == ".xls")
        //    {
        //        // Convert .xls to .xlsx format in memory
        //        using (var excelEngine = new ExcelEngine())
        //        {
        //            IApplication application = excelEngine.Excel;
        //            application.DefaultVersion = ExcelVersion.Excel2016;

        //            // Open the .xls file
        //            IWorkbook workbook = application.Workbooks.Open(stream);

        //            // Convert to .xlsx in memory
        //            var xlsxStream = new MemoryStream();
        //            workbook.SaveAs(xlsxStream, ExcelSaveType.SaveAsXlsx);
        //            xlsxStream.Position = 0;

        //            // Use the converted stream
        //            stream.Dispose(); // Dispose the original stream
        //            stream = xlsxStream; // Use the converted stream
        //        }
        //    }

        //    using var package = new ExcelPackage(stream);
        //    var worksheet = package.Workbook.Worksheets[0];
        //    var rowCount = worksheet.Dimension.Rows;

        //    var students = new List<Parent>();

        //    for (int row = 2; row <= rowCount; row++)
        //    {
        //        var student = new Parent
        //        {
        //            Name = worksheet.Cells[row, 1].Text,
        //            Email = worksheet.Cells[row, 2].Text,
        //            Password = worksheet.Cells[row, 3].Text,
        //            PhoneNumber = worksheet.Cells[row, 4].Text
        //        };
        //        students.Add(student);
        //    }

        //    _PrimaryConnect_Db.Parents.AddRange(students);
        //    await _PrimaryConnect_Db.SaveChangesAsync();
        //    return Ok("Added Successfully");
        //}


    }
}
