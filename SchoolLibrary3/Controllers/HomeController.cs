using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SchoolLibrary3.Models;
using SchoolLibrary3.Models.Entities;
using SchoolLibrary3.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolLibrary3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationContext _appContext;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, ApplicationContext appContext)
        {
            _logger = logger;
            _userManager = userManager;
            _appContext = appContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        

        [Authorize]
        public async Task<IActionResult> Private()
        {
            User user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userRoles = await _userManager.GetRolesAsync(user); // Получаем список всех ролей пользователя (а у нас она строго одна)
            switch (userRoles[0])
            {
                case "admin":
                    return RedirectToAction("Accounts", "Account"); // Управление аккаунтами
                case "librarian":
                    return RedirectToAction("Storage", "Home"); // Управление книжным фондом - TODO
                default:
                    return View(); // Настройки рядового пользователя
            }
        }

        public async Task<IActionResult> Storage()
        {
            /* 
                StorageViewModel ddd = new StorageViewModel();
                ddd.Books = new SelectList(_appContext.Books, "Id", "Name", 2);
                return View(ddd);
            */
            List<Book> BookToList = _appContext.Books.ToList();
            return View(BookToList);
        }
    }
}
