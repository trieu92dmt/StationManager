namespace EntityModels.Core
{
    public class ApiSuccessReponse<T>
    {
        public int StatusCode { get; set; } = 200;
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
