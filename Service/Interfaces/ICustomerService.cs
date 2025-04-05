using Repository.ViewModel;

namespace Service.Interfaces;

public interface ICustomerService
{
    Task<CustomersListViewModel> GetCustomersListModel();
    Task<CustomersListViewModel> GetCutomerByPaginationAsync(CustomerPaginationViewModel model);
    
}
