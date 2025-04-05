namespace Repository.ViewModel;

public class OrdersViewModel
{
    public int Orderid { get; set; }
    public int Customerid { get; set; }
    public int tableId { get; set; }
    public string? CustomerName { get; set; }
    public string? Status { get; set; }
    public string? PaymentMode { get; set; }
    public decimal? Rating { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime Createdat { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

}
