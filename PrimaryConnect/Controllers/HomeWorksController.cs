//using Google;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using PrimaryConnect.Data;
//using PrimaryConnect.Models;

//namespace PrimaryConnect.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class HomeworksController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly IWebHostEnvironment _env; // Ensure this is declared

//        // Constructor injecting both AppDbContext and IWebHostEnvironment
//        public HomeworksController(AppDbContext context, IWebHostEnvironment env)
//        {
//            _context = context;
//            _env = env;  // Inject IWebHostEnvironment
//        }

//        //    [HttpPost("upload")]
//        //    public async Task<IActionResult> UploadHomework(IFormFile file, [FromServices] IFileUploadPathProvider uploadPathProvider)
//        //    {
//        //        // Check if a file is provided
//        //        if (file == null || file.Length == 0)
//        //        {
//        //            return BadRequest("A file must be uploaded.");
//        //        }

//        //        try
//        //        {
//        //            // Get the upload folder path from the provider
//        //            var uploadDir = uploadPathProvider.GetUploadPath();

//        //            // Create the directory if it doesn't exist
//        //            if (!Directory.Exists(uploadDir))
//        //            {
//        //                Directory.CreateDirectory(uploadDir);
//        //            }

//        //            // Define the file path
//        //            var filePath = Path.Combine(uploadDir, file.FileName);

//        //            // Save the file to the specified path
//        //            using (var stream = new FileStream(filePath, FileMode.Create))
//        //            {
//        //                await file.CopyToAsync(stream);
//        //            }

//        //            // Return the file path as response
//        //            return Ok(new { filePath });
//        //        }
//        //        catch (Exception ex)
//        //        {
//        //            return StatusCode(500, "Internal server error: " + ex.Message);
//        //        }
//        //    }

//        //}
//        [HttpPost("assign-homework")]
//        public async Task<IActionResult> AssignHomework([FromBody] Homework homework)
//        {
//            if (homework == null)
//            {
//                return BadRequest("Homework data is required.");
//            }

//            try
//            {
//                // Check if homework is assigned to a specific user or to all parents in a class
//                if (homework.AssignedToAll && homework.ClassId != null)
//                {
//                    // Assign homework to all parents of a class
//                    var usersInClass = await _context.Parents
//                        .Where(u => u.ClassId == homework.ClassId)
//                        .ToListAsync();

//                    foreach (var user in usersInClass)
//                    {
//                        var assignedHomework = new Homework
//                        {
//                            FilePath = homework.FilePath,
//                            Title = homework.Title,
//                            Description = homework.Description,
//                            UserId = user.Id,
//                            ClassId = homework.ClassId,
//                            AssignedToAll = true,
//                            DateAssigned = DateTime.Now
//                        };

//                        _context.homeworks.Add(assignedHomework);
//                    }
//                }
//                else if (homework.UserId != null)
//                {
//                    // Assign homework to a specific user
//                    _context.homeworks.Add(homework);
//                }
//                else
//                {
//                    return BadRequest("UserId or ClassId must be specified.");
//                }

//                // Save to database
//                await _context.SaveChangesAsync();

//                // Return response
//                return Ok(new { message = "Homework assigned successfully" });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, "Internal server error: " + ex.Message);
//            }
//        }
//    }
//}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimaryConnect.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PrimaryConnect.Models;
using Microsoft.AspNetCore.Hosting;
using PrimaryConnect.Dto;

[Route("api/[controller]")]
[ApiController]
public class HomeworkController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env; // Store the injected web host env

    public HomeworkController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env; // Initialize the environment
    }

    [HttpPost("assign-homework")]
    public async Task<IActionResult> AssignHomework([FromForm] HomeworkUploadRequest dto)
    {
        if (dto.File == null || (dto.UserId == null && dto.ClassId == null))
            return BadRequest("File and either UserId or ClassId must be provided.");

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, dto.File.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        var baseHomework = new Homework
        {
            Title = "Auto Homework",
            Description = "Auto Description",
            FilePath = filePath,
            DateAssigned = DateTime.Now
        };

        if (dto.AssignedToAll && dto.ClassId != null)
        {
            var users = await _context.Students
                .Where(u => u.ClassId == dto.ClassId)
                .ToListAsync();

            foreach (var user in users)
            {
                _context.homeworks.Add(new Homework
                {
                    Title = baseHomework.Title,
                    Description = baseHomework.Description,
                    FilePath = baseHomework.FilePath,
                    UserId = user.Id,
                    ClassId = dto.ClassId,
                    AssignedToAll = true,
                    DateAssigned = baseHomework.DateAssigned
                });
            }
        }
        else if (dto.UserId != null)
        {
            baseHomework.UserId = dto.UserId;
            baseHomework.ClassId = dto.ClassId;
            baseHomework.AssignedToAll = false;

            _context.homeworks.Add(baseHomework);
        }

        await _context.SaveChangesAsync();
        return Ok("Homework assigned successfully.");
    }
    
    
    [HttpGet("student/{id}")]
    public async Task<IActionResult> GetHomeworksByStudent(int id)
    {
        var data = await _context.homeworks
            .Where(h => h.UserId == id)
            .ToListAsync();
        return Ok(data);
    }

    // GET: api/homework/parent/5
    [HttpGet("parent/{id}")]
    public async Task<IActionResult> GetHomeworksByParent(int id)
    {
        var parent = await _context.Parents.FindAsync(id);
        if (parent == null)
            return NotFound("Parent not found");

        var data = await _context.homeworks
            .Where(h => h.UserId == parent.Id)
            .ToListAsync();
        return Ok(data);
    }

    // GET: api/homework/download/filename.pdf
    [HttpGet("download/{fileName}")]
    public IActionResult Download(string fileName)
    {
        var path = Path.Combine(_env.WebRootPath, "uploads", fileName);
        if (!System.IO.File.Exists(path))
            return NotFound();

        var contentType = "application/octet-stream";
        return PhysicalFile(path, contentType, fileName);
    }

    // GET: /uploads/filename.pdf
    [HttpGet("/uploads/{fileName}")]
    public IActionResult ViewFile(string fileName)
    {
        var path = Path.Combine(_env.WebRootPath, "uploads", fileName);
        if (!System.IO.File.Exists(path))
            return NotFound();

        var contentType = "application/octet-stream";
        return PhysicalFile(path, contentType);
    }

    // DELETE: api/homework/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHomework(int id)
    {
        var hw = await _context.homeworks.FindAsync(id);
        if (hw == null) return NotFound();

        var filePath = Path.Combine(_env.WebRootPath, "uploads", hw.FilePath);
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        _context.homeworks.Remove(hw);
        await _context.SaveChangesAsync();
        return Ok("Deleted successfully");
    }

    // POST: api/homework/delete-multiple
    [HttpPost("delete-multiple")]
    public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
    {
        var hws = await _context.homeworks.Where(h => ids.Contains(h.Id)).ToListAsync();

        foreach (var hw in hws)
        {
            var path = Path.Combine(_env.WebRootPath, "uploads", hw.FilePath);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _context.homeworks.Remove(hw);
        }

        await _context.SaveChangesAsync();
        return Ok("Multiple homeworks deleted");
    }

    // PUT: api/homework/5
    //[HttpPut("{id}")]
    //public async Task<IActionResult> UpdateHomework(int id, [FromBody] HomeworkUpdateDto dto)
    //{
    //    var hw = await _context.homeworks.FindAsync(id);
    //    if (hw == null) return NotFound();

    //    hw.Title = dto.Title ?? hw.Title;
    //    hw.Description = dto.Description ?? hw.Description;
    //    hw.EndDate = dto.EndDate ?? hw.EndDate;

    //    await _context.SaveChangesAsync();
    //    return Ok("Homework updated");
    //}

    //// GET: api/homework/filters
    //[HttpGet("filters")]
    //public async Task<IActionResult> GetFilters()
    //{
    //    var subjects = await _context.homeworks
    //        .Select(h => h.Subject)
    //        .Distinct()
    //        .ToListAsync();

    //    var types = await _context.homeworks
    //        .Select(h => Path.GetExtension(h.FilePath).ToLower())
    //        .Distinct()
    //        .ToListAsync();

    //    return Ok(new { subjects, types });
    //}


    [HttpGet]
    public async Task<IActionResult> GetAllHomeworks()
    {
        var homeworks = await _context.homeworks.ToListAsync();
        return Ok(homeworks);
    }
}




