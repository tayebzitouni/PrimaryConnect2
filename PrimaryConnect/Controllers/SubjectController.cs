using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubjectController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Subject
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjects()
        {
            return await _context.subjects
                .Select(s => new SubjectDto
                {
                    id = s.id,
                    Name = s.Name
                })
                .ToListAsync();
        }

        // GET: api/Subject/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectDto>> GetSubject(int id)
        {
            var subject = await _context.subjects.FindAsync(id);

            if (subject == null)
            {
                return NotFound();
            }

            return new SubjectDto
            {
                id = subject.id,
                Name = subject.Name
            };
        }

        // POST: api/Subject
        [HttpPost]
        public async Task<ActionResult<SubjectDto>> CreateSubject(SubjectDto subjectDto)
        {
            var subject = subjectDto.ToSubject();

            _context.subjects.Add(subject);
            await _context.SaveChangesAsync();

            subjectDto.id = subject.id;

            return CreatedAtAction(nameof(GetSubject), new { id = subject.id }, subjectDto);
        }

        // PUT: api/Subject/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, SubjectDto subjectDto)
        {
            if (id != subjectDto.id)
            {
                return BadRequest();
            }

            var subject = await _context.subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            subject.Name = subjectDto.Name;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Subject/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.subjects.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            _context.subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
