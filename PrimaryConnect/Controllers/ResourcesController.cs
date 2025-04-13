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

        var resource = new Resource
        {
            Subject = dto.Subject,
            FileName = dto.File.FileName,
            Type = fileType,
            TeacherRemark = dto.TeacherRemark
        };

        _context.resources.Add(resource);
        await _context.SaveChangesAsync();

        return Ok("Resource uploaded successfully.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllResources()
    {
        var resources = await _context.resources
            .Select(r => new {
                Id = r.Id,
                Subject = r.Subject,
                FileName = r.FileName,
                Type = r.Type,
                TeacherRemark = r.TeacherRemark
            })
            .ToListAsync();

        return Ok(resources);
    }

    [HttpGet("download/{fileName}")]
    public IActionResult DownloadResource(string fileName)
    {
        var path = Path.Combine(_env.WebRootPath, "resources", fileName);
        if (!System.IO.File.Exists(path))
            return NotFound();

        var contentType = "application/octet-stream";
        return PhysicalFile(path, contentType, fileName);
    }

    [HttpGet("view/{fileName}")]
    public IActionResult ViewResource(string fileName)
    {
        var path = Path.Combine(_env.WebRootPath, "resources", fileName);
        if (!System.IO.File.Exists(path))
            return NotFound();

        var contentType = GetContentType(fileName);
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
}