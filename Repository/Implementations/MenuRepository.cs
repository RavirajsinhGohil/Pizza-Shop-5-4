using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Interfaces;
using Repository.Models;
using Repository.ViewModel;

namespace Repository.Implementations;

public class MenuRepository : IMenuRepository
{
    private readonly ApplicationDbContext _dbo;
    public MenuRepository(ApplicationDbContext dbo)
    {
        _dbo = dbo;
    }

    public async Task<List<CategoryViewModel>> GetCategories()
    {
        return await _dbo.Menucategories
            .Where(c => !c.Isdeleted)
            .OrderBy(c => c.Menucategoryid)
            .Select(c => new CategoryViewModel
            {
                Categoryid = c.Menucategoryid,
                Name = c.Categoryname,
                Description = c.Description
            })
            .ToListAsync();
    }

    public async Task<List<ItemViewModel>> GetItems(int categoryId)
    {
        return await _dbo.Items
            .Where(i => i.Categoryid == categoryId && !i.Isdeleted && !i.Ismodifiable)
            .Select(i => new ItemViewModel
            {
                Itemid = i.Itemid,
                Name = i.Itemname,
                Rate = i.Rate ?? 0,
                Quantity = i.Quantity,
                Itemtype = i.Itemtype,
                Isavailable = i.Available,
                Itemimage = i.Itemimage ?? "~/images/dinning-menu.png"
            })
            .ToListAsync();
    }

    public async Task<List<ModifierGroupViewModel>> GetModifierGroups()
    {
        return await _dbo.Modifiergroups
            .Where(m => !m.Isdeleted)
            .Select(m => new ModifierGroupViewModel
            {
                ModifierGroupId = m.Modifiergroupid,
                modifierGroupName = m.Modifiername,
                modifierGroupDescription = m.Description,
                ExistingModifiers = _dbo.Itemmodifiergroupmappings
                .Where(c => c.Modifiergroupid == m.Modifiergroupid)
                .Join(_dbo.Items,
                mapping => mapping.Itemid,
                item => item.Itemid,
                (mapping, item) => new ModifiersViewModel
                {
                    ModifierId = item.Itemid,
                    ModifierGroupId = item.Categoryid,
                    Name = item.Itemname,
                    Unit = item.Unit,
                    Rate = item.Rate ?? 0,
                    Quantity = item.Quantity,
                    Isdeleted = item.Isdeleted
                })
            .Where(item => item.Quantity > 0 && !item.Isdeleted).ToList()
            })
            .ToListAsync();
    }

    public async Task<List<ModifiersViewModel>> GetModifiers(int categoryId)
    {
        return await _dbo.Items
            .Where(i => !i.Isdeleted && i.Ismodifiable)
            .Select(i => new ModifiersViewModel
            {
                ModifierId = i.Itemid,
                ModifierGroupId = i.Categoryid,
                Name = i.Itemname,
                Unit = i.Unit,
                Rate = i.Rate ?? 0,
                Quantity = i.Quantity
            })
            .ToListAsync();
    }

    public async Task AddCategory(Menucategory menucategory)
    {
        await _dbo.Menucategories.AddAsync(menucategory);
        await _dbo.SaveChangesAsync();
    }

    public Menucategory GetCategoryForEdit(int categoryId)
    {
        return _dbo.Menucategories.FirstOrDefault(c => c.Menucategoryid == categoryId && c.Isdeleted == false);
    }

    public async Task<MenuItemViewModel> GetItemsByCategoryAsync(int categoryid, string searchTerm, int page, int pageSize)
    {
        var query = _dbo.Items
                        .Where(c => c.Categoryid == categoryid && c.Isdeleted == false && c.Ismodifiable == false && c.Quantity > 0);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(item => item.Itemname.ToLower().Contains(searchTerm.ToLower()));
        }

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var paginatedItems = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new ItemViewModel
            {
                Itemid = item.Itemid,
                Name = item.Itemname,
                Rate = item.Rate,
                Itemtype = item.Itemtype,
                Unit = item.Unit,
                Quantity = item.Quantity,
                Isavailable = item.Available,
                Itemimage = item.Itemimage,
                Tax = item.Tax,
                ItemShortCode = item.Itemshortcode,
                Description = item.Description,
                CategoryId = item.Categoryid,
            })
            .ToListAsync();

        return new MenuItemViewModel
        {
            Items = paginatedItems,
            CurrentPage = page,
            totalItems = totalItems,
            TotalPages = totalPages,
            PageSize = pageSize,
            SelectedCategoryId = categoryid
        };
    }

    public async Task<MenuItemViewModel> GetModifierItemsByModifierGroupAsync(int categoryid, string searchTerm, int page, int pageSize)
    {
        var query = _dbo.Itemmodifiergroupmappings
            .Where(c => c.Modifiergroupid == categoryid)
            .Join(_dbo.Items,
                mapping => mapping.Itemid,
                item => item.Itemid,
                (mapping, item) => new ModifiersViewModel
                {
                    ModifierId = item.Itemid,
                    ModifierGroupId = item.Categoryid,
                    Name = item.Itemname,
                    Unit = item.Unit,
                    Rate = item.Rate ?? 0,
                    Quantity = item.Quantity,
                    Isdeleted = item.Isdeleted
                })
            .Where(item => item.Quantity > 0 && !item.Isdeleted);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(item => item.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var paginatedItems = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new MenuItemViewModel
        {
            Modifiers = paginatedItems,
            CurrentPage = page,
            totalItems = totalItems,
            TotalPages = totalPages,
            PageSize = pageSize,
            SelectedCategoryId = categoryid
        };
    }

    public async Task<bool> EditCategory(CategoryViewModel model)
    {
        Menucategory category = GetCategoryForEdit(model.Categoryid);
        if (category == null)
        {
            return false;
        }
        category.Categoryname = model.Name;
        category.Description = model.Description;
        _dbo.Menucategories.Update(category);
        await _dbo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCategory(int categoryId)
    {
        Menucategory category = GetCategoryForEdit(categoryId);
        if (category == null)
        {
            return false;
        }
        category.Isdeleted = true;
        await _dbo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddItem(Item item)
    {
        await _dbo.Items.AddAsync(item);
        await _dbo.SaveChangesAsync();
        return true;
    }

    public Item GetItemById(int id)
    {
        return _dbo.Items.FirstOrDefault(m => m.Itemid == id);
    }

    public List<Menucategory> GetAllCategories()
    {
        return _dbo.Menucategories.ToList();
    }

    public void UpdateItem(Item item)
    {
        _dbo.Items.Update(item);
        _dbo.SaveChanges();
    }

    public bool DeleteItem(int itemId)
    {
        Item item = _dbo.Items.FirstOrDefault(i => i.Itemid == itemId);
        item.Isdeleted = true;
        _dbo.SaveChanges();
        return true;
    }

    public async Task<bool> AddModifierGroup(Modifiergroup modifiergroup)
    {
        await _dbo.Modifiergroups.AddAsync(modifiergroup);
        await _dbo.SaveChangesAsync();
        return true;
    }

    public Modifiergroup GetModifierGroupById(int id)
    {
        return _dbo.Modifiergroups.FirstOrDefault(mg => mg.Modifiergroupid == id && !mg.Isdeleted);
    }

    public bool UpdateModifierGroup(Modifiergroup model)
    {
        _dbo.Modifiergroups.Update(model);
        _dbo.SaveChangesAsync();
        return true;
    }

    public bool DeleteModifierGroup(Modifiergroup modifierGroup)
    {
        _dbo.Modifiergroups.Update(modifierGroup);
        _dbo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddItemModifierGroupMappings(Itemmodifiergroupmapping modifierMapping)
    {
        await _dbo.Itemmodifiergroupmappings.AddAsync(modifierMapping);
        try
        {
            await _dbo.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("raviraj");
            Console.WriteLine(ex);
            return false;
        }
    }

    public Itemmodifiergroupmapping GetItemModifierGroupMappingsById(int modifierId, int modifierGroupId)
    {
        Itemmodifiergroupmapping mapping = _dbo.Itemmodifiergroupmappings.FirstOrDefault(i => i.Itemid == modifierId && i.Modifiergroupid == modifierGroupId);
        return mapping;
    }

    public int GetTotalCountOfModifierMapping()
    {
        return _dbo.Modifiergroups.Count();
    }

    public async Task<List<ModifiersViewModel>> GetExistingModifiersForEdit(int id)
    {
        return await _dbo.Itemmodifiergroupmappings
                            .Where(c => c.Modifiergroupid == id)
                            .Join(_dbo.Items,
                            mapping => mapping.Itemid,
                            item => item.Itemid,
                            (mapping, item) => new ModifiersViewModel
                            {
                                ModifierId = item.Itemid,
                                ModifierGroupId = item.Categoryid,
                                Name = item.Itemname,
                                Unit = item.Unit,
                                Rate = item.Rate ?? 0,
                                Quantity = item.Quantity,
                                Isdeleted = item.Isdeleted
                            }).ToListAsync();
    }

    public async Task<List<ModifierGroupViewModel>> GetModifierGroupsForEditModifier()
    {
        return  _dbo.Modifiergroups
                    .Where(m => !m.Isdeleted)
                    .Select(m => new ModifierGroupViewModel
                    {
                        ModifierGroupId = m.Modifiergroupid,
                        modifierGroupName = m.Modifiername
                    }).ToList();
    }

    public async Task<int> GetTotalCountOfItems()
    {
        return await _dbo.Items.MaxAsync(i => i.Itemid);
    }

    public async Task<List<Itemmodifiergroupmapping>> GetModifierGroupsForEditItem(int itemId)
    {
        try
        {
            return  _dbo.Itemmodifiergroupmappings.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception123 ", ex);
            return null;
            // throw;
        }
        
        
    }

}