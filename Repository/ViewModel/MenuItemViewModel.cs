using Repository.Models;

namespace Repository.ViewModel;

public class MenuItemViewModel
{
    public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    public List<ItemViewModel> Items { get; set; } = new List<ItemViewModel>();
    public List<ModifierGroupViewModel> ModifierGroups { get; set; } = new List<ModifierGroupViewModel>();

    public List<ModifiersViewModel> Modifiers { get; set; } = new List<ModifiersViewModel>();
    public int CurrentPage { get; set; }
    public int totalItems { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int SelectedCategoryId { get; set; } 
    // public IFormFile ItemPhoto { get; set; }
    public List<int> SelectedModifiers { get; set; } = new List<int>();
    
}
