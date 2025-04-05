using Repository.GetDataFromToken;
using Repository.ViewModel;

namespace Service.Interfaces;

public interface ITaxService
{
    TaxListPaginationViewModel GetTaxList(int pageNumber = 1, int pageSize = 2, string searchKeyword = "");
    Task<AuthResponse> AddTax(AddTaxViewModel model);
    Task<AuthResponse> EditTax(AddTaxViewModel model);
    Task<AuthResponse> DeleteTax(int id);
}
