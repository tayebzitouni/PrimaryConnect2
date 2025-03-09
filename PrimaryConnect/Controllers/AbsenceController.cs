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
    public class AbsenceController : ControllerBase
    {

        public AbsenceController(AppDbContext appDbContext)
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;


        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllById(int id)
        {
            return Ok(await _PrimaryConnect_Db.absences.Where(m => m.StudentId == id).ToListAsync());
        }




        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddAbsence(Absence_Dto Absence)
        {
            Absence _Absence = Absence.ToAbsence();
            _PrimaryConnect_Db.
                absences.Add(_Absence);
            try
            {
                await _PrimaryConnect_Db.SaveChangesAsync();
            }
            catch
            { return BadRequest(); }

            return Ok(Absence);
        }


        // DELETE: api/admins/{id}
        [HttpDelete("{id}", Name = "DeleteAbsence")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAbsence(int id)
        {
            Absence? _Absence = await _PrimaryConnect_Db.absences.FindAsync(id);
            if (_Absence == null)
            {
                return NotFound("Admin not found.");
            }

            _PrimaryConnect_Db.absences.Remove(_Absence);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}", Name = "UpdateAbsence")]
        public async Task<IActionResult> UpdateAbsence(int id, Absence_Dto Updated_Absence)
        {
            if (id != Updated_Absence.Id)
            {
                return BadRequest("ID mismatch.");
            }

            Absence? existing_Absence = await _PrimaryConnect_Db.absences.FindAsync(id);
            if (existing_Absence == null)
            {
                return NotFound("Admin not found.");
            }
            existing_Absence = Updated_Absence.ToAbsence();

            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }


    }
}
