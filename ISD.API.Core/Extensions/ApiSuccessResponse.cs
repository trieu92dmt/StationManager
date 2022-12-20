using ISD.API.EntityModels.Models;
using System.Collections.Generic;

namespace ISD.API.Core.Extensions
{
    public class ApiSuccessResponse<T>
    {
        public int Code { get; set; } = 200;
        public T Data { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; } = true;
    }

    public class ApiFailResponse<T>
    {
        public int Code { get; set; } = 400;
        public T Data { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; } = false;
    }
}
