
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
    public class StudentsController : ControllerBase
    {
        public StudentsController(AppDbContext appDbContext)
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;


        [HttpGet("GetAllStudents")]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            return await _PrimaryConnect_Db.Students.ToListAsync();
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}", Name = "GetStudentById")]
        public async Task<ActionResult<Administrator>> GetStudentById(int id)
        {
            var admin = await _PrimaryConnect_Db.Students.FindAsync(id);

            if (admin == null)
            {
                return NotFound("Student not found.");
            }

            return Ok(admin);
        }



        [HttpPost("AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Administrator>> AddStudent( Student_Dto student)
        {
            if (student == null)
            {
                return BadRequest("Student data is required.");
            }
            Student _student = student.ToStudent();
            _PrimaryConnect_Db.Students.Add(_student);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return Ok(student);
        }


        // DELETE: api/admins/{id}
        [HttpDelete("{id}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _PrimaryConnect_Db.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            _PrimaryConnect_Db.Students.Remove(student);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}", Name = "UpdateStudent")]
        public async Task<IActionResult> UpdateStudent(int id, Student_Dto updatedStudent)
        {
            if (id != updatedStudent.Id)
            {
                return BadRequest("ID mismatch.");
            }

          Student? existingStudent= await _PrimaryConnect_Db.Students.FindAsync(id);
            if (existingStudent == null)
            {
                return NotFound("Student not found.");
            }
            existingStudent=updatedStudent.ToStudent();


            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }



    }

}
