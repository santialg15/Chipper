using LogServer.Exceptions;
using LogServer.ServiceAdapter.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

        /*
         * GetLogByUser
         * GetLogByChipKey
         * GetLogByDate
         * GetLogByAction
         */

        [HttpGet("GetLogByUser/{user}")]
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

        [HttpGet("GetLogByChipKey/{Key}")]
        //[Route("{key}", Name = "GetLogByChipKey")]
        public IActionResult GetLogByChipKey(string Key)
        {
            try
            {
                return new OkObjectResult(_logServiceAdapter.GetLogByChipKey(Key)); // 200
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


        [HttpGet("GetLogByDate/{date}")]
        //[Route("{date}", Name = "GetLogByDate")]
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


        [HttpGet("GetLogByAction/{action}")]
        //[Route("{action}", Name = "GetLogByAction")]
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
