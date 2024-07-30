using Microsoft.AspNetCore.Mvc;
using WildPay.Models.Entities;

namespace WildPay.Controllers;

public class ExpenditureController : Controller
{
    
    // READ
    public IActionResult List()
    {
        return View();
    }
    
    // UPDATE
    [HttpGet]
    public IActionResult Edit()
    {
        return View();
    }

    // UPDATE
    [HttpPost]
    public IActionResult Edit(Expenditure expenditure)
    {
        return RedirectToAction(actionName: "List", controllerName: "Expenditure");
    }

    // CREATE
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // CREATE
    [HttpPost]
    public IActionResult Create(Expenditure expenditure)
    {
        return RedirectToAction(actionName: "List", controllerName: "Expenditure");
    }

    // DELETE
    [HttpGet]
    public IActionResult Delete()
    {
        return View();
    }

    // DELETE
    [HttpPost]
    public IActionResult Delete(Expenditure expenditure)
    {
        return RedirectToAction(actionName: "List", controllerName: "Expenditure");
    }
}