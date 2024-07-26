using Microsoft.AspNetCore.Mvc;
using WildPay.Models.Entities;

namespace WildPay.Controllers;

public class GroupController : Controller
{
    
    // GET
    [HttpGet]
    public IActionResult List()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Group group)
    {
        return RedirectToAction(actionName: "List", controllerName: "Group");
    }
}