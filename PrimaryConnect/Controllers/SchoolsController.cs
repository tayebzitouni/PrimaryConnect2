using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimaryConnect.Data;

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


    }
}
