using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;
using PrimaryConnect.Models;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : Controller
    {
        public TeachersController(AppDbContext appDbContext)
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;


        [HttpGet("[action]")]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            Teacher? teacher = await _PrimaryConnect_Db.Teachers.SingleOrDefaultAsync(teacher => teacher.Email == Email);
            if (teacher != null)
            {
                if (teacher.Password == Password)
                {

                    return Ok(teacher.Id );
                }
                else
                {
                    return BadRequest("the Password is uncorrect");
                }

            }
            else
                return NotFound();
        }

        [HttpGet("GetAllTeachers")]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetAllTeachers()
        {
            return await _PrimaryConnect_Db.Teachers.ToListAsync();
        }

        [HttpGet("[action]")]
        public async Task <IActionResult> FrogetPassword(string Email)
        {
            if(await _PrimaryConnect_Db.Teachers.AnyAsync(t=>t.Email == Email))
            {

            return Ok(UsefulFunctions.GenerateandSendKey(Email) );
            }
            return NotFound();  

        }
        [HttpPost("[action]")]
        public async Task<IActionResult> ChangePassword(string Email, string Password)
        {
            Teacher? teacher = await _PrimaryConnect_Db.Teachers.SingleOrDefaultAsync(t=>t.Email==Email);
            if (teacher == null) { return NotFound(); }
            teacher.Password= Password;
            _PrimaryConnect_Db.Teachers.Update(teacher);
            await _PrimaryConnect_Db.SaveChangesAsync();
            return Ok();
            
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}", Name = "GetTeacherById")]
        public async Task<ActionResult<Administrator>> GetTeacherById(int id)
        {
            var admin = await _PrimaryConnect_Db.Teachers.FindAsync(id);

            if (admin == null)
            {
                return NotFound("Teacher not found.");
            }

            return Ok(admin);
        }


        [HttpPost("AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Administrator>> AddTeacher(Teacher_Dto _teacher)
        {
            if (_teacher == null)
            {
                return BadRequest("Student data is required.");
            }
            Teacher teacher=_teacher.ToTeacher();


            _PrimaryConnect_Db.Teachers.Add(teacher);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllTeachers), new { id = teacher.Id }, teacher);
        }


        // DELETE: api/admins/{id}
        [HttpDelete("{id}", Name = "DeleteTeacher")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var admin = await _PrimaryConnect_Db.Teachers.FindAsync(id);
            if (admin == null)
            {
                return NotFound("Teacher not found.");
            }

            _PrimaryConnect_Db.Teachers.Remove(admin);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}", Name = "UpdateTeacher")]
        public async Task<IActionResult> UpdateTeacher(int id, Teacher_Dto teacher)
        {
            if (id != teacher.Id)
            {
                return BadRequest("ID mismatch.");
            }

        Teacher? existingTeacher = await _PrimaryConnect_Db.Teachers.FindAsync(id);
            if (existingTeacher == null)
            {
                return NotFound("Teacher not found.");
            }



            existingTeacher = teacher.ToTeacher();


            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }



    }


}