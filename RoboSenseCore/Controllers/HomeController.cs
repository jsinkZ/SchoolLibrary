using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoboSenseCore.Models;
using RoboSenseCore.ViewModels.Home;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RoboSenseCore.Controllers
{
    public class HomeController : Controller
    {
        #region "Статичные страницы"

        public ActionResult PrivacyAgreement(AgreementType type)
        {
            if (type == AgreementType.GDPR) return View(new PrivacyViewModel(type));
            else return PartialView(new PrivacyViewModel(type));
        }

        public ActionResult Documents()
        {
            ViewBag.Message = "Документы: согласия и приказы";
            return View();
        }

        public ActionResult UnderConstruction()
        {
            ViewBag.Message = "Готовится к публикации";
            return View();
        }


        public ActionResult PrivateMenu()
        {
            return View();
        }

        public ActionResult RegistrationHowTo()
        {
            return View();
        }

        public ActionResult News()
        {
            ViewBag.Message = "Новости";
            //return RedirectToAction("UnderConstruction", "Home");
            return View();
        }


        public ActionResult Contacts()
        {
            ViewBag.Message = "Контакты";

            return View();
        }


        #endregion

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationContext _appContext;

        public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appContext = context;
        }


        [Authorize]
        public async Task<IActionResult> Private()
        {
            if (User.IsInRole("admin"))
            {
                return RedirectToAction("AdminPrivate", "Home");
            }
            else
            {
                ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                //InitCabinet(user);
                return View();
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult AdminPrivate()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
