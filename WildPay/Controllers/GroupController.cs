using Microsoft.AspNetCore.Mvc;

namespace WildPay.Controllers;

public class GroupController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}