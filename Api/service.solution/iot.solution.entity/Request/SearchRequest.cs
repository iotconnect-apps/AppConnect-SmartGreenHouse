namespace iot.solution.entity
{
    public class BaseRequest
    {
        public string Guid { get; set; }
        public string CompanyId { get; set; }
        public System.Guid GreenHouseId { get; set; }
        public string Version { get; set; } = "v1";
        
    }
    public class SearchRequest : BaseRequest
    {
        public System.Guid InvokingUser { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
        public string SearchText { get; set; }
    }

    public class SearchResult<T>
    {
        public T Items { get; set; }
        public int Count { get; set; }
    }
}
