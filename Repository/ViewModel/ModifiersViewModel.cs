using Microsoft.AspNetCore.Http;

namespace Repository.ViewModel;

public class ModifiersViewModel
{
    public int ModifierId { get; set; }
    public int? ModifierGroupId { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public decimal? Rate { get; set; }
    public decimal? Quantity { get; set; }
    
    public string? Description { get; set; }

    public bool Isdeleted { get; set; }
    public List<int> Ids { get; set; }
}
