using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ISD.Core.Exceptions
{
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error)
            : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
