using LogServer.Exceptions;
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
        [Route("User")]
        public IActionResult GetLogByUser([FromQuery]string user)
         {
            try
            {
                return new OkObjectResult(_logServiceAdapter.GetLogByUser(user)); // 200
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

        [HttpGet]
        [Route("Chip")]
        public IActionResult GetLogByChipKey([FromQuery] string key)
        {
            try
            {
                return new OkObjectResult(_logServiceAdapter.GetLogByChipKey(key)); // 200
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


        [HttpGet]
        [Route("Date")]
        public IActionResult GetLogByDate([FromQuery] string date)
        {
            try
            {
                return new OkObjectResult(_logServiceAdapter.GetLogByDate(date)); // 200
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


        [HttpGet]
        [Route("Action")]
        public IActionResult GetLogByAction([FromQuery] string action)
        {
            try
            {
                return new OkObjectResult(_logServiceAdapter.GetLogByAction(action)); // 200
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
