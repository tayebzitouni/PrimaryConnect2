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
    public class Teacher_ClassController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Teacher_ClassController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost("assign")]
        public async Task<IActionResult> AssignTeacherToClass([FromBody] Teacher_classDto assignment)
        {
            // Check if teacher exists
            var teacherExists = await _context.Teachers.AnyAsync(t => t.Id == assignment.TeacherId);
            if (!teacherExists)
                return BadRequest("Invalid TeacherId.");

            foreach (int i in assignment.ClassId)
            {
                var classExists = await _context.Classes.AnyAsync(c => c.id == i);
                if (!classExists)
                    return BadRequest($"Invalid ClassId: {i}");

                var temp = assignment.ToSubject(); // Make sure this returns a new instance each time
                temp.ClassId = i;
                _context.TeacherClasses.Add(temp);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Teacher assigned to class successfully.",
            });
        }





        // GET: api/TeacherClass/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Teacher_classDto>>> GetAllAssignments()
        {
            var assignments = await _context.TeacherClasses
            
                .ToListAsync();

            return Ok(assignments);
        }

        // GET: api/TeacherClass/by-teacher/5
        [HttpGet("by-teacher/{teacherId}")]
        public async Task<ActionResult<IEnumerable<Teacher_classDto>>> GetAssignmentsByTeacher(int teacherId)
        {
            var assignments = await _context.TeacherClasses
                .Where(tc => tc.TeacherId == teacherId)
                .Include(tc => tc.Class)
                .ToListAsync();

            return Ok(assignments);
        }

        // GET: api/TeacherClass/by-class/3
        [HttpGet("by-class/{classId}")]
        public async Task<ActionResult<IEnumerable<Teacher_ClassController>>> GetAssignmentsByClass(int classId)
        {
            var assignments = await _context.TeacherClasses
                .Where(tc => tc.ClassId == classId)
                .Include(tc => tc.Teacher)
                .ToListAsync();

            return Ok(assignments);
        }

        // DELETE: api/TeacherClass/delete/1
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var assignment = await _context.TeacherClasses.FindAsync(id);

            if (assignment == null)
                return NotFound("Assignment not found.");

            _context.TeacherClasses.Remove(assignment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Assignment deleted successfully." });
        }
 }
}


