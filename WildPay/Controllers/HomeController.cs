using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WildPay.Exceptions;
using WildPay.Models;
using WildPay.Models.Entities;
using WildPay.Repositories.Interfaces;

namespace WildPay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGroupRepository _groupRepository;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, IGroupRepository groupRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _groupRepository = groupRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ApplicationUser currentUser = await _userManager.GetUserAsync(User);

                var userId = _userManager.GetUserId(User);

                string firstname = currentUser is not null ? currentUser.Firstname : string.Empty;
                ViewBag.Firstname = firstname;

                var groups = await _groupRepository.GetGroupsAsync(userId);
                return View(groups);
            }
            catch (DatabaseException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CookiesPolicy()
        {
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Exception(string message)
        {
            return View("Exception", message);
        }
    }
}
