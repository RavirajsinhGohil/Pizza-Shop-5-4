namespace Repository.ViewModel;

public class TaxViewModel
{
    public int TaxId { get; set; }
    public string TaxName { get; set; } = null!;
    public string Type { get; set; } = null!;
    public decimal TaxAmount { get; set; }
    public bool? Isenable { get; set; }
    public bool? Isdefault { get; set; }
}