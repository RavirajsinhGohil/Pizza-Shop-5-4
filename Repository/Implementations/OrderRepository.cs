using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.Data;
using Repository.Interfaces;
using Repository.Models;
using Repository.ViewModel;

namespace Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _dbo;
    public OrderRepository(ApplicationDbContext dbo)
    {
        _dbo = dbo;
    }

    public async Task<OrdersViewModel> GetOrderById(int orderId)
    {
        return await _dbo.Orders
                .Where(o => o.Orderid == orderId)
                .Select(o => new OrdersViewModel
                {
                    Orderid = o.Orderid,
                    Customerid = o.Customerid,
                    tableId = o.Tableid,
                    CustomerName = o.Customer.Firstname,
                    Status = o.Status,
                    PaymentMode = o.Paymentmode,
                    TotalAmount = o.Totalamount,
                    Createdat = o.Createdat,
                    CreatedBy = o.Createdby.ToString(),
                    UpdatedAt = o.Updatedat,
                    UpdatedBy = o.Updatedby.ToString()
                }).FirstAsync();
    }


    // Move it to the customerRepository
    public async Task<CustomerViewModel> GetCustomerById(int customerId)
    {
        return await _dbo.Customers
                        .Where(c => c.Customerid == customerId)
                        .Select(c => new CustomerViewModel 
                        {
                            Customerid = c.Customerid,
                            Firstname = c.Firstname,
                            Lastname = c.Lastname,
                            Email = c.Email,
                            Phone = c.Phone
                        }).FirstAsync();
    } 

    public async Task<List<OrdersViewModel>> GetOrdersListModel()
    {
        return await _dbo.Orders
            .Where(o => !o.Isdeleted)
            .OrderBy(o => o.Orderid)
            .Select(o => new OrdersViewModel
            {
                Orderid = o.Orderid,
                Customerid = o.Customerid,
                CustomerName = _dbo.Customers.FirstOrDefault(c => c.Customerid == o.Customerid).Firstname,
                Status = o.Status,
                PaymentMode = o.Paymentmode,
                Rating = o.Avgrating,
                TotalAmount = o.Orderid,
                Createdat = o.Createdat
            })
            .ToListAsync();
    }

    public async Task<OrdersListViewModel> GetOrderByPaginationAsync(string searchTerm, int page, int pageSize, string SortBy, string SortOrder, string statusLog, string timeLog, string fromDate, string toDate)
    {
        var query = _dbo.Orders.Include(o => o.Customer).AsQueryable();
        // var query = _dbo.Orders.Include(o => o.Customer).Include(o => o.Payments).AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))  
        {
            query = query.Where(order => order.Orderid.ToString().ToLower().Contains(searchTerm.ToLower()) || order.Customer.Firstname.ToLower().Contains(searchTerm.ToLower()));
        }

        if (!statusLog.IsNullOrEmpty() && statusLog != "All Status")
        {
            query = query.Where(o => o.Status == statusLog);
        }

        // Apply Sorting using switch case
        query = SortBy switch
        {
            "OrderId" => SortOrder == "asc"
                ? query.OrderBy(u => u.Orderid)
                : query.OrderByDescending(u => u.Orderid),

            "CreatedAt" => SortOrder == "asc"
                ? query.OrderBy(u => u.Createdat)
                : query.OrderByDescending(u => u.Createdat),

            "CustomerName" => SortOrder == "asc"
                ? query.OrderBy(u => u.Customer.Firstname)
                : query.OrderByDescending(u => u.Customer.Firstname),

            "TotalAmount" => SortOrder == "asc"
                ? query.OrderBy(u => u.Totalamount)
                : query.OrderByDescending(u => u.Totalamount),

            _ => query.OrderBy(u => u.Orderid)
        };

        if (!timeLog.IsNullOrEmpty() && timeLog != "All Time")
        {
            DateTime now = DateTime.Now;
            DateTime startDate = now;
            switch (timeLog)
            {
                case "Last 7 days":
                    startDate = now.AddDays(-7);
                    break;
                case "Last 30 days":
                    startDate = now.AddDays(-30);
                    break;
                case "Current Month":
                    startDate = new DateTime(now.Year, now.Month, 1);
                    break;
            }

            query = query.Where(o => o.Createdat >= startDate);
        }

        // Apply custom date filter
        if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out DateTime from))
        {
            query = query.Where(o => o.Createdat >= from);
        }

        if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out DateTime to))
        {
            query = query.Where(o => o.Createdat <= to);
        }

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var paginatedItems = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(order => new OrdersViewModel
            {
                Orderid = order.Orderid,
                Customerid = order.Customerid,
                CustomerName = _dbo.Customers.FirstOrDefault(c => c.Customerid == order.Customerid).Firstname,
                Status = order.Status,
                PaymentMode = order.Paymentmode,
                Rating = order.Avgrating,
                TotalAmount = order.Totalamount,
                Createdat = order.Createdat
            })
            .ToListAsync();

        return new OrdersListViewModel
        {
            orders = paginatedItems,
            CurrentPage = page,
            totalItems = totalItems,
            TotalPages = totalPages,
            PageSize = pageSize
        };


    }    

    public async Task<OrdersListViewModel> GetOrdersForExport(string searchTerm, string statusLog, string timeLog)
    {
        
        var query = _dbo.Orders.Include(o => o.Customer).AsQueryable();
        // var query = _dbo.Orders.Include(o => o.Customer).Include(o => o.Payments).AsQueryable();
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(order => order.Orderid.ToString().ToLower().Contains(searchTerm.ToLower()));
            //On customer Name search is remaining
        }

        if (!statusLog.IsNullOrEmpty() && statusLog != "All Status")
        {
            query = query.Where(o => o.Status == statusLog);
        }

        if (!timeLog.IsNullOrEmpty() && timeLog != "All Time")
        {
            DateTime now = DateTime.Now;
            DateTime startDate = now;
            switch (timeLog)
            {
                case "Last 7 days":
                    startDate = now.AddDays(-7);
                    break;
                case "Last 30 days":
                    startDate = now.AddDays(-30);
                    break;
                case "Current Month":
                    startDate = new DateTime(now.Year, now.Month, 1);
                    break;
            }

            query = query.Where(o => o.Createdat >= startDate);
        }

        int totalItems = await query.CountAsync();

        var paginatedItems = await query
            .Take(totalItems)
            .Select(order => new OrdersViewModel
            {
                Orderid = order.Orderid,
                Customerid = order.Customerid,
                CustomerName = _dbo.Customers.FirstOrDefault(c => c.Customerid == order.Customerid).Firstname,
                Status = order.Status,
                PaymentMode = order.Paymentmode,
                Rating = order.Avgrating,
                TotalAmount = order.Totalamount,
                Createdat = order.Createdat
            })
            .OrderBy(o => o.Orderid)
            .ToListAsync();

        return new OrdersListViewModel
        {
            orders = paginatedItems
        };
    }

    public async Task<TableViewModel> GetTablesByOrderId (int tableId)
    {
        return await _dbo.Orders.Include(o => o.Table)
                                .Where(o => o.Tableid == tableId)
                                .Select(o => new TableViewModel
                                {
                                    TableId = o.Tableid,
                                    TableName = o.Table.Tablename,
                                    SectionId = o.Table.Section.Sectionid,
                                    SectionName = o.Table.Section.Sectionname,
                                    Name = o.Table.Tablename,
                                    Capacity = o.Table.Capacity,
                                    Status = o.Table.Status
                                }).FirstAsync();
    }
}




