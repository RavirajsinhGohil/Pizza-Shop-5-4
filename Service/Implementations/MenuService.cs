using System.Threading.Tasks;
using Repository.Interfaces;
using Repository.Models;
using Repository.ViewModel;
using Service.Interfaces;

namespace Service.Implementations;

public class MenuService : IMenuService
{
    private IMenuRepository _menuRepository;
    private IUserService _userService;

    public MenuService(IMenuRepository menuRepository, IUserService userService)
    {
        _menuRepository = menuRepository;
        _userService = userService;
    }

    public async Task<MenuItemViewModel> GetMenuModel(int categoryId)
    {
        List<CategoryViewModel> categories = await _menuRepository.GetCategories();
        List<ItemViewModel> items = await _menuRepository.GetItems(categoryId);
        List<ModifierGroupViewModel> modifierGroups = await _menuRepository.GetModifierGroups();
        List<ModifiersViewModel> modifiers = await _menuRepository.GetModifiers(categoryId);

        return new MenuItemViewModel
        {
            Categories = categories,
            Items = items,
            ModifierGroups = modifierGroups,
            Modifiers = modifiers
        };
    }

    public bool AddCategory(CategoryViewModel model)
    {
        try
        {
            var category = new Menucategory
            {
                Menucategoryid = model.Categoryid,
                Categoryname = model.Name,
                Description = model.Description,
                Isdeleted = false
            };
            _menuRepository.AddCategory(category);
            return true;
        }
        catch
        {
            return false;
        }
    }



    public async Task<MenuItemViewModel> GetItemsByCategoryAsync(int categoryid, string searchTerm, int page, int pageSize)
    {
        return await _menuRepository.GetItemsByCategoryAsync(categoryid, searchTerm, page, pageSize);
    }

    public async Task<MenuItemViewModel> GetModifierItemsByModifierGroupAsync(int categoryid, string searchTerm, int page, int pageSize)
    {
        return await _menuRepository.GetModifierItemsByModifierGroupAsync(categoryid, searchTerm, page, pageSize);
    }



    public Menucategory GetCategoryForEdit(int categoryId)
    {
        return _menuRepository.GetCategoryForEdit(categoryId);
    }

    public async Task<bool> EditCategory(CategoryViewModel model)
    {
        return await _menuRepository.EditCategory(model);
    }

    public async Task<bool> DeleteCategory(int categoryId)
    {
        return await _menuRepository.DeleteCategory(categoryId);
    }

    public async Task<bool> AddItem(ItemViewModel model)
    {
        Item menuItem = new Item
        {
            Categoryid = model.CategoryId,
            Itemname = model.Name,
            Itemtype = model.Itemtype,
            Rate = model.Rate,
            Quantity = model.Quantity,
            Unit = model.Unit,
            Available = model.Isavailable,
            Tax = model.Tax,
            Itemshortcode = model.ItemShortCode,
            Description = model.Description,
            // Itemimage = _userService.(model.ItemPhoto),
            Isdeleted = false,
            Createdat = DateTime.Now,
        };
        await _menuRepository.AddItem(menuItem);

        int totalItems = await _menuRepository.GetTotalCountOfItems();

        // foreach (var modifierGroupId in model.ModifierGroupIds)
        // {
        //     var modifierMapping = new Itemmodifiergroupmapping
        //     {
        //         Itemid = totalItems,
        //         Modifiergroupid = modifierGroupId
        //     };
        //     await _menuRepository.AddItemModifierGroupMappings(modifierMapping);
        // }
        return true;
    }


    public async Task<MenuItemViewModel> GetMenuItemForEdit(int id)
    {
        var item = _menuRepository.GetItemById(id);
        if (item == null)
        {
            return null;
        }

        var categories = _menuRepository.GetAllCategories()
                        .Select(c => new CategoryViewModel
                        {
                            Categoryid = c.Menucategoryid,
                            Name = c.Categoryname
                        }).ToList();
        
        Console.WriteLine("123456789");

        // var ItemModifierGroup = await _menuRepository.GetModifierGroupsForEditItem(id);
        // var modifierGroupIds = ItemModifierGroup.Where(i => i.Itemid == id).Select(i => i.Modifiergroupid).ToList();

        return new MenuItemViewModel
            {
                Items = new List<ItemViewModel>
                {
                    new ItemViewModel
                    {
                        Itemid = item.Itemid,
                        Name = item.Itemname,
                        CategoryId = item.Categoryid,
                        Itemtype = item.Itemtype,
                        Rate = item.Rate,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        Isavailable = true,
                        Tax = item.Tax,
                        ItemShortCode = item.Itemshortcode,
                        Description = item.Description,
                        Itemimage = item.Itemimage,
                        Isdeleted = item.Isdeleted,
                        // ModifierGroupIds = modifierGroupIds
                    }
                },
                Categories = categories
            };

        // try
        // {
        //     return new MenuItemViewModel
        //     {
        //         Items = new List<ItemViewModel>
        //         {
        //             new ItemViewModel
        //             {
        //                 Itemid = item.Itemid,
        //                 Name = item.Itemname,
        //                 CategoryId = item.Categoryid,
        //                 Itemtype = item.Itemtype,
        //                 Rate = item.Rate,
        //                 Quantity = item.Quantity,
        //                 Unit = item.Unit,
        //                 // Isavailable = item.Available,
        //                 Tax = item.Tax,
        //                 ItemShortCode = item.Itemshortcode,
        //                 Description = item.Description,
        //                 Itemimage = item.Itemimage,
        //                 // Isdeleted = item.Isdeleted,
        //                 ModifierGroupIds = await _menuRepository.GetModifierGroupsForEditItem(id)
        //             }
        //         },
        //         Categories = categories
        //     };
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine("Exception123 ", ex);
        //     return null;
        // }
    }

    public bool UpdatedMenuItem(int id, MenuItemViewModel model)
    {
        Item item = _menuRepository.GetItemById(id);
        if (item == null)
        {
            return false;
        }

        var updatedItem = model.Items.FirstOrDefault();
        if (updatedItem != null)
        {
            item.Itemname = updatedItem.Name;
            item.Categoryid = updatedItem.CategoryId;
            item.Itemtype = updatedItem.Itemtype;
            item.Rate = updatedItem.Rate;
            item.Quantity = updatedItem.Quantity;
            item.Unit = updatedItem.Unit;
            item.Available = updatedItem.Isavailable;
            item.Tax = updatedItem.Tax;
            item.Itemshortcode = updatedItem.ItemShortCode;
            item.Description = updatedItem.Description;
            item.Itemimage = updatedItem.Itemimage;
        }
        _menuRepository.UpdateItem(item);
        return true;
    }

    public bool DeleteItem(int itemId)
    {
        return _menuRepository.DeleteItem(itemId);
    }

    public async Task<bool> AddModifierGroup(string name, string description, List<int> selectedModifiers)
    {
        try
        {
            Modifiergroup modifiergroup = new Modifiergroup
            {
                // Modifiergroupid = model.ModifierGroupId,
                Modifiername = name,
                Description = description,
                Isdeleted = false
            };
            await _menuRepository.AddModifierGroup(modifiergroup);

            Modifiergroup modifiergroup1 = modifiergroup;
            int modifierMappingCount = _menuRepository.GetTotalCountOfModifierMapping();
            // modifierMappingCount = modifierMappingCount + 1;

            for (int i = 0; i < selectedModifiers.Count; i++)
            {
                int modifierId = selectedModifiers[i];
                Item item = _menuRepository.GetItemById(modifierId);
                ModifiersViewModel modifier = new ModifiersViewModel()
                {
                    ModifierId = item.Itemid,
                    ModifierGroupId = item.Categoryid,
                    Name = item.Itemname,
                    Unit = item.Unit,
                    Rate = item.Rate,
                    Quantity = item.Quantity
                };
                var modifierMapping = new Itemmodifiergroupmapping
                {
                    Itemid = modifier.ModifierId,
                    Modifiergroupid = modifierMappingCount
                };

                bool isAdded = await _menuRepository.AddItemModifierGroupMappings(modifierMapping);
            }

            // foreach(var modifierId in selectedModifiers)
            // {
            //     Itemmodifiergroupmapping mapping = new Itemmodifiergroupmapping
            //     {
            //         Itemid = modifierId,
            //         Modifiergroupid = modifierMappingCount
            //     };
            //     await _menuRepository.AddItemModifierGroupMappings(mapping);
            // }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("145236987");
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<ModifierGroupViewModel> GetModifierGroupForEdit(int id)
    {
        Modifiergroup modifierGroup = _menuRepository.GetModifierGroupById(id);
        List<ModifiersViewModel> existingModifiers = await _menuRepository.GetExistingModifiersForEdit(id);
        ModifierGroupViewModel modifierGroupViewModel = new ModifierGroupViewModel
        {
            ModifierGroupId = modifierGroup.Modifiergroupid,
            modifierGroupName = modifierGroup.Modifiername,
            modifierGroupDescription = modifierGroup.Description,
            ExistingModifiers = existingModifiers
        };
        return modifierGroupViewModel;
    }

    // public bool UpdateModifierGroup(ModifierGroupViewModel model)
    // {
    //     Modifiergroup modifiergroup = new Modifiergroup
    //     {
    //         Modifiergroupid = model.ModifierGroupId,
    //         Modifiername = model.modifierGroupName,
    //         Description = model.modifierGroupDescription,
    //         Isdeleted = false
    //     };
    //     return _menuRepository.UpdateModifierGroup(modifiergroup);
    // }

    public bool DeleteModifierGroup(int modifierGroupId)
    {

        Modifiergroup modifierGroup = _menuRepository.GetModifierGroupById(modifierGroupId);
        modifierGroup.Isdeleted = true;
        return _menuRepository.DeleteModifierGroup(modifierGroup);
    }

    public async Task<bool> AddModifierinGroups(ModifiersViewModel model)
    {
        foreach (var modifierGroupId in model.Ids)
        {
            var menuItem = new Item
            {
                Categoryid = modifierGroupId,
                Itemname = model.Name,
                Rate = model.Rate,
                Quantity = model.Quantity,
                Unit = model.Unit,
                Description = model.Description,
                Available = true,
                Ismodifiable = true,
                Isdeleted = false,
                Createdat = DateTime.Now,
                // Createdby = 
            };
            _menuRepository.AddItem(menuItem);

            var modifierMapping = new Itemmodifiergroupmapping
            {
                Itemid = menuItem.Itemid,
                Modifiergroupid = modifierGroupId
            };

            _menuRepository.AddItemModifierGroupMappings(modifierMapping);
        }
        return true;
    }

    public async Task<bool> addExistingModifiersForEdit(int modifierGroupId, string name, string description, List<int> selectedModifiers)
    {
        Modifiergroup modifierGroup = new Modifiergroup
        {
            Modifiergroupid = modifierGroupId,
            Modifiername = name,
            Description = description
        };

        _menuRepository.UpdateModifierGroup(modifierGroup);

        for (int i = 0; i < selectedModifiers.Count; i++)
        {
            var modifierId = selectedModifiers[i];
            var mapping = _menuRepository.GetItemModifierGroupMappingsById(modifierId, modifierGroupId);
            if (mapping == null)
            {
                Item item = _menuRepository.GetItemById(modifierId);
                ModifiersViewModel modifier = new ModifiersViewModel()
                {
                    ModifierId = item.Itemid,
                    ModifierGroupId = item.Categoryid,
                    Name = item.Itemname,
                    Unit = item.Unit,
                    Rate = item.Rate,
                    Quantity = item.Quantity
                };
                var modifierMapping = new Itemmodifiergroupmapping
                {
                    Itemid = modifier.ModifierId,
                    Modifiergroupid = modifierGroupId
                };

                bool isAdded = await _menuRepository.AddItemModifierGroupMappings(modifierMapping);
            }
        }
        return true;
    }

    public async Task<MenuItemViewModel> GetDataForEditModifier(int id)
    {
        Item item = _menuRepository.GetItemById(id);

        List<ModifierGroupViewModel> modifierGroups = await _menuRepository.GetModifierGroupsForEditModifier();

        MenuItemViewModel viewModel = new MenuItemViewModel
        {
            Modifiers = new List<ModifiersViewModel>
            {
                new ModifiersViewModel
                {
                    ModifierId = item.Itemid,
                    Name = item.Itemname,
                    ModifierGroupId = item.Categoryid,
                    Rate = item.Rate,
                    Quantity = item.Quantity,
                    Unit = item.Unit,
                    Description = item.Description,
                    Isdeleted = item.Isdeleted,
                }
            },
            ModifierGroups = modifierGroups
        };
        return viewModel;
    }

    public async Task<bool> UpdateModifier(MenuItemViewModel model)
    {
        // var modifierGroup = _db.Modifiergroups
        //                    .Select(m => new ModifierGroupViewModel
        //                    {
        //                        ModifierGroupId = m.Modifiergroupid,
        //                        modifierGroupName = m.Modifiername
        //                    }).ToList();
        // model.ModifierGroups = modifierGroup;
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(ModelState);
        // }
        // var modifier = _db.Items.FirstOrDefault(c => c.Itemid == model.Modifiers.FirstOrDefault().ModifierId);
        // if (modifier == null)
        // {
        //     return NotFound();
        // }
        // modifier.Itemname = model.Modifiers.FirstOrDefault()?.Name;
        // modifier.Categoryid = model.Modifiers.FirstOrDefault()?.ModifierGroupId ?? modifier.Categoryid;
        // modifier.Rate = model.Modifiers.FirstOrDefault()?.Rate ?? modifier.Rate;
        // modifier.Quantity = model.Modifiers.FirstOrDefault()?.Quantity ?? modifier.Quantity;
        // modifier.Unit = model.Modifiers.FirstOrDefault()?.Unit;
        // modifier.Description = model.Modifiers.FirstOrDefault()?.Description;

        // _db.Items.Update(modifier);
        // _db.SaveChanges();
        return true;
    }

    // MenuItemViewModel IMenuService.GetMenuItemForEdit(int id)
    // {
    //     throw new NotImplementedException();
    // }

}
