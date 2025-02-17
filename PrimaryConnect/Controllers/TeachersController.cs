using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect.Data;
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

                    return Ok(teacher);
                }
                else
                {
                    return BadRequest("the Password is uncorrect");
                }

            }
            else
                return NotFound();
        }


    }
}

