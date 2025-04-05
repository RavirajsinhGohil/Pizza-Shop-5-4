
using Repository.Interfaces;
using Service.Implementations;
using Repository.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
namespace Web.Controllers;

public class TaxController : Controller
{

    private readonly ITaxService _taxservice;

    public TaxController(ITaxService taxservice)
    {
        _taxservice = taxservice;
    }

    public IActionResult Index()
    {  
        ViewBag.active = "Tax";
        return View();
    }

    // Get Partial View OF TaxTableList

    public IActionResult GetTaxList(int pageNumber = 1, int pageSize = 5, string searchKeyword = "")
    {
        var model = _taxservice.GetTaxList( pageNumber, pageSize, searchKeyword);
        return PartialView("_TableList", model);
    }

    // POST : Add Tax
   [HttpPost]
    public IActionResult AddTax(MainTaxViewModel model)
    {
      var response = _taxservice.AddTax(model.Taxes).Result;

       if(!response.Success)
        {
        TempData["error"] = response.Message;
        }
        TempData["success"] = response.Message; 

      return RedirectToAction("Index","Tax");
    }
    // POST : Edit Tax
   [HttpPost]
    public IActionResult EditTax(MainTaxViewModel model)
    {
      var response = _taxservice.EditTax(model.Taxes).Result;

       if(!response.Success)
        {
        TempData["error"] = response.Message;
        }
        TempData["success"] = response.Message; 

      return RedirectToAction("Index","Tax");
    }
    // Delete Tax

    public IActionResult DeleteTax(int id = 1)
    {
      var response = _taxservice.DeleteTax(id).Result;

      if(!response.Success)
      {
        TempData["error"] = response.Message;
      }
        TempData["success"] = response.Message; 

      return RedirectToAction("Index","Tax");
    }
}