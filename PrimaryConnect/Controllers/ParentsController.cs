using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimaryConnect.Data;
using PrimaryConnect.Models;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParentsController : ControllerBase
    {
        public ParentsController(AppDbContext appDbContext)
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;

        [HttpGet("[action]")]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            Parent? parent = await _PrimaryConnect_Db.Parents.SingleOrDefaultAsync(parent => parent.Email == Email);
            if (parent != null)
            {
                if (parent.Password == Password)
                {
                    return Ok(parent);
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
