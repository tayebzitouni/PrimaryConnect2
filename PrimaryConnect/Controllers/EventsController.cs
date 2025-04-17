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
        public EventsController(AppDbContext appDbContext)
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;


        [HttpGet("[action]")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _PrimaryConnect_Db.events.SingleOrDefaultAsync(m => m.Id == id));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll( )
        {
            return Ok(await _PrimaryConnect_Db.events.ToListAsync());
        }



        [HttpPost("{Event}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddEvent(Event_Dto Event)
        {
            Event _Event = Event.ToEvent();
            _PrimaryConnect_Db.
                events.Add(_Event);
            try
            {
                await _PrimaryConnect_Db.SaveChangesAsync();
            }
            catch
            { return BadRequest(); }

            return Ok(Event);
        }


        // DELETE: api/admins/{id}
        [HttpDelete("{id}", Name = "DeleteEvent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMark(int id)
        {
            Event? _Event = await _PrimaryConnect_Db.events.FindAsync(id);
            if (_Event == null)
            {
                return NotFound("Admin not found.");
            }

            _PrimaryConnect_Db.events.Remove(_Event);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}", Name = "UpdateEvent")]
        public async Task<IActionResult> UpdateMark(int id, Event_Dto Updated_Event)
        {
            if (id != Updated_Event.Id)
            {
                return BadRequest("ID mismatch.");
            }

            Event? existing_Absence = await _PrimaryConnect_Db.events.FindAsync(id);
            if (existing_Absence == null)
            {
                return NotFound("Admin not found.");
            }
            existing_Absence = Updated_Event.ToEvent();

            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }


    }
}
