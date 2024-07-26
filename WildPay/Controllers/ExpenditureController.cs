using Microsoft.AspNetCore.Mvc;
using WildPay.Models.Entities;

namespace WildPay.Controllers;

public class ExpenditureController : Controller
{
    // GET
    public IActionResult List()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Edit()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Edit(Expenditure expenditure)
    {
        return RedirectToAction(actionName: "List", controllerName: "Expenditure");
    }
}