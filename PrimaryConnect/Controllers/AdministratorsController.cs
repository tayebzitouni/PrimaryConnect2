using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using PrimaryConnect.Data;
using PrimaryConnect.Dto;
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


        //[HttpPost("login/admin")]
        //public async Task<IActionResult> (string email, string password)
        //{

        //}

        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> FrogetPassword(string Email)
        {
            if (await _PrimaryConnect_Db.Administrators.AnyAsync(t => t.Email == Email))
            {

                return Ok(UsefulFunctions.GenerateandSendKey(Email));
            }
            return NotFound();

        }
        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> ChangePassword(string Email, string Password)
        {
            Administrator? Admin = await _PrimaryConnect_Db.Administrators.SingleOrDefaultAsync(t => t.Email == Email);
            if (Admin == null) { return NotFound(); }
            Admin.Password = Password;
            _PrimaryConnect_Db.Administrators.Update(Admin);
            await _PrimaryConnect_Db.SaveChangesAsync();
            return Ok();

        }
        
        
        [HttpGet("GetAllAdmins")]
        public async Task<ActionResult<IEnumerable<Administrator>>> GetAllAdmins()
        {
            return await _PrimaryConnect_Db.Administrators.ToListAsync();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Administrator>> GetAdminById(int id)
        {
            var admin = await _PrimaryConnect_Db.Administrators.FindAsync(id);

            if (admin == null)
            {
                return NotFound("Admin not found.");
            }

            return Ok(admin);
        }


        
        [HttpPost("AddAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddAdmin(Admin_Dto admin)
        {

            Administrator _admin = admin.ToAdministrator();
            _PrimaryConnect_Db.Administrators.Add(_admin);
            try
            {
                await _PrimaryConnect_Db.SaveChangesAsync();
            }
            catch
            { return BadRequest(); }

            return Ok(admin);
        }

        [Authorize]
        // DELETE: api/admins/{id}
        [HttpDelete("{id}", Name = "DeleteAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _PrimaryConnect_Db.Administrators.FindAsync(id);
            if (admin == null)
            {
                return NotFound("Admin not found.");
            }

            _PrimaryConnect_Db.Administrators.Remove(admin);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}", Name = "UpdateAdmin")]
        public async Task<IActionResult> UpdateAdmin(int id, Admin_Dto updatedAdmin)
        {
            if (id != updatedAdmin.Id)
            {
                return BadRequest("ID mismatch.");
            }

            Administrator? existingAdmin = await _PrimaryConnect_Db.Administrators.FindAsync(id);
            if (existingAdmin == null)
            {
                return NotFound("Admin not found.");
            }

            // Map fields manually or use a mapper
            existingAdmin.Name = updatedAdmin.Name;
            existingAdmin.Email = updatedAdmin.Email;
            existingAdmin.PhoneNumber = updatedAdmin.PhoneNumber ;
            existingAdmin.Password = updatedAdmin.Password;
            existingAdmin.Permitions = updatedAdmin.Permitions; 

            // Add other fields as necessary...

            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent();
        }



    }
}
