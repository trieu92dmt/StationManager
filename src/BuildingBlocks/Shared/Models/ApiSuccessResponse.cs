namespace Shared.Models
{
    public class ApiSuccessResponse<T>
    {
        public int Code { get; set; } = 200;
        public T Data { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; } = true;
        public int? ResultsCount { get; set; }
        public int? RecordsTotal { get; set; }
        public int? PagesCount { get; set; }
    }
}
