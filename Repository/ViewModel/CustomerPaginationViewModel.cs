namespace Repository.ViewModel;

public class CustomerPaginationViewModel
{
    // string searchTerm = "", int page = 1, int pageSize = 2, string SortBy = "CustomerId", string SortOrder = "asc", string timeLog="All Time"
    public string SearchTerm { get; set; } = "";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 2;
    public string? SortBy { get; set; } = "CustomerId";
    public string? SortOrder { get; set; } = "asc";
    public string? TimeLog { get; set; } ="All Time";
    public DateTime? CustomDate { get; set; }

}
