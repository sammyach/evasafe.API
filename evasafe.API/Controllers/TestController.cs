using evasafe.API.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace evasafe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly EVASAFEDBContext _context;

        public TestController(EVASAFEDBContext context)
        {
            _context = context;  
        }
        [HttpGet]
        [Route("hi")]
        public IActionResult Hi()
        {
            return Ok($"Hi, API iz app and running @ {DateTime.UtcNow}");
        }

        [HttpGet]
        [Route("hi/db")]
        public async Task<IActionResult> PingDB()
        {
            var db = _context.Database;
            var mzg = db.CanConnect() ? "DB iz available and can be connected to." : "Cannot connect to DB";
            var data = await _context.EvAccountActionsTypes.FirstOrDefaultAsync();
            return Ok($"{DateTime.UtcNow}: {mzg} --- {data?.ActionType}");
        }

        [HttpPost]
        [Route("hello/db")]
        public async Task<IActionResult> PingDBPost(string data)
        {
            var db = _context.Database;
            var mzg = db.CanConnect() ? "DB iz available and can be connected to." : "Cannot connect to DB";
            
            return Ok($"{DateTime.UtcNow}: {mzg} -- {data}");
        }

    }
}
