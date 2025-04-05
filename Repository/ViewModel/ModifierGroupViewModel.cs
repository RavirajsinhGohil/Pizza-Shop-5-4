using Repository.Models;

namespace Repository.ViewModel;

public class ModifierGroupViewModel
{
    public int ModifierGroupId { get; set; }
    public string? modifierGroupName { get; set; }
    public string? modifierGroupDescription { get; set; }
    public List<ModifiersViewModel> ExistingModifiers { get; set; }
}
