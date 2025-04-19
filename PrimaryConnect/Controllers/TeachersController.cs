using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OfficeOpenXml;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;
using PrimaryConnect.Models;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : Controller
    {
        public TeachersController(AppDbContext appDbContext)
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;




        [HttpGet("GetAllTeachers")]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetAllTeachers()
        {
            return await _PrimaryConnect_Db.Teachers.ToListAsync();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> FrogetPassword(string Email)
        {
            if (await _PrimaryConnect_Db.Teachers.AnyAsync(t => t.Email == Email))
            {

                return Ok(UsefulFunctions.GenerateandSendKey(Email));
            }
            return NotFound();

        }
        [HttpPost("[action]")]
        public async Task<IActionResult> ChangePassword(string Email, string Password)
        {
            Teacher? teacher = await _PrimaryConnect_Db.Teachers.SingleOrDefaultAsync(t => t.Email == Email);
            if (teacher == null) { return NotFound(); }
            teacher.Password = Password;
            _PrimaryConnect_Db.Teachers.Update(teacher);
            await _PrimaryConnect_Db.SaveChangesAsync();
            return Ok();

        }




        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}", Name = "GetTeacherById")]
        public async Task<ActionResult<Administrator>> GetTeacherById(int id)
        {
            var admin = await _PrimaryConnect_Db.Teachers.FindAsync(id);

            if (admin == null)
            {
                return NotFound("Teacher not found.");
            }

            return Ok(admin);
        }


        [HttpPost("AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Teacher>> AddTeacher(Teacher_Dto _teacher)
        {
            if (_teacher == null)
            {
                return BadRequest("Student data is required.");
            }
            Teacher teacher = _teacher.ToTeacher();


            _PrimaryConnect_Db.Teachers.Add(teacher);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllTeachers), new { id = teacher.Id }, teacher);
        }


        // DELETE: api/admins/{id}
        [HttpDelete("{id}", Name = "DeleteTeacher")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var admin = await _PrimaryConnect_Db.Teachers.FindAsync(id);
            if (admin == null)
            {
                return NotFound("Teacher not found.");
            }

            _PrimaryConnect_Db.Teachers.Remove(admin);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("edit-teacher/{id}")]
        public async Task<IActionResult> EditTeacher(int id, Teacher_Dto teacher)
        {
            var existingTeacher = await _PrimaryConnect_Db.Teachers.FindAsync(id);
            if (existingTeacher == null)
            {
                return NotFound("Teacher not found.");
            }

            // Only update fields if they are not null or empty (optional logic)
            if (!string.IsNullOrWhiteSpace(teacher.Name))
                existingTeacher.Name = teacher.Name;

            if (!string.IsNullOrWhiteSpace(teacher.Email))
                existingTeacher.Email = teacher.Email;

            if (!string.IsNullOrWhiteSpace(teacher.Subject))
                existingTeacher.Subject = teacher.Subject;

            if (!string.IsNullOrWhiteSpace(teacher.PhoneNumber))
                existingTeacher.PhoneNumber = teacher.PhoneNumber;

            // Add checks for any other fields you want to support updating

            await _PrimaryConnect_Db.SaveChangesAsync();
            return NoContent();
        }


        [HttpGet("download-template")]
        public IActionResult DownloadExcelTemplate()
        {

            ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

            using (var package = new ExcelPackage())
            {
                // Main sheet for teacher input
                var sheet = package.Workbook.Worksheets.Add("Teachers");
                sheet.Cells[1, 1].Value = "TeacherName";
                sheet.Cells[1, 2].Value = "Email";
                sheet.Cells[1, 3].Value = "PhoneNumber";
                sheet.Cells[1, 4].Value = "Password";
                sheet.Cells[1, 5].Value = "Subject";     // will have dropdown
                sheet.Cells[1, 6].Value = "SchoolId";    // will have dropdown

                // Sheet for options
                var schools = _PrimaryConnect_Db.schools.Select(s => new { s.Id, s.Name }).ToList();
                var optionsSheet = package.Workbook.Worksheets.Add("Options");

                // Fill School options (column A & B)
                for (int i = 0; i < schools.Count; i++)
                {
                    optionsSheet.Cells[i + 1, 1].Value = schools[i].Id;
                    optionsSheet.Cells[i + 1, 2].Value = schools[i].Name;
                }

                // Fill Subject options (column D)
                string[] subjects = new[] { "عربية", "فرنسية", "إنجليزية" };
                for (int i = 0; i < subjects.Length; i++)
                {
                    optionsSheet.Cells[i + 1, 4].Value = subjects[i];  // D column
                }

                // Data validation for SchoolId
                var schoolValidation = sheet.DataValidations.AddListValidation("F2:F100");
                schoolValidation.Formula.ExcelFormula = $"Options!$A$1:$A${schools.Count}";
                schoolValidation.ShowErrorMessage = true;
                schoolValidation.ErrorTitle = "Invalid School";
                schoolValidation.Error = "Select a valid School ID from the dropdown.";

                // Data validation for Subject
                var subjectValidation = sheet.DataValidations.AddListValidation("E2:E100");
                subjectValidation.Formula.ExcelFormula = $"Options!$D$1:$D${subjects.Length}";
                subjectValidation.ShowErrorMessage = true;
                subjectValidation.ErrorTitle = "Invalid Subject";
                subjectValidation.Error = "Choose only from the provided subjects.";

                // Export file
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "TeacherUploadTemplate_WithSubjects.xlsx");
            }

        }
        [HttpPost("upload-filled-template")]
        public async Task<IActionResult> UploadFilledTemplate(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file was uploaded.");

            ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

            var teachers = new List<Teacher>();
            var errors = new List<string>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets["Teachers"];
                    if (worksheet == null)
                        return BadRequest("Worksheet 'Teachers' not found.");

                    int rowCount = worksheet.Dimension.End.Row;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            string name = worksheet.Cells[row, 1].Text?.Trim();
                            string email = worksheet.Cells[row, 2].Text?.Trim();
                            string phone = worksheet.Cells[row, 3].Text?.Trim();
                            string password = worksheet.Cells[row, 4].Text?.Trim();
                            string subject = worksheet.Cells[row, 5].Text?.Trim();
                            string schoolName = worksheet.Cells[row, 6].Text?.Trim();

                            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(subject) ||
                                string.IsNullOrWhiteSpace(schoolName))
                            {
                                errors.Add($"Row {row}: Missing required fields.");
                                continue;
                            }

                            // Find the school by name
                            var school = _PrimaryConnect_Db.schools.FirstOrDefault(s => s.Name == schoolName);
                            if (school == null)
                            {
                                errors.Add($"Row {row}: School '{schoolName}' not found.");
                                continue;
                            }

                            var teacherDto = new Teacher_Dto
                            {
                                Name = name,
                                Email = email,
                                PhoneNumber = phone,
                                Password = password,
                                Subject = subject,
                                SchoolId = school.Id
                            };

                            teachers.Add(teacherDto.ToTeacher());
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Row {row}: {ex.Message}");
                        }
                    }
                }
            }

            if (teachers.Any())
            {
                await _PrimaryConnect_Db.Teachers.AddRangeAsync(teachers);
                await _PrimaryConnect_Db.SaveChangesAsync();
            }

            if (errors.Any())
                return Ok(new { Message = "Some rows were not imported.", Errors = errors });

            return Ok(new { Message = "All teachers imported successfully." });
        }


    }

}


