using Microsoft.AspNetCore.Mvc;
using Repository.ViewModel;
using Service.Interfaces;

namespace Web.Controllers;

public class OrdersController : Controller
{
    public readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Orders()
    {
        // ViewBag["ActivePage"] = "Orders";
        OrdersListViewModel viewModel = await _orderService.GetOrdersListModel();
        return View(viewModel);
    }

    // GetOrderByPagination
    [HttpGet]
    public async Task<IActionResult> GetOrderByPagination(string searchTerm = "", int page = 1, int pageSize = 2, string SortBy = "Orderid", string SortOrder = "asc", string statusLog = "All Status", string timeLog="All Time", string fromDate = "", string toDate = "")
    {
        OrdersListViewModel model = await _orderService.GetOrderByPaginationAsync(searchTerm, page, pageSize, SortBy, SortOrder, statusLog, timeLog, fromDate, toDate);
        return PartialView("_OrderList", model);
    }

    [HttpGet]
    public async Task<IActionResult> ExportOrdersInExcel(string searchTerm, string statusLog, string timeLog)
    {
        byte[] model = await _orderService.ExportDataInExcel(searchTerm, statusLog, timeLog);
        string fileName = $"Orders_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        if (model == null || model.Length == 0)
        {
            return Json("Failed to generate Excel file.");
        }
        return File(model,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
    }
    
    public async Task<IActionResult> OrderDetails(int orderId)
    {
        OrderDetailViewModel viewModel = await _orderService.GetOrderDetail(orderId);
        return View(viewModel);
    }

    
    
}
