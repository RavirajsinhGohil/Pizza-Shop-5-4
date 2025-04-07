using Repository.Models;
using Repository.ViewModel;

namespace Service.Interfaces;

public interface IMenuService
{
    Task<MenuItemViewModel> GetMenuModel(int categoryId);
    bool AddCategory(CategoryViewModel model);
    Task<MenuItemViewModel> GetItemsByCategoryAsync(int categoryid, string searchTerm, int page, int pageSize);
    Task<MenuItemViewModel> GetModifierItemsByModifierGroupAsync(int categoryid, string searchTerm, int page, int pageSize);
    Menucategory GetCategoryForEdit(int categoryId);
    Task<bool> EditCategory(CategoryViewModel model);
    Task<bool> DeleteCategory(int categoryId);
    Task<bool> AddItem(ItemViewModel model);
    Task<MenuItemViewModel> GetMenuItemForEdit(int id);
    bool UpdatedMenuItem(int id, MenuItemViewModel model);
    bool DeleteItem(int itemId);
    Task<bool> AddModifierGroup(string name, string description, List<int> selectedModifiers);
    // bool UpdateModifierGroup(ModifierGroupViewModel model);
    bool DeleteModifierGroup(int modifierGroupId);
    Task<bool> AddModifierinGroups(ModifiersViewModel model);
    Task<bool> addExistingModifiersForEdit(int modifierGroupId, string name, string description, List<int> selectedModifiers);
    Task<ModifierGroupViewModel> GetModifierGroupForEdit(int id);
    Task<MenuItemViewModel> GetDataForEditModifier(int id);
    Task<bool> UpdateModifier(MenuItemViewModel model);

}
