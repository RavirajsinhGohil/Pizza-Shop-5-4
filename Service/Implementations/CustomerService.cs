using Repository.Implementations;
using Repository.Interfaces;
using Repository.ViewModel;
using Service.Interfaces;

namespace Service.Implementations;

public class CustomerService : ICustomerService
{

    private readonly ICustomerRepository _customerRepository;

    public CustomerService (ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomersListViewModel> GetCustomersListModel()
    {
        CustomersListViewModel customerList = new CustomersListViewModel();
        customerList.Customers = await _customerRepository.GetCustomersListModel();
        return customerList;
    }

    public async Task<CustomersListViewModel> GetCutomerByPaginationAsync(CustomerPaginationViewModel model)
    {
        return await _customerRepository.GetCutomerByPaginationAsync(model);
    }

}
