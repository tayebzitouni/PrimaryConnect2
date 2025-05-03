//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using PrimaryConnect.Data;
//using Microsoft.EntityFrameworkCore;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using PrimaryConnect.Models;
//using Microsoft.AspNetCore.Hosting;
//using PrimaryConnect.Dto;

//[Route("api/[controller]")]
//[ApiController]
//public class ResourcesController : ControllerBase
//{
//    private readonly AppDbContext _context;
//    private readonly IWebHostEnvironment _env;

//    public ResourcesController(AppDbContext context, IWebHostEnvironment env)
//    {
//        _context = context;
//        _env = env;
//    }

//    [HttpPost("upload-resource")]
//    public async Task<IActionResult> UploadResource([FromForm] ResourceUploadRequest dto)
//    {
//        if (dto.File == null)
//            return BadRequest("File must be provided.");

//        var uploadsFolder = Path.Combine(_env.WebRootPath, "resources");
//        if (!Directory.Exists(uploadsFolder))
//            Directory.CreateDirectory(uploadsFolder);

//        var filePath = Path.Combine(uploadsFolder, dto.File.FileName);
//        using (var stream = new FileStream(filePath, FileMode.Create))
//        {
//            await dto.File.CopyToAsync(stream);
//        }

//        string contentType = dto.File.ContentType;
//        string fileType = GetFileType(contentType);

//        var resource = new Resource
//        {
//            Subject = dto.Subject,
//            FileName = dto.File.FileName,
//            Type = fileType,
//            TeacherRemark = dto.TeacherRemark
//        };

//        _context.resources.Add(resource);
//        await _context.SaveChangesAsync();

//        return Ok("Resource uploaded successfully.");
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetAllResources()
//    {
//        var resources = await _context.resources
//            .Select(r => new {
//                Id = r.Id,
//                Subject = r.Subject,
//                FileName = r.FileName,
//                Type = r.Type,
//                TeacherRemark = r.TeacherRemark
//            })
//            .ToListAsync();

//        return Ok(resources);
//    }

//    [HttpGet("download/{fileName}")]
//    public IActionResult DownloadResource(string fileName)
//    {
//        var path = Path.Combine(_env.WebRootPath, "resources", fileName);
//        if (!System.IO.File.Exists(path))
//            return NotFound();

//        var contentType = "application/octet-stream";
//        return PhysicalFile(path, contentType, fileName);
//    }

//    [HttpGet("view/{fileName}")]
//    public IActionResult ViewResource(string fileName)
//    {
//        var path = Path.Combine(_env.WebRootPath, "resources", fileName);
//        if (!System.IO.File.Exists(path))
//            return NotFound();

//        var contentType = GetContentType(fileName);
//        return PhysicalFile(path, contentType);
//    }

//    [HttpDelete("{id}")]
//    public async Task<IActionResult> DeleteResource(int id)
//    {
//        var resource = await _context.resources.FindAsync(id);
//        if (resource == null) return NotFound();

//        var filePath = Path.Combine(_env.WebRootPath, "resources", resource.FileName);
//        if (System.IO.File.Exists(filePath))
//            System.IO.File.Delete(filePath);

//        _context.resources.Remove(resource);
//        await _context.SaveChangesAsync();
//        return Ok("Resource deleted successfully");
//    }

//    [HttpPut("{id}/remark")]
//    public async Task<IActionResult> UpdateRemark(int id, [FromBody] string remark)
//    {
//        var resource = await _context.resources.FindAsync(id);
//        if (resource == null) return NotFound();

//        resource.TeacherRemark = remark;
//        await _context.SaveChangesAsync();
//        return Ok("Remark updated successfully");
//    }

//    private string GetFileType(string contentType)
//    {
//        if (contentType.StartsWith("image/"))
//            return "image";
//        else if (contentType.StartsWith("video/"))
//            return "video";
//        else if (contentType.StartsWith("audio/"))
//            return "audio";
//        else if (contentType == "application/pdf")
//            return "document";
//        else if (contentType == "application/msword" ||
//                 contentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
//            return "document";
//        else
//            return "other";
//    }

//    private string GetContentType(string fileName)
//    {
//        var extension = Path.GetExtension(fileName).ToLower();
//        return extension switch
//        {
//            ".pdf" => "application/pdf",
//            ".doc" => "application/msword",
//            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
//            ".mp4" => "video/mp4",
//            ".jpg" or ".jpeg" => "image/jpeg",
//            ".png" => "image/png",
//            _ => "application/octet-stream"
//        };
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
public class ResourcesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ResourcesController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;

    }

    [HttpPost("upload-resource")]
    public async Task<IActionResult> UploadResource([FromForm] ResourceUploadRequest dto)
    {
        if (dto.File == null)
            return BadRequest("File must be provided.");

        var uploadsFolder = Path.Combine(_env.WebRootPath, "resources");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, dto.File.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        string contentType = dto.File.ContentType;
        string fileType = GetFileType(contentType);

        var teacherId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
        if (teacherId == null)
            return Unauthorized("Teacher is not logged in.");

        var resource = new Resource
        {
            Subject = dto.Subject,
            FileName = dto.File.FileName,
            Type = fileType,
            TeacherRemark = dto.TeacherRemark,
            TeacherId = teacherId,
            date = dto.date,
            level = dto.level,
            AssignedToall = dto.AssignedToall
        };

        _context.resources.Add(resource);
        await _context.SaveChangesAsync();

        return Ok("Resource uploaded successfully.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllResources()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var resources = await _context.resources
            .Select(r => new
            {
                r.Id,
                r.Subject,
                r.FileName,
                r.Type,
                r.TeacherRemark,
                r.date,
                r.level,
                r.AssignedToall,
                DownloadUrl = $"{baseUrl}/api/resources/download/{r.FileName}"
            })
            .ToListAsync();

        return Ok(resources);
    }



    [HttpGet("download/{id}")]
    public async Task<IActionResult> DownloadResource(int id)
    {
        var resource = await _context.resources.FindAsync(id);
        if (resource == null)
            return NotFound("Resource not found.");

        var path = Path.Combine(_env.WebRootPath, "resources", resource.FileName);
        if (!System.IO.File.Exists(path))
            return NotFound("File not found on server.");

        var contentType = "application/octet-stream";
        return PhysicalFile(path, contentType, resource.FileName);
    }

    [HttpGet("view/{id}")]
    public async Task<IActionResult> ViewResource(int id)
    {
        var resource = await _context.resources.FindAsync(id);
        if (resource == null)
            return NotFound("Resource not found.");

        var path = Path.Combine(_env.WebRootPath, "resources", resource.FileName);
        if (!System.IO.File.Exists(path))
            return NotFound("File not found on server.");

        var contentType = GetContentType(resource.FileName);
        return PhysicalFile(path, contentType);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResource(int id)
    {
        var resource = await _context.resources.FindAsync(id);
        if (resource == null) return NotFound();

        var filePath = Path.Combine(_env.WebRootPath, "resources", resource.FileName);
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        _context.resources.Remove(resource);
        await _context.SaveChangesAsync();
        return Ok("Resource deleted successfully");
    }

    [HttpPut("{id}/remark")]
    public async Task<IActionResult> UpdateRemark(int id, [FromBody] string remark)
    {
        var resource = await _context.resources.FindAsync(id);
        if (resource == null) return NotFound();

        resource.TeacherRemark = remark;
        await _context.SaveChangesAsync();
        return Ok("Remark updated successfully");
    }

    private string GetFileType(string contentType)
    {
        if (contentType.StartsWith("image/"))
            return "image";
        else if (contentType.StartsWith("video/"))
            return "video";
        else if (contentType.StartsWith("audio/"))
            return "audio";
        else if (contentType == "application/pdf")
            return "document";
        else if (contentType == "application/msword" ||
                 contentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            return "document";
        else
            return "other";
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLower();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".mp4" => "video/mp4",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
    }


    [HttpGet("my-resources")]
    public async Task<IActionResult> GetMyResources()
    {
        // Get TeacherId from session
        var teacherIdString = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(teacherIdString) || !int.TryParse(teacherIdString, out int teacherId))
            return Unauthorized("Teacher is not logged in.");

        // Fetch resources uploaded by this teacher
        var resources = await _context.resources
            .Where(r => r.TeacherId == teacherId)
            .Select(r => new
            {
                r.Id,
                r.Subject,
                r.FileName,
                r.Type,
                r.TeacherRemark,
                r.date,
                DownloadUrl = Url.Action("DownloadResource", "Resources", new { id = r.Id }, Request.Scheme),
                ViewUrl = Url.Action("ViewResource", "Resources", new { id = r.Id }, Request.Scheme)
            })
            .ToListAsync();

        return Ok(resources);
    }
    [HttpGet("resources-for-student/{studentId}")]
    public async Task<IActionResult> GetResourcesForStudent(int studentId)
    {
        // Find the student
        var student = await _context.Students.FindAsync(studentId);
        if (student == null)
            return NotFound("Student not found.");

        // Get the class of the student
        var studentClass = await _context.Classes.FindAsync(student.ClassId);
        if (studentClass == null)
            return NotFound("Class not found for the student.");

        int level = studentClass.level; // Now we have the level

        // Fetch resources
        var resources = await _context.resources
            .Where(r => r.AssignedToall || r.level == level)
            .Select(r => new
            {
                r.Id,
                r.Subject,
                r.FileName,
                r.Type,
                r.TeacherRemark,
                r.date,
                DownloadUrl = Url.Action("DownloadResource", "Resources", new { id = r.Id }, Request.Scheme),
                ViewUrl = Url.Action("ViewResource", "Resources", new { id = r.Id }, Request.Scheme)
            })
            .ToListAsync();

        return Ok(resources);
    }


}
