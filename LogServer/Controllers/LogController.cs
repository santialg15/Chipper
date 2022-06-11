using LogServer.Exceptions;
using LogServer.ServiceAdapter.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LogServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    
    public class LogController : ControllerBase
    {
         private readonly ILogServiceAdapter _logServiceAdapter;

         public LogController(ILogServiceAdapter logServiceAdapter)
         {
             _logServiceAdapter = logServiceAdapter;
         }

        [HttpGet]
        [Route("GetLogByUser/{user}")]
        public IActionResult GetLogByUser(string user)
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
        [Route("GetLogByChipKey/{key}")]
        public IActionResult GetLogByChipKey(string key)
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
        [Route("GetLogByDate/{date}")]
        public IActionResult GetLogByDate(string date)
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
        [Route("GetLogByAction/{action}")]
        public IActionResult GetLogByAction(string action)
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
