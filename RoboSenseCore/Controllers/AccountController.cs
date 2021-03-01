using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoboSenseCore.Models;
using RoboSenseCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RoboSenseCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationContext _appContext;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appContext = context;
        }

        [HttpGet, Authorize(Roles = "admin")]
        public IActionResult PreRegister(PreRegisterViewModel model)
        {
            //model.Cities = new SelectList(_appContext.Cities, "Id", "Name");
            //model.Schools = _appContext.Schools.FromSql("GetSchools4PreRegister")
            //    .Where(c => c.CityId == new Guid(model.Cities.FirstOrDefault().Value)).OrderBy(s => s.Rank).ToList<Class>();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "admin")]
        public async Task<IActionResult> PreRegister(PreRegisterViewModel model, Guid? CityId)
        {
            if (CityId != null) // Если постбэк был из-за смены города
            {
                ModelState.Clear();
                //model.Cities = new SelectList(_appContext.Cities, "Id", "Name");
                //model.Schools = _appContext.Schools.FromSql("GetSchools4PreRegister")
                //    .Where(c => c.CityId == CityId).OrderBy(s => s.Rank).ToList<Class>();
                return View(model);
            }

            if (ModelState.IsValid)
            {
                model.Email = model.Email.Trim();
                //Если такой представитель уже есть, ругаемся
                ApplicationUser user = await _userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    ModelState.AddModelError(string.Empty, "Представитель с таким именем уже существует!");
                }
                else
                {

                    user = new ApplicationUser { Email = model.Email, UserName = model.Email, PhoneNumber = String.Empty };
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // добавляем Приглашённого
                    SqlParameter[] param = {
                            new SqlParameter("@Email", model.Email),
                            new SqlParameter("@TheToken", code),
                        };
                    Int32 yes = await _appContext.Database.ExecuteSqlCommandAsync("Exec AddInvitee @Email, @Deputizing, @FreeDeputizing, @TheToken", param);
                    if (yes != 1) //Если он не добавился, ругаемся
                    {
                        ModelState.AddModelError(string.Empty, "Не удалось добавить Приглашённого в таблицу Invitee");
                    }
                    else
                    {
                        // создаем ссылку для подтверждения
                        var callbackUrl = Url.Action("Register", "Account", new { code = code }, protocol: HttpContext.Request.Scheme);
                        //Отправляем письмо
                        EmailService emailService = new EmailService();
                        await emailService.SendEmailAsync(model.Email, "Фестиваль «RoboSense»: Приглашение для регистрации в качестве Представителя",
                            $"Для регистрации личного кабинета на сайте robosense.ru перейдите по ссылке: <a href='{callbackUrl}'>Регистрация</a>");
                        return View("DisplayEmail");
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register(RegisterViewModel model, string code)
        {
            ViewBag.Allow = false;
            code = code == null ? "incorrect" : System.Net.WebUtility.HtmlDecode(code);
            SqlParameter[] param = {
                            new SqlParameter("@TheToken", code),
                            new SqlParameter { ParameterName = "@Email", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Output, Size = 50 }
                        };
            // Проверяем наличие приглашения
            _appContext.Database.ExecuteSqlCommand("exec CheckInvitee @TheToken, @Email output, @Deputizing output, @FreeDeputizing output", param);
            // Если приглашение имеется, даём зарегистрироваться
            if (param[1].Value != DBNull.Value)
            {
                model.Email = param[1].Value.ToString();
                model.Deputizing = (Guid)param[2].Value;
                model.FreeDeputizing = (Boolean)param[3].Value;
                model.TheToken = code;
                ModelState.Clear();
                ViewBag.Allow = true;
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Ваше приглашение недействительно. Обратитесь в оргкомитет");
            }
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewBag.Allow = true;  // Если уж попали сюда, то код приглашения при вызове метода GET был указан корректно
            if (ModelState.IsValid)
            {
                SqlParameter[] param = {
                            new SqlParameter("@TheToken", model.TheToken),
                            new SqlParameter { ParameterName = "@Email", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Output, Size = 50 }
                        };
                // Проверяем наличие приглашения
                _appContext.Database.ExecuteSqlCommand("exec CheckInvitee @TheToken, @Email output, @Deputizing output, @FreeDeputizing output", param);
                // Если приглашение имеется, даём зарегистрироваться
                if (param[1].Value != DBNull.Value)
                {
                    model.Email = param[1].Value.ToString();
                    model.Deputizing = (Guid)param[2].Value;
                    model.FreeDeputizing = (Boolean)param[3].Value;

                    ApplicationUser user = new ApplicationUser { Email = model.Email, UserName = model.Email, PhoneNumber = model.PhoneNumber };
                    // добавляем пользователя
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // По умолчанию все добавляемые пользователи имеют роль Представителя
                        await _userManager.AddToRoleAsync(user, "deputy");
                        // Подтверждаем почту: один фиг он попал сюда по ссылке, присланной на его почту - значит, она рабочая
                        model.TheToken = await _userManager.GenerateEmailConfirmationTokenAsync(user); // Заново генерим токен (ибо старый не прокатит, т.к. пароль изменился и т.д.
                        var emailResult = await _userManager.ConfirmEmailAsync(user, model.TheToken);
                        if (!emailResult.Succeeded)
                        {
                            ModelState.Clear();
                            ModelState.AddModelError(string.Empty, "При подтверждении адреса электронной почты произошла ошибка. Сообщите об этом в Оргкомитет.");
                            ViewBag.Allow = false;
                            return View(model);
                        }
                        // Удаляем приглашение
                        Int32 yes = await _appContext.Database.ExecuteSqlCommandAsync("delete from Invities where TheToken = @TheToken", param);
                        if (yes != 1)
                        {
                            ModelState.Clear();
                            ModelState.AddModelError(string.Empty, "При удалении приглашения произошла ошибка. Сообщите об этом в Оргкомитет.");
                            ViewBag.Allow = false;
                            return View(model);
                        }
                        // установка куки (логон, то есть)
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("Private", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.Clear();
                            ModelState.AddModelError(string.Empty, error.Description);
                            ViewBag.Allow = false;
                        }
                    }
                }
                else
                {
                    ModelState.Clear();
                    ModelState.AddModelError(string.Empty, "Ваше приглашение недействительно. Обратитесь в оргкомитет");
                    ViewBag.Allow = false;
                }
            }
            return View(model);
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // пользователь с данным email может отсутствовать в бд
                    // тем не менее мы выводим стандартное сообщение, чтобы скрыть 
                    // наличие или отсутствие пользователя в бд
                    return View("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                EmailService emailService = new EmailService();
                await emailService.SendEmailAsync(model.Email, "Сброс пароля",
                    $"Для изменения пароля пройдите по ссылке: <a href='{callbackUrl}'>переустановить пароль</a>");
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                return View("ResetPasswordConfirmation");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Вы не подтвердили свой Email");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
                if (result.Succeeded)
                {
                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("News", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("News", "Home");
        }

        [HttpGet, Authorize(Roles = "admin")]
        public ActionResult Accounts()
        {

            //TODO
            List<AccountsViewModel> usrs = _appContext.AccountsViewModel.FromSql("Select AU.Id, UserName, S.Name as DeputyName, EmailConfirmed, FreeDeputizing, PhoneNumber from AspNetUsers AU inner join Schools S on S.Id=AU.Deputizing").ToList<AccountsViewModel>();
            return View(usrs);
        }

        [HttpGet, Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteAccount(String Id)
        {
            ApplicationUser chel = await _userManager.FindByIdAsync(Id);
            if (chel == null) return RedirectToAction("NullReference", "Home");
            return View(chel);
        }

        [HttpPost, ActionName("DeleteAccount"), Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteUserConfirmed(ApplicationUser chel)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(chel.Id);
            if (chel == null) return RedirectToAction("NullReference", "Home");
            IdentityResult result = await _userManager.RemoveFromRoleAsync(user, "deputy"); // Удаляем из роли
            result = await _userManager.DeleteAsync(user); // Удаляем Представителя
            return RedirectToAction("Accounts");
        }

    }
}