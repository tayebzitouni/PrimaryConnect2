using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmittedDocumentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SubmittedDocumentController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // POST: api/Document/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("No file uploaded.");
            
            // Create upload folder if not exists
            
           
            var uploadPath = Path.Combine(_env.WebRootPath, "SubmitedDocuments/Parent");
            if (HttpContext.Session.GetString("Role") == "Teacher")
            {
                uploadPath = Path.Combine(_env.WebRootPath, "SubmitedDocuments/Teacher");
            }
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Generate unique file name
            var fileName = Guid.NewGuid() + Path.GetExtension(dto.File.FileName);
            var fullPath = Path.Combine(uploadPath, fileName);

            // Save the file
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            // Save metadata to DB
            var document = dto.ToDocument();
            document.FileName = fileName;
            document.Type = Path.GetExtension(fileName);
            document.Date = DateTime.Now.ToString("yyyy-MM-dd");
            document.UploaderRole = HttpContext.Session.GetString("Role");
            document.personid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            _context.documents.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Document uploaded successfully.", documentId = document.Id });
        }

        //// GET: api/Document/all
        //[HttpGet("all")]
        //public async Task<ActionResult<IEnumerable<Document>>> GetAllDocuments()
        //{
        //    var documents = await _context.documents.ToListAsync();

        //    var result = documents.Select(doc => new 
        //    {
        //        Id = doc.Id,
        //        title = doc.title,
        //        Description = doc.Dsecription,

        //        DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/SubmittedDocument/download/{doc.Id}"
        //    }).ToList();

        //    return Ok(result);
        //}
        // GET: api/Document/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllDocuments()
        {
            var documents = await _context.documents.ToListAsync();

            var result = documents.Select(doc => new
            {
                // Copy all properties from Document
                doc.Id,
                doc.title,
                doc.Dsecription,
                doc.Date,       // if you have this or others, add here
                doc.Type,
                doc.personid,
                doc.IsApproved,// example additional fields

                // Add the DownloadUrl
                DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/SubmittedDocument/download/{doc.Id}"
            }).ToList();

            return Ok(result);
        }



        [HttpGet("view/{id}")]
        public async Task<IActionResult> ViewDocument(int id)
        {
            var doc = await _context.documents.FindAsync(id);
            if (doc == null)
                return NotFound("Document not found in database.");

            // Determine folder based on role
            string folder = doc.UploaderRole switch
            {
                "Teacher" => "SubmitedDocuments/Teacher",
                "Parent" => "SubmitedDocuments/Parent",
                _ => "SubmitedDocuments/Unknown"
            };

            var filePath = Path.Combine(_env.WebRootPath, folder, doc.FileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found on server.");

            // Detect content type for browser preview
            var mimeType = UsefulFunctions.GetMimeType(doc.FileName.ToString());

            return PhysicalFile(filePath, mimeType); // ← No download name = browser view
        }

        // GET: api/Document/download/5
        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var doc = await _context.documents.FindAsync(id);
            if (doc == null)
                return NotFound();

            string folder = "SubmitedDocuments/Parent";
            if (HttpContext.Session.GetString("Role") == "Teacher")
            {
                folder = "SubmitedDocuments/Teacher";
            }

            var path = Path.Combine(_env.WebRootPath, folder, doc.FileName);

            if (!System.IO.File.Exists(path))
                return NotFound("File not found on server.");

            var mime = "application/octet-stream";
            return PhysicalFile(path, mime, doc.FileName);
        }

        [HttpPut("set-approval/{id}")]
        public async Task<IActionResult> SetApprovalStatus(int id, [FromQuery] bool approve)
        {
            var document = await _context.documents.FindAsync(id);

            if (document == null)
                return NotFound("Document not found.");

            document.IsApproved = approve;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = approve ? "Document approved." : "Document rejected.",
                documentId = document.Id,
                status = document.IsApproved
            });
        }

        //[HttpGet("teacher")]
        //public async Task<ActionResult<IEnumerable<DocumentDto>>> GetTeacherDocuments()
        //{
        //    var teacherDocs = await _context.documents
        //        .Where(d => d.UploaderRole == "Teacher")
        //        .ToListAsync();

        //    return Ok(teacherDocs);
        //}

        // GET: api/Document/parent
        //[HttpGet("parent")]
        //public async Task<ActionResult<IEnumerable<DocumentDto>>> GetParentDocuments()
        //{
        //    var parentDocs = await _context.documents
        //        .Where(d => d.UploaderRole == "Parent")
        //        .ToListAsync();

        //    return Ok(parentDocs);
        //}

        [HttpGet("teacher")]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetTeacherDocumentsWithUrl()
        {
            var documents = await _context.documents
                .Where(d => d.UploaderRole == "Teacher")
                .ToListAsync();

            var result = documents.Select(doc => new
            {
                // Copy all properties from Document
                doc.Id,
                doc.title,
                doc.Dsecription,
                doc.Date,       // if you have this or others, add here
                doc.Type,
                doc.personid,
                doc.IsApproved,// example additional fields

                // Add the DownloadUrl
                DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/SubmittedDocument/download/{doc.Id}"
            }).ToList();


            return Ok(result);
        }
        [HttpGet("parent")]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetParentDocuments()
        {
            var documents = await _context.documents
                .Where(d => d.UploaderRole == "Parent")
                .ToListAsync();

            var result = documents.Select(doc => new
            {
                // Copy all properties from Document
                doc.Id,
                doc.title,
                doc.Dsecription,
                doc.Date,       // if you have this or others, add here
                doc.Type,
                doc.personid,
                doc.IsApproved,
                // example additional fields

                // Add the DownloadUrl
                DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/SubmittedDocument/download/{doc.Id}"
            }).ToList();


            return Ok(result);
        }


    }
}

