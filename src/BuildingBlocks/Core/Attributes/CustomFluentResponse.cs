﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;


namespace Core.Attributes
{
    public static class CustomFluentResponse
    {
        public static IActionResult FluentValidationResponse(ActionContext actionContext)
        {
            var responseObj = new FluentResponse();

            var msgData = actionContext.ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(m => m.Value.Errors.FirstOrDefault().ErrorMessage
                    )
                    .FirstOrDefault();

            responseObj.Message = msgData;

            var responseContext = new BadRequestObjectResult(responseObj)
            {
                StatusCode = (int)HttpStatusCode.OK,
            };
            responseContext.ContentTypes.Add("application/json");

            return responseContext;
        }
    }

    public class FluentResponse
    {
        public int Code { get; set; } = (int)HttpStatusCode.UnprocessableEntity;
        public string Message { get; set; }
        public bool IsSuccess { get; set; } = false;
    }
}
