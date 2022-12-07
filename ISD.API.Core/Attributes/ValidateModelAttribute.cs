using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Core.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Filter action executing.
        /// </summary>
        /// <param name="actionContext">context</param>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var valid = ExecutingAsync(actionContext).Result;
            if (valid)
                base.OnActionExecuting(actionContext);
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var valid = await ExecutingAsync(context);
            if (valid)
                await base.OnActionExecutionAsync(context, next);
        }

        private async Task<bool> ExecutingAsync(ActionExecutingContext actionContext)
        {
            if (!actionContext.ModelState.IsValid || actionContext.ActionArguments.Any(kv => kv.Value == null))
            {
                var msgData = actionContext.ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(m => new
                    {
                        key = m.Key,
                        value = m.Value.Errors.FirstOrDefault().ErrorMessage
                    })
                    .ToList();

                var response = new
                {
                    code = (int)HttpStatusCode.UnprocessableEntity,
                    message = JsonConvert.SerializeObject(msgData)
                };

                var responseContext = actionContext.HttpContext.Response;
                responseContext.StatusCode = (int)HttpStatusCode.OK;
                responseContext.ContentType = "application/json";

                var responseBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
                await responseContext.Body.WriteAsync(responseBody);

                return false;
            }

            return true;
        }
    }
}
