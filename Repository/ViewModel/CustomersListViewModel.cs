namespace Repository.ViewModel;

public class CustomersListViewModel
{
    public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
    public int CurrentPage { get; set; }
    public int totalItems { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; } 
    public string? SortOrder { get; set; }
}
