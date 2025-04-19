using Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;
using PrimaryConnect.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event_Dto>>> GetEvents()
        {
            var events = await _context.events.ToListAsync();

            var eventDtos = events.Select(e => new Event_Dto
            {
                title = e.title,
                Description = e.Description,
                date = e.date,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                All = e.All,
                level = e.level,
                Color = e.Color
            }).ToList();

            return Ok(eventDtos);
        }


        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var _event = await _context.events.FindAsync(id);

            if (_event == null)
            {
                return NotFound();
            }

            return _event;
        }

        // POST: api/Events
        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent([FromBody] Event_Dto dto)
        {
            var _event = dto.ToEvent();
            _context.events.Add(_event);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = _event.Id }, _event);
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(int id, [FromBody] Event_Dto dto)
        {
            var existingEvent = await _context.events.FindAsync(id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            // Update properties
            existingEvent.title = dto.title;
            existingEvent.Description = dto.Description;
            existingEvent.date = dto.date;
            existingEvent.StartTime = dto.StartTime;
            existingEvent.EndTime = dto.EndTime;
            existingEvent.All = dto.All;
            existingEvent.level = dto.level;

            _context.Entry(existingEvent).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var _event = await _context.events.FindAsync(id);
            if (_event == null)
            {
                return NotFound();
            }

            _context.events.Remove(_event);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("GetEventsForStudent/{studentId}")]
        public async Task<IActionResult> GetEventsForStudent(int studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var events = await _context.events
                .Where(e => e.All || e.level == student.Degree)
                .ToListAsync();

            // Map to DTOs if needed
            var eventDtos = events.Select(e => new Event_Dto
            {
                title = e.title,
                Description = e.Description,
                date = e.date,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                All = e.All,
                level = e.level,
                Color = e.Color
            }).ToList();

            return Ok(eventDtos);
        }


    }
}
