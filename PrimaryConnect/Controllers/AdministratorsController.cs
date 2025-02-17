using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect.Data;
using PrimaryConnect.Models;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorsController : ControllerBase
    {
        public AdministratorsController(AppDbContext appDbContext)
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;


        [HttpGet("[action]")]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            Administrator? admin =await _PrimaryConnect_Db.Administrators.SingleOrDefaultAsync(admin => admin.Email == Email);
            if (admin != null) {
                if (admin.Password == Password)
                {return Ok(admin);
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
