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
    public class SchoolsController : ControllerBase
    {
        public SchoolsController(AppDbContext appDbContext)
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;
        [HttpPost("[action]")]
        public async Task<IActionResult> add(School_Dto school)
        {
        

            if (ModelState.IsValid)
            {
                School _school = school.ToSchool();
                await _PrimaryConnect_Db.schools.AddAsync(_school);

                _PrimaryConnect_Db.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()

        {
            return Ok(await _PrimaryConnect_Db.schools.ToListAsync());
        }
    }
}