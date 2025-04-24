using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;
using PrimaryConnect.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClassController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/class
        [HttpPost]
        public async Task<IActionResult> CreateClass([FromBody] ClassDto dto)
        {
            if (dto == null)
                return BadRequest("Class data is required.");
            Class myclass = dto.ClassDtoToClass();
            _context.Classes.Add(myclass);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClass), new { id = dto.id }, dto);
        }

        // GET: api/class/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetClass(int id)
        {
            var classData = await _context.Classes
              
                .FirstOrDefaultAsync(c => c.id == id);

            if (classData == null)
                return NotFound();

            return Ok(classData);
        }


        [HttpGet("a/{name}", Name = "GetClassByname")]
        public async Task<IActionResult> GetClassByname(string name)
        {
            var classData = await _context.Classes

                .FirstOrDefaultAsync(c => c.name == name);

            if (classData == null)
                return NotFound();

            return Ok(classData);
        }

        // GET: api/class
        [HttpGet]
        public async Task<IActionResult> GetAllClasses()
        {
            var classes = await _context.Classes
                .ToListAsync();

            return Ok(classes);
        }

        // PUT: api/class/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(int id, ClassDto updatedAdmin)
        {
            if (id != updatedAdmin.id)
            {
                return BadRequest("ID mismatch.");
            }

            Class? existingAdmin = await _context.Classes.FindAsync(id);
            if (existingAdmin == null)
            {
                return NotFound("Admin not found.");
            }

            // Map fields manually or use a mapper
            existingAdmin.name = updatedAdmin.Name;
            existingAdmin.SchoolId = updatedAdmin.schoolid;
            existingAdmin.level = updatedAdmin.level;
          

            // Add other fields as necessary...

            await _context.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/class/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var classToDelete = await _context.Classes.FindAsync(id);
            if (classToDelete == null)
                return NotFound();

            _context.Classes.Remove(classToDelete);
            await _context.SaveChangesAsync();

            return Ok("Class deleted successfully.");
        }

        // GET: api/class/school/5
        [HttpGet("school/{schoolId}")]
        public async Task<IActionResult> GetClassesBySchool(int schoolId)
        {
            var classes = await _context.Classes
                .Where(c => c.SchoolId == schoolId)
                .ToListAsync();

            if (classes == null || !classes.Any())
                return NotFound("No classes found for the specified school.");

            return Ok(classes);
        }

        // GET: api/class/teachers/5
        [HttpGet("teachers/{teacherId}")]
        public async Task<IActionResult> GetClassesByTeacher(int teacherId)
        {
            var classes = await _context.Classes
                .Where(c => c.teachers.Any(t => t.Id == teacherId))
                .ToListAsync();

            if (classes == null || !classes.Any())
                return NotFound("No classes found for the specified teacher.");

            return Ok(classes);
        }


        [HttpGet("level")]
        public async Task<IActionResult> GetClassesByLevel(int level)
        {
            var classes = await _context.Classes
                .Where(c => c.level==level)
                .ToListAsync();

            if (classes == null || !classes.Any())
                return NotFound("No classes found for the specified level.");

            return Ok(classes);
        }
    }
}

