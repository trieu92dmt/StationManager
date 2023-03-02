namespace Core.Models
{
    public class DatatableViewModel
    {

        public int pageIndex { protected get; set; }
        public int pageSize { protected get; set; }
        public int offset
        {
            get
            {
                return pageIndex * pageSize - pageSize;
            }
        }
        public int limit
        {
            get
            {
                return pageSize;
            }
        }
        public List<ColumnViewModel> columns { get; set; }
        public SearchViewModel search { get; set; }
        public List<OrderViewModel> order { get; set; }
    }

    public class ColumnViewModel
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public SearchViewModel search { get; set; }
    }

    public class SearchViewModel
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class OrderViewModel
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
}
