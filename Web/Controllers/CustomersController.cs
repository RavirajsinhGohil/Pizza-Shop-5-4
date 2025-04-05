using Microsoft.AspNetCore.Mvc;
using Repository.ViewModel;
using Service.Interfaces;

namespace Web.Controllers;

public class CustomersController : Controller
{
    public readonly ICustomerService _customerService;
    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<IActionResult> Customers()
    {
        CustomersListViewModel viewModel = await _customerService.GetCustomersListModel();
        return View(viewModel);
    }

    // GetCustomerByPagination
    [HttpGet]
    public async Task<IActionResult> GetCustomerByPagination(CustomerPaginationViewModel viewModel)
    {
        
        CustomersListViewModel model = await _customerService.GetCutomerByPaginationAsync(viewModel);
        return PartialView("_OrderList", model);
    }
}
