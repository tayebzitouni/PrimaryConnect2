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
    public class MarkController : Controller
    {
        public MarkController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        private AppDbContext _context;


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mark_Dto>>> GetAllMarks()
        {
            return await _context.marks
                .Select(m => new Mark_Dto
                {
                    Id = m.Id,
                    subjectid = m.SubjectId,
                    Mark = m.mark,
                    Remarque = m.Remarque,
                    Semestre = m.Semestre,
                    Year = m.Year,
                    StudentId = m.StudentId
                })
                .ToListAsync();
        }

        // GET: api/Marks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Mark_Dto>> GetMarkById(int id)
        {
            var mark = await _context.marks.FindAsync(id);
            if (mark == null)
                return NotFound();

            return new Mark_Dto
            {
                Id = mark.Id,
                subjectid = mark.SubjectId,
                Mark = mark.mark,
                Remarque = mark.Remarque,
                Semestre = mark.Semestre,
                Year = mark.Year,
                StudentId = mark.StudentId
            };
        }

        // GET: api/Marks/get-student-marks?studentId=1&semester=1&year=2024
        [HttpGet("get-student-marks")]
        public async Task<ActionResult<IEnumerable<Mark_Dto>>> GetStudentMarks(int studentId, int semester, int year)
        {
            var marks = await _context.marks
                .Where(m => m.StudentId == studentId && m.Semestre == semester && m.Year == year)
                .Select(m => new Mark_Dto
                {
                    Id = m.Id,
                    subjectid = m.SubjectId,
                    Mark = m.mark,
                    Remarque = m.Remarque,
                    Semestre = m.Semestre,
                    Year = m.Year,
                    StudentId = m.StudentId
                })
                .ToListAsync();

            if (!marks.Any())
                return NotFound("No marks found for the specified parameters.");

            return Ok(marks);
        }

        // POST: api/Marks
        [HttpPost]
        public async Task<ActionResult<Mark_Dto>> CreateMark(Mark_Dto markDto)
        {
            var mark = markDto.ToMark();
            _context.marks.Add(mark);
            await _context.SaveChangesAsync();

            markDto.Id = mark.Id;
            return CreatedAtAction(nameof(GetMarkById), new { id = mark.Id }, markDto);
        }

        // PUT: api/Marks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMark(int id, Mark_Dto markDto)
        {
            if (id != markDto.Id)
                return BadRequest();

            var mark = await _context.marks.FindAsync(id);
            if (mark == null)
                return NotFound();

            mark.SubjectId = markDto.subjectid;
            mark.mark = markDto.Mark;
            mark.Remarque = markDto.Remarque;
            mark.Semestre = markDto.Semestre;
            mark.Year = markDto.Year;
            mark.StudentId = markDto.StudentId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Marks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMark(int id)
        {
            var mark = await _context.marks.FindAsync(id);
            if (mark == null)
                return NotFound();

            _context.marks.Remove(mark);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }


}

