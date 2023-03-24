namespace Shared.Common
{
    public class PagingResponse
    {
        public int FilterResultsCount { get; set; }
        public int TotalResultsCount { get; set; }
        public int TotalPagesCount { get; set; }
    }
}
