using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        private Boolean IsSaveSuccessfull(String message4fail)
        {
            try
            {
                _appContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, ApplicationContext appContext)
        {
            _logger = logger;
            _userManager = userManager;
            _appContext = appContext;
        }

        public ActionResult PrivacyAgreement(AgreementType type)
        {
            if (type == AgreementType.GDPR) return View(new PrivacyViewModel(type));
            else return PartialView(new PrivacyViewModel(type));
        }

        public IActionResult Index()
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
        [HttpGet, Authorize(Roles = "librarian")]
        public async Task<ActionResult> EditBook(Guid? Id)
        {
            BookViewModel book = new BookViewModel();
            Book ddd=await _appContext.Books.FindAsync(Id);
            book.Author = ddd.Author;
            book.Created = ddd.Created;
            book.Description = ddd.Description;
            book.Id = ddd.Id;
            book.Publisher = ddd.Publisher;
            book.TotalPages = ddd.TotalPages;
            book.Name = ddd.Name;

            book.Geners = new SelectList(_appContext.Genres, "Id","Name");
            book.GenreId = ddd.GenreId;
            book.Statuses = new SelectList(_appContext.Statuses, "bookId", "status");
            book.StatusId = ddd.StatusId;



            if (book == null) return RedirectToAction("NullReference", "Home");
            if (Id != null) ViewBag.Title = "Редактирование книги";
            else ViewBag.Title = "Добавление книги";
            return View(book);
        }

        [HttpPost, Authorize(Roles = "librarian")]
        public async Task<ActionResult> EditBook(BookViewModel book2, Guid GenreId, Guid StatusId)
        {

            Book book = new Book();

            book.Author = book2.Author;
            book.Created = book2.Created;
            book.Description = book2.Description;
            book.Id = book2.Id;
            book.Publisher = book2.Publisher;
            book.TotalPages = book2.TotalPages;
            book.Name = book2.Name;
            book.GenreId = GenreId;
            book.StatusId = StatusId;

            if (book == null)
            {
                return RedirectToAction("NullReference", "Home");
            }

            //Book findBook = await _appContext.Books.FindAsync(book.Id);
            if (ModelState.IsValid)
            {
                book.Name = book.Name.Trim();
                if (book.Id == Guid.Empty)
                {
                    book.Id = Guid.NewGuid();
                    book.Created = DateTime.Now;
                    _appContext.Books.Add(book);
                }
                else
                {
                    _appContext.Entry(book).State = EntityState.Modified;
                }

                if (IsSaveSuccessfull("При сохранении участника произошла ошибка") == false)
                {
                    ViewBag.Title = "Ошибка!";
                    return View(book);
                }
            }
            return RedirectToAction("Storage");
        }

        [HttpGet, Authorize(Roles = "librarian")]
        public async Task<ActionResult> writeOffBook(Guid? Id)
        {
            Book book = await _appContext.Books.FindAsync(Id);
            if (book == null) return RedirectToAction("NullReference", "Home");
            if (Id != null) ViewBag.Title = "Редактирование книги";
            else ViewBag.Title = "Добавление книги";
            return View(book);
        }

        [HttpPost, Authorize(Roles = "librarian")]
        public async Task<ActionResult> writeOffBook(Book book)
        {
            if (book == null)
            {
                return RedirectToAction("NullReference", "Home");
            }

            
            _appContext.Entry(book).State = EntityState.Modified;
                
            if (IsSaveSuccessfull("При сохранении участника произошла ошибка") == false)
            {
                return RedirectToAction("Storage");
            }

            ViewBag.Title = "Ошибка!";
            return View(book);
        }
        }
}
