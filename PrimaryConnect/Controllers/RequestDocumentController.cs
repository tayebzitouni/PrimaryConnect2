using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;
using PrimaryConnect.Models;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestDocumentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestDocumentController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/RequestDocument/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateRequestDocument([FromBody] RequestDocumentDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid data.");

            var request = dto.ToDocument();
            request.personid = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            request.ParentOrTeacher = HttpContext.Session.GetString("Role");
            request.Date = DateTime.Now.ToString("yyyy-MM-dd");

            _context.requests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Request document created successfully.",
                requestId = request.Id
            });
        }

        // GET: api/RequestDocument/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<RequestDocument>>> GetAllRequestDocuments()
        {
            var requests = await _context.requests.ToListAsync();
            return Ok(requests);
        }

        // PUT: api/RequestDocument/set-approval/5?approve=true
        [HttpPut("set-approval/{id}")]
        public async Task<IActionResult> SetApprovalStatus(int id, [FromQuery] bool approve)
        {
            var request = await _context.requests.FindAsync(id);

            if (request == null)
                return NotFound("Request document not found.");

            request.IsApproved = approve;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = approve ? "Request approved." : "Request rejected.",
                requestId = request.Id,
                status = request.IsApproved
            });
        }

        [HttpGet("parent")]
        public async Task<ActionResult<IEnumerable<RequestDocument>>> GetParentRequests()
        {
            var parentRequests = await _context.requests
                .Where(r => r.ParentOrTeacher == "Parent")
                .ToListAsync();

            return Ok(parentRequests);
        }

        [HttpGet("teacher")]
        public async Task<ActionResult<IEnumerable<RequestDocument>>> GetTeacherRequests()
        {
            var teacherRequests = await _context.requests
                .Where(r => r.ParentOrTeacher == "Teacher")
                .ToListAsync();

            return Ok(teacherRequests);

        }

            [HttpGet("approved")]
            public async Task<ActionResult<IEnumerable<RequestDocument>>> GetApprovedRequests()
            {
                var approvedRequests = await _context.requests
                    .Where(r => r.IsApproved == true)
                    .ToListAsync();

                return Ok(approvedRequests);
            }

        [HttpGet("nonapproved")]
        public async Task<ActionResult<IEnumerable<RequestDocument>>> GetNonApprovedRequests()
        {
            var nonApprovedRequests = await _context.requests
                .Where(r => r.IsApproved == false)
                .ToListAsync();

            return Ok(nonApprovedRequests);
        }

    }
    }

