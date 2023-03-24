namespace Shared.Common
{
    public class CommonResponse
    {
        public string Key { get; set; }
        public string Value { get; set; }

    }
    public class Common2Response
    {
        public Guid Key { get; set; }
        public string Value { get; set; }

    }

    public class Common3Response
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
    }

    public class CommonResponse<T>
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public T Data { get; set; }
    }

    public class Common2Response<T>
    {
        public Guid Key { get; set; }
        public string Value { get; set; }
        public T Data { get; set; }
    }
}
