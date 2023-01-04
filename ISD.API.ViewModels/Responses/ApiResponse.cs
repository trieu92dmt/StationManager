namespace ISD.API.ViewModels
{
    public class ApiResponse
    {
        public int Code { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string DeveloperMessage { get; set; }
        public object Data { get; set; }
        public object AdditionalData { get; set; }
    }

    public class ApiExResponse
    {
        public int code { get; set; }
        public bool isSuccess { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public object additionalData { get; set; }

    }
}
