namespace Repository.ViewModel;

public class TaxListPaginationViewModel
{
    public IEnumerable<TaxViewModel>? Items { get; set; }
    public Pagination? Page { get; set; }

}