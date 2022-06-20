using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServerAdmin.DTOs;

namespace ServerAdmin.Filters
{
    public class ExceptionFilter : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            int statusCode = 500;

            ResponseDTO response = new ResponseDTO()
            {
                Code = 2002,
                IsSuccess = false,
                ErrorMessage = context.Exception.Message
            };

            if (context.Exception is NullReferenceException)
            {
                statusCode = 404;
                response.Code = 2003;
            }

            if (context.Exception is ArgumentException)
            {
                statusCode = 400;
                response.Code = 2004;
            }

            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };
        }
    }
}
