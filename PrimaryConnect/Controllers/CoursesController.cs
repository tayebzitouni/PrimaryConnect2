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
    public class CoursesController : ControllerBase
    {
        public CoursesController(AppDbContext appDbContext)
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;


        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllById(int id)
        {
            return Ok(await _PrimaryConnect_Db.courses.Where(m => m.StudentId == id).ToListAsync());
        }




        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCourse(Course_Dto Course)
        {
            Courses _Course = Course.ToCourse();
            _PrimaryConnect_Db.
                courses.Add(_Course);
            try
            {
                await _PrimaryConnect_Db.SaveChangesAsync();
            }
            catch
            { return BadRequest(); }

            return Ok(Course);
        }


        // DELETE: api/admins/{id}
        [HttpDelete("{id}", Name = "DeleteCourse")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            Courses? _Course = await _PrimaryConnect_Db.courses.FindAsync(id);
            if (_Course == null)
            {
                return NotFound("Admin not found.");
            }

            _PrimaryConnect_Db.courses.Remove(_Course);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}", Name = "UpdateCourse")]
        public async Task<IActionResult> UpdateCourse(int id, Course_Dto Updated_Course)
        {
            if (id != Updated_Course.Id)
            {
                return BadRequest("ID mismatch.");
            }

            Courses? existing_Absence = await _PrimaryConnect_Db.courses.FindAsync(id);
            if (existing_Absence == null)
            {
                return NotFound("Admin not found.");
            }
            existing_Absence = Updated_Course.ToCourse();

            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

    }
}
