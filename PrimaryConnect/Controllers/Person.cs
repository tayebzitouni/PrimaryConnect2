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
        public class PersonController : ControllerBase
        {
            private readonly AppDbContext _context;

            public PersonController(AppDbContext context)
            {
                _context = context;
            }

            // GET: api/Person
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
            {
                return await _context.person.ToListAsync();
            }

            // GET: api/Person/5
            [HttpGet("{id}")]
            public async Task<ActionResult<Person>> GetPerson(int id)
            {
                var person = await _context.person.FindAsync(id);

                if (person == null)
                {
                    return NotFound();
                }

                return person;
            }

        [HttpPost]
        public async Task<ActionResult<PersonDto>> PostPerson(PersonDto personDto)
        {
            var person = new Person
            {
                Name = personDto.Name,
                Email = personDto.Email,
               
                Password = personDto.Password,
                PhoneNumber = personDto.PhoneNumber,
               
                // Password will be handled separately
            };

            _context.person.Add(person);
            await _context.SaveChangesAsync();

            personDto.Id = person.Id;

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, personDto);
        }

        // DELETE: api/Person/5
        [HttpDelete("{id}")]
            public async Task<IActionResult> DeletePerson(int id)
            {
                var person = await _context.person.FindAsync(id);
                if (person == null)
                {
                    return NotFound();
                }

                _context.person.Remove(person);
                await _context.SaveChangesAsync();

                return NoContent();
            }

            private bool PersonExists(int id)
            {
                return _context.person.Any(e => e.Id == id);
            }
        }
    }


