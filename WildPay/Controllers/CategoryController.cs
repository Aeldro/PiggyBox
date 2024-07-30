using Microsoft.AspNetCore.Mvc;
using WildPay.Models.Entities;

namespace WildPay.Controllers
{
    public class CategoryController : Controller
    {
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
        public IActionResult Edit(Category category)
        {
            return RedirectToAction(actionName: "List", controllerName: "Category");
        }

    }
}
