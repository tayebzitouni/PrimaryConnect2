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
    public class MarkController : Controller
    {
        public MarkController(AppDbContext appDbContext)
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;


        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllById(int id)
        {
            return Ok(await _PrimaryConnect_Db.marks.Where(m=>m.StudentId==id).ToListAsync());
        }




        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddMark(Mark_Dto mark)
        {
            Marks _mark=mark.ToMark();
            _PrimaryConnect_Db.marks.Add(_mark);
            try
            {
                await _PrimaryConnect_Db.SaveChangesAsync();
            }
            catch
            { return BadRequest(); }

            return Ok(mark);
        }


        // DELETE: api/admins/{id}
        [HttpDelete("{id}", Name = "DeleteMark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMark(int id)
        {
           Marks? mark = await _PrimaryConnect_Db.marks.FindAsync(id);
            if (mark == null)
            {
                return NotFound("Admin not found.");
            }

            _PrimaryConnect_Db.marks.Remove(mark);
            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id}", Name = "UpdateMark")]
        public async Task<IActionResult> UpdateMark(int id, Mark_Dto UpdatedMark)
        {
            if (id != UpdatedMark.Id)
            {
                return BadRequest("ID mismatch.");
            }

            Marks? existingMark = await _PrimaryConnect_Db.marks.FindAsync(id);
            if (existingMark == null)
            {
                return NotFound("Admin not found.");
            }
            existingMark = UpdatedMark.ToMark();

            await _PrimaryConnect_Db.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }



    }
}
