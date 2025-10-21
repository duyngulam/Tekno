namespace Tekno.Application.Common.Paging
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public int TotalPages => (int)Math.Ceiling((decimal)TotalRecords / PageSize);

        public PagedResult() { }

        public PagedResult(IEnumerable<T> data, int totalRecords, int page, int pageSize)
        {
            Data = data;
            TotalRecords = totalRecords;
            Page = page;
            PageSize = pageSize;
        }
    }
}
