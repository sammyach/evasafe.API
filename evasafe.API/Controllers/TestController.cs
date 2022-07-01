using evasafe.API.data;
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
            string? rip = HttpContext.Connection?.RemoteIpAddress?.ToString(); //GetServerVariable("HTTP_X_FORWARDED_FOR");// .Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string? lip = HttpContext.Connection?.LocalIpAddress?.ToString();
            string? rip4 = HttpContext.Connection?.RemoteIpAddress?.MapToIPv4().ToString();

            HttpContext.Request.Headers.TryGetValue("LocalTime", out var localTime);


            //if (string.IsNullOrEmpty(ip))
            //{
            //    ip = HttpContext.GetServerVariable("REMOTE_ADDR"); //System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //}
            return Ok($"Hi, API iz app and running @ {DateTime.UtcNow.ToUniversalTime()} - remote: {rip} - remote4: {rip4} - local: {lip} - localTime: {localTime}");
        }

        [HttpGet]
        [Route("hi/db")]
        public async Task<IActionResult> PingDB()
        {
            try
            {
                var db = _context.Database;
                var mzg = db.CanConnect() ? "DB iz available and can be connected to." : "Cannot connect to DB";
                var data = await _context.EvAccountActionsTypes.FirstOrDefaultAsync();
                return Ok($"{DateTime.UtcNow}: {mzg} --- {data?.ActionType}");
            }
            catch (Exception e)
            {
                return BadRequest($"xxx - {e.Message}");
            }
            
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
