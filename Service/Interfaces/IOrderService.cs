using Repository.ViewModel;

namespace Service.Interfaces;

public interface IOrderService
{
    Task<OrdersListViewModel> GetOrdersListModel();
    Task<OrdersListViewModel> GetOrderByPaginationAsync (string searchTerm , int page, int pageSize, string SortBy,  string SortOrder, string statusLog, string timeLog, string fromDate, string toDate);
    Task<byte[]> ExportDataInExcel (string searchTerm, string statusLog, string timeLog);
    Task<OrderDetailViewModel> GetOrderDetail(int orderId);
}
