using Repository.ViewModel;

namespace Repository.Interfaces;

public interface ICustomerRepository
{
    Task<List<CustomerViewModel>> GetCustomersListModel();
    Task<CustomersListViewModel> GetCutomerByPaginationAsync(CustomerPaginationViewModel model);
}
