using LogServer.Exceptions;
using LogServer.Models;
using LogServer.ServiceAdapter.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LogServer.Controllers
{
    [Route("logs")]
    [ApiController]
    
    public class LogController : ControllerBase
    {
         private readonly ILogServiceAdapter _logServiceAdapter;

         public LogController(ILogServiceAdapter logServiceAdapter)
         {
             _logServiceAdapter = logServiceAdapter;
         }

        [HttpGet]
        public IActionResult GetLogByParameters([FromQuery] Parametros parametros)
        {
            try
            {
                return new OkObjectResult(_logServiceAdapter.GetLogByParameters(parametros)); // 200
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message); // 400
            }
            catch (LogDoesNotExistException e)
            {
                return NotFound(e.Message); // 404
            }
        }
    }
}
