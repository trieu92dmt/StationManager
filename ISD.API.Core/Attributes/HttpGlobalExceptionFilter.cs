using ISD.API.Core.Exceptions;
using ISD.API.Resources;
using ISD.API.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;

namespace ISD.API.Core.Attributes
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        public HttpGlobalExceptionFilter(IWebHostEnvironment env)
        {
            _env = env;
        }
        /// <summary>
        /// System exception
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var developerMessage = exception.Message + "\r\n" + exception.StackTrace;
            while (exception.InnerException != null)
            {
                developerMessage += "\r\n--------------------------------------------------\r\n";
                exception = exception.InnerException;
                developerMessage += (exception.Message + "\r\n" + exception.StackTrace);
            }

            if (context.ModelState.ErrorCount > 0)
            {
                var errors = context.ModelState.Where(v => v.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => $"{char.ToLower(kvp.Key[0])}{kvp.Key.Substring(1)}",
                        kvp => kvp.Value.Errors.FirstOrDefault()?.ErrorMessage
                    );

                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                context.Result = new UnprocessableEntityObjectResult(new ApiResponse
                {
                    Code = StatusCodes.Status422UnprocessableEntity,
                    //Message = developerMessage,
                    Data = errors
                });
                context.ExceptionHandled = true;
                return;
            }

            var json = new ApiResponse
            {
                Message = context.Exception.Message
            };

            json.DeveloperMessage = developerMessage;

            var userName = context.HttpContext.User.Identity.IsAuthenticated
                ? context.HttpContext.User.Identity.Name : "Guest"; //Gets user Name from user Identity 

            // 400 Bad Request
            if (context.Exception.GetType() == typeof(ISDException))
            {
                var errorCode = (int?)exception.Data[ISDException.ErrorCode];
                if (errorCode != null)
                {
                    json.Code = errorCode.Value;
                    context.HttpContext.Response.StatusCode = errorCode.Value;
                }
                else
                {
                    json.Code = StatusCodes.Status400BadRequest;
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    json.IsSuccess = false;
                }
                context.Result = new BadRequestObjectResult(json);
            }
            // 404 Not Found
            else if (context.Exception.GetType() == typeof(NotFoundException))
            {
                json.Code = StatusCodes.Status404NotFound;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Result = new NotFoundObjectResult(json);
                json.IsSuccess = false;

            }
            // 500 Internal Server Error
            else
            {
                json.IsSuccess = false;
                json.Message = CommonResource.MSG_SYSTEM_ERROR;
                json.Code = StatusCodes.Status500InternalServerError;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Result = new InternalServerErrorObjectResult(json);
            }
            context.ExceptionHandled = true;
        }
    }
}
