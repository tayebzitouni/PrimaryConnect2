using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimaryConnect.Data;

namespace PrimaryConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
       public StudentsController( AppDbContext appDbContext) 
        {
            _PrimaryConnect_Db = appDbContext;
        }
        private AppDbContext _PrimaryConnect_Db;






        [HttpGet]
        public IActionResult GetStudentsList(int SchoolId)
        {

            return Ok();
        }

          }
    
}
