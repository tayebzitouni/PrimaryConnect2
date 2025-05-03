using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect.Data;
using PrimaryConnect.Models;
using PrimaryConnect.Dto;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class HomeworkController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public HomeworkController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost("assign-homework")]
    public async Task<IActionResult> AssignHomework([FromForm] HomeworkUploadRequest dto)
    {
        if (dto.File == null || dto.ClassId == null || !dto.ClassId.Any())
            return BadRequest("File and at least one ClassId are required.");

        // Correct way to get UserId
        var teacherId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
        if (teacherId == null)
            return Unauthorized("Teacher session not found.");

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, dto.File.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        string contentType = dto.File.ContentType;
        string fileCategory = contentType switch
        {
            var c when c.StartsWith("image/") => "Image",
            var c when c.StartsWith("video/") => "Video",
            var c when c.StartsWith("audio/") => "Audio",
            "application/pdf" => "PDF",
            _ => "Other"
        };

        var homework = new Homework
        {
            FilePath = filePath,
            Type = fileCategory,
            Subject = dto.Subject,
            teacherId = teacherId,  // <<< here .Value
            ClassId = dto.ClassId,
            AssignedToAll = dto.AssignedToAll,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };

        _context.homeworks.Add(homework);
        await _context.SaveChangesAsync();
        return Ok("Homework assigned successfully.");
    }


    //[HttpGet("student/{id}")]
    //public async Task<IActionResult> GetHomeworksByStudent(int id)
    //{
    //    var student = await _context.Students.FindAsync(id);
    //    if (student == null)
    //        return NotFound("Student not found.");

    //    var allHomeworks = await _context.homeworks
    //        .Where(h => h.AssignedToAll)
    //        .ToListAsync();

    //    var matchedHomeworks = allHomeworks
    //        .Where(h => h.ClassId != null && h.ClassId.Contains(student.ClassId))
    //        .Select(h => new
    //        {
    //            h.Id,
    //            h.Subject,
    //            h.Type,
    //            h.teacherId,
    //            h.AssignedToAll,
    //            FileName = Path.GetFileName(h.FilePath),
    //            DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/homework/download/{Path.GetFileName(h.FilePath)}",
    //            ViewUrl = $"{Request.Scheme}://{Request.Host}/uploads/{Path.GetFileName(h.FilePath)}",
    //            StartDate = h.StartDate.ToString("yyyy-MM-dd"),
    //            EndDate = h.EndDate.ToString("yyyy-MM-dd")
    //        })
    //        .ToList();

    //    return Ok(matchedHomeworks);
    //}

    [HttpGet("parent/{id}")]
    public async Task<IActionResult> GetHomeworksByParent(int id)
    {
        var parent = await _context.Parents.FindAsync(id);
        if (parent == null)
            return NotFound("Parent not found");

        var data = await _context.homeworks
            .Where(h => h.teacherId == parent.Id) // You might want to adjust logic here
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("download/{id}")]
    public async Task<IActionResult> Download(int id)
    {
        var homework = await _context.homeworks.FindAsync(id);
        if (homework == null)
            return NotFound("Homework not found.");

        var path = homework.FilePath;
        if (!System.IO.File.Exists(path))
            return NotFound("File not found on server.");

        var fileName = Path.GetFileName(path);
        var contentType = "application/octet-stream";
        return PhysicalFile(path, contentType, fileName);
    }

    [HttpGet("view/{id}")]
    public async Task<IActionResult> ViewFile(int id)
    {
        var homework = await _context.homeworks.FindAsync(id);
        if (homework == null)
            return NotFound("Homework not found.");

        var path = homework.FilePath;
        if (!System.IO.File.Exists(path))
            return NotFound("File not found on server.");

        var contentType = GetContentType(path);

        var fileBytes = await System.IO.File.ReadAllBytesAsync(path);
        Response.Headers.Add("Content-Disposition", $"inline; filename={Path.GetFileName(path)}");

        return File(fileBytes, contentType);
    }

    //[HttpGet("view/{id}")]
    //public async Task<IActionResult> ViewFile(int id)
    //{
    //    var homework = await _context.homeworks.FindAsync(id);
    //    if (homework == null)
    //        return NotFound("Homework not found.");

    //    var path = homework.FilePath;
    //    if (!System.IO.File.Exists(path))
    //        return NotFound("File not found on server.");

    //    var contentType = GetContentType(path);
    //    return PhysicalFile(path, contentType);
    //}

    // Helper method to get the right content type
    private string GetContentType(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".mp4" => "video/mp4",
            ".mp3" => "audio/mpeg",
            _ => "application/octet-stream",
        };
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHomework(int id)
    {
        var hw = await _context.homeworks.FindAsync(id);
        if (hw == null) return NotFound();

        var filePath = Path.Combine(_env.WebRootPath, "uploads", Path.GetFileName(hw.FilePath));
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        _context.homeworks.Remove(hw);
        await _context.SaveChangesAsync();
        return Ok("Deleted successfully");
    }

    [HttpPost("delete-multiple")]
    public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
    {
        var hws = await _context.homeworks.Where(h => ids.Contains(h.Id)).ToListAsync();

        foreach (var hw in hws)
        {
            var path = Path.Combine(_env.WebRootPath, "uploads", Path.GetFileName(hw.FilePath));
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _context.homeworks.Remove(hw);
        }

        await _context.SaveChangesAsync();
        return Ok("Multiple homeworks deleted");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllHomeworks()
    {
        var homeworks = await _context.homeworks.ToListAsync();
        return Ok(homeworks);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateHomework(int id, [FromForm] HomeworkUploadRequest dto)
    {
        var homework = await _context.homeworks.FindAsync(id);
        if (homework == null)
            return NotFound("Homework not found.");

        // Update fields (subject, start date, end date, etc.)
        homework.Subject = dto.Subject;
        homework.StartDate = dto.StartDate;
        homework.EndDate = dto.EndDate;
        homework.AssignedToAll = dto.AssignedToAll;
        homework.ClassId = dto.ClassId; // Optional update for ClassId

        // If a new file is uploaded
        if (dto.File != null)
        {
            // Delete the old file (if it exists)
            if (System.IO.File.Exists(homework.FilePath))
                System.IO.File.Delete(homework.FilePath);

            // Save the new file
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.File.FileName);
            var filePath = Path.Combine(uploadsFolder, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            homework.FilePath = filePath;

            // Determine new file type (based on content type)
            string contentType = dto.File.ContentType;
            homework.Type = contentType switch
            {
                var c when c.StartsWith("image/") => "Image",
                var c when c.StartsWith("video/") => "Video",
                var c when c.StartsWith("audio/") => "Audio",
                "application/pdf" => "PDF",
                _ => "Other"
            };
        }

        // Save changes to the database
        await _context.SaveChangesAsync();

        return Ok("Homework updated successfully.");
    
    }


    [HttpGet("homeworks-by-student/{studentId}")]
    public async Task<IActionResult> GetHomeworksByStudent(int studentId)
    {
        // Get the student's class ID from the database (assuming you have a Student model and table)
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
            return NotFound("Student not found.");

        // Get all homeworks assigned to the class or to all students
        var homeworks = await _context.homeworks
            .Where(h => h.AssignedToAll || (h.ClassId != null && h.ClassId.Contains(student.ClassId)))
            .Select(h => new
            {
                h.Id,
                h.Subject,
                h.Type,
                h.StartDate,
                h.EndDate,
                h.AssignedToAll,
                h.FilePath,
                DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/homework/download/{Path.GetFileName(h.FilePath)}"
            })
            .ToListAsync();

        // If no homeworks found
        if (!homeworks.Any())
            return NotFound("No homeworks assigned to the student.");

        return Ok(homeworks);
    }


    [HttpGet("homeworks-by-teacher/{teacherId}")]
    public async Task<IActionResult> GetHomeworksByTeacher(int teacherId)
    {
        var homeworks = await _context.homeworks
            .Where(h => h.teacherId == teacherId)
            .Select(h => new
            {
                h.Id,
                h.Subject,
                h.Type,
                h.StartDate,
                h.EndDate,
                h.AssignedToAll,
                h.FilePath,
                DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/homework/download/{Path.GetFileName(h.FilePath)}"
            })
            .ToListAsync();

        if (!homeworks.Any())
            return NotFound("No homeworks found for this teacher.");

        return Ok(homeworks);
    }


}
