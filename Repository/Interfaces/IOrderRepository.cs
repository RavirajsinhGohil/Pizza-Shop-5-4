using Repository.Models;
using Repository.ViewModel;

namespace Repository.Interfaces;

public interface IOrderRepository
{
    Task<OrdersViewModel> GetOrderById(int orderId);
    Task<List<OrdersViewModel>> GetOrdersListModel();
    Task<OrdersListViewModel> GetOrderByPaginationAsync(string searchTerm, int page, int pageSize, string SortBy,  string SortOrder, string statusLog, string timeLog, string fromDate, string toDate);
    Task<OrdersListViewModel> GetOrdersForExport(string searchTerm, string statusLog, string timeLog);
    Task<CustomerViewModel> GetCustomerById(int customerId);
    Task<TableViewModel> GetTablesByOrderId (int tableId);
}
