using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.Data;
using Repository.Interfaces;
using Repository.ViewModel;

namespace Repository.Implementations;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _dbo;
    public CustomerRepository(ApplicationDbContext dbo)
    {
        _dbo = dbo;
    }
    public async Task<List<CustomerViewModel>> GetCustomersListModel()
    {
        return await _dbo.Customers
            .Where(c => !c.Isdeleted)
            .OrderBy(c => c.Customerid)
            .Select(c => new CustomerViewModel
            {
                Customerid = c.Customerid,
                Firstname = c.Firstname,
                Lastname = c.Lastname,
                Email = c.Email,
                Phone = c.Phone
            })
            .ToListAsync();
    }

    public async Task<CustomersListViewModel> GetCutomerByPaginationAsync(CustomerPaginationViewModel model)
    {
        var query = _dbo.Customers.Include(c => c.Orders).AsQueryable();
        // var query = _dbo.Orders.Include(o => o.Customer).Include(o => o.Payments).AsQueryable();

        if (!string.IsNullOrEmpty(model.SearchTerm))  
        {
            query = query.Where(customer => customer.Firstname.ToLower().Contains(model.SearchTerm.ToLower()));
        }

        // Apply Sorting using switch case
        query = model.SortBy switch
        {
            "Name" => model.SortOrder == "asc"
                ? query.OrderBy(c => c.Customerid)
                : query.OrderByDescending(u => u.Firstname),

            "Date" => model.SortOrder == "asc"
                ? query.OrderBy(u => u.Createdat)
                : query.OrderByDescending(u => u.Createdat),

            "Total Order" => model.SortOrder == "asc"
                ? query.OrderBy(u => u.Firstname)
                : query.OrderByDescending(u => u.Firstname),

            _ => query.OrderBy(u => u.Customerid)
        };

        if (!model.TimeLog.IsNullOrEmpty() && model.TimeLog != "All Time")
        {
            DateTime now = DateTime.Now;
            DateTime startDate = now;
            switch (model.TimeLog)
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
                // case "Custom Date":
                //     startDate = 
            }
            query = query.Where(o => o.Createdat >= startDate);
        }

        // Apply custom date filter
        // if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out DateTime from))
        // {
        //     query = query.Where(o => o.Createdat >= from);
        // }

        // if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out DateTime to))
        // {
        //     query = query.Where(o => o.Createdat <= to);
        // }

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)model.PageSize);

        var paginatedItems = await query
            .Skip((model.Page - 1) * model.PageSize)
            .Take(model.PageSize)
            .Select(customer => new CustomerViewModel
            {
                Customerid = customer.Customerid,
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
                Email = customer.Email,
                Phone = customer.Phone
            })
            .ToListAsync();

        return new CustomersListViewModel
        {
            Customers = paginatedItems,
            CurrentPage = model.Page,
            totalItems = totalItems,
            TotalPages = totalPages,
            PageSize = model.PageSize
        };
    }
}
