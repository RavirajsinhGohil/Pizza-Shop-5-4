using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Repository;
using Repository.Data;
using Repository.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Repository.Interfaces;
using System.Threading.Tasks;

namespace Web.Controllers;

public class MenuController : Controller
{
    private readonly IAuthService _authService;
    private readonly IMenuService _menuService;

    public MenuController(IAuthService authService, ApplicationDbContext db, IMenuService menuService)
    {
        _authService = authService;
        _menuService = menuService;
    }

    [HttpGet("Menu")]
    public async Task<IActionResult> Menu(int categoryid = 1)
    {
        var viewModel = await _menuService.GetMenuModel(categoryid);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetItemsByCategory(int categoryid, string searchTerm = "", int page = 1, int pageSize = 2)
    {
        var model = await _menuService.GetItemsByCategoryAsync(categoryid, searchTerm, page, pageSize);
        return PartialView("_MenuItems", model);
    }

    [HttpPost("AddCategory")]
    public async Task<IActionResult> AddCategory(CategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                bool isCategoryAdded = _menuService.AddCategory(model);
                if (isCategoryAdded == true)
                {
                    TempData["success"] = "Category is Added!";
                    return RedirectToAction("Menu", "Menu");
                }
            }
            catch (Exception)
            {
                TempData["error"] = "An error occurred while adding the category. Please try again.";
                return View(model);
            }
        }
        return View(model);
    }


    // [HttpGet("EditCategory")]
    // public IActionResult EditCategory(int id)
    // {
    //     var category = _menuService.GetCategoryForEdit(id);
    //     if (category == null)
    //     {
    //         return NotFound();
    //     }
    //     return View(category);
    // }

    [HttpPost]
    public async Task<IActionResult> EditCategory(CategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var category = _menuService.GetCategoryForEdit(model.Categoryid);
                if (category == null)
                {
                    TempData["error"] = "Category not found.";
                    return Json(new { success = false });
                }

                bool isUpdated = await _menuService.EditCategory(model);
                if (isUpdated)
                {
                    TempData["success"] = "Category Updated Successfully!";
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception)
            {
                TempData["error"] = "An error occurred while editing the category. Please try again.";
                return Json(new { success = false });
            }
        }
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {
        var category = _menuService.GetCategoryForEdit(categoryId);
        if (category == null)
        {
            TempData["error"] = "Category not found.";
        }
        bool IsDeleted = await _menuService.DeleteCategory(categoryId);
        if (IsDeleted == true)
        {
            TempData["success"] = "Category is Deleted Successfully";
            return RedirectToAction("Menu");
        }
        else
        {
            TempData["error"] = "Category is not Deleted";
            return RedirectToAction("Menu");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetModifierGroupForNewItem (int modifierGroupId)
    {
        ModifierGroupViewModel modifiergroup = await _menuService.GetModifierGroupForEdit(modifierGroupId);
        return Json(new { success = true, data = modifiergroup });
    }

    [HttpPost("AddNewItem")]
    public async Task<IActionResult> AddNewItem(ItemViewModel model)
    {
        try
        {
            bool IsItemAdded = await _menuService.AddItem(model);
            if (IsItemAdded)
            {
                TempData["success"] = "Menu item added successfully!";
                return RedirectToAction("Menu");
            }
            else
            {
                TempData["error"] = "Menu item is not added!";
                return RedirectToAction("Menu");
            }
        }
        catch (Exception)
        {
            TempData["error"] = "An error occurred while adding the item. Please try again.";
            return View(model);
        }
    }

    [HttpGet("EditMenuItem")]
    public IActionResult EditMenuItem(int id)
    {
        try
        {
            var viewModel = _menuService.GetMenuItemForEdit(id);
            if (viewModel == null)
            {
                return NotFound();
            }

            return PartialView("_EditMenuItemModal", viewModel);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error.");
        }
    }


    // POST: Edit Menu Item (update the item)
    [HttpPost]
    public IActionResult EditMenuItem(int id, MenuItemViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool result = _menuService.UpdatedMenuItem(id, model);
            if (result == false)
            {
                return Json(new { errror = true, message = "Item is not updated successfully." });
            }
            TempData["success"] = "Item is updated successfully";

            return Json(new { success = true, message = "Item updated successfully." });

        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error.");
        }
    }

    // [HttpGet]
    // public IActionResult GetModifierPartialData(int id)
    // {

    //     Item modifiers = _menuService.GetModiferById(id); _db.Items.FirstOrDefault(m => m.Itemid == id);
    //     return PartialView("_ModifierInItem");
    // }

    [HttpGet]
    public IActionResult DeleteMenuItem(int itemId)
    {
        bool isDeleted = _menuService.DeleteItem(itemId);
        if (isDeleted == true)
        {
            TempData["success"] = "Item is Deleted Successfully";
            return RedirectToAction("Menu");
        }
        else
        {
            TempData["error"] = "Item is not Deleted";
            return RedirectToAction("Menu");
        }
    }

    [HttpPost("AddModifierGroup")]
    public async Task<IActionResult> AddModifierGroup(string name, string description, [FromBody] List<int> selectedModifiers)
    {
        try
        {
            bool isModifierGroupAdded = await _menuService.AddModifierGroup(name, description, selectedModifiers);
            if (isModifierGroupAdded == true)
            {
                TempData["success"] = "Modifier Group is Added Successfully!";
                return Json(new { success = true });
            }
            else
            {
                TempData["error"] = "Modifier Group is not Added!";
                return Json(new { success = false });
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = ex;
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("EditModifierGroup")]
    public async Task<IActionResult> EditModifierGroup(int id)
    {
        try
        {
            ModifierGroupViewModel modifierGroupViewModel = await _menuService.GetModifierGroupForEdit(id);
            return Json(new { success = true, data = modifierGroupViewModel });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet]
    public IActionResult DeleteModifierGroup(int modifierGroupId)
    {
        bool isDeleted = _menuService.DeleteModifierGroup(modifierGroupId);
        if (isDeleted == true)
        {
            TempData["success"] = "Modifier Group is Deleted";
            return RedirectToAction("Menu");
        }
        else
        {
            TempData["error"] = "Modifier Group is not Deleted";
            return RedirectToAction("Menu");
        }
    }

    [HttpPost("AddNewModifier")]
    public async Task<IActionResult> AddNewModifier(ModifiersViewModel model)
    {
        try
        {
            bool isAdded = await _menuService.AddModifierinGroups(model);
            if (isAdded)
            {
                TempData["success"] = "Menu item added successfully!";
                return RedirectToAction("Menu");
            }
            else
            {
                TempData["success"] = "Menu item is not added!";
                return RedirectToAction("Menu");
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = ex;
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetModifierItemsByModifierGroup(int categoryid, string searchTerm = "", int page = 1, int pageSize = 2)
    {
        var model = await _menuService.GetModifierItemsByModifierGroupAsync(categoryid, searchTerm, page, pageSize); 
        return PartialView("_Modifiers", model);
    }

    [HttpGet]
    public async Task<IActionResult> EditModifier(int id)
    {
        try
        {
            MenuItemViewModel model = await _menuService.GetDataForEditModifier(id);
            return PartialView("_EditModifierModal", model);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditModifier([FromForm] MenuItemViewModel model)
    {
        bool isUpdated = await _menuService.UpdateModifier(model); // Remaining Functionality
        
        return Json(new { success = true, message = "Modifier Item updated successfully." });

    }

    [HttpPost("AddSelectedModifiers")]
    public async Task<IActionResult> AddSelectedModifiers(int modifierGroupId, string name, string description, [FromBody] List<int> selectedModifiers)
    {
        try
        {
            bool isAdded = await _menuService.addExistingModifiersForEdit(modifierGroupId, name, description, selectedModifiers);
            if (isAdded)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("147852");
            Console.WriteLine(ex);
            return StatusCode(500, "Internal server error.");
        }
    }

}