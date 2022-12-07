using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace ISD.API.Core.SeedWork
{
    public class ApiSuccessResult<T> : ApiResult<T>
    {
        [DataMember]
        public T Data { get; set; }
        public bool IsSucces { get; set; }

        public ApiSuccessResult()
        {
            Code = StatusCodes.Status200OK;
        }

        public ApiSuccessResult(T data)
        {
            Data = data;
            Code = StatusCodes.Status200OK;
            IsSucces = true;
        }

        public ApiSuccessResult(T data, string message)
        {
            Data = data;
            Code = StatusCodes.Status200OK;
            Message = message;
            IsSucces = true;
        }

        public ApiSuccessResult(T data, PagingSP paging)
        {
            Data = data;
            Paging = paging;
            Code = StatusCodes.Status200OK;
            IsSucces = true;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PagingSP Paging { get; set; }
    }
}
