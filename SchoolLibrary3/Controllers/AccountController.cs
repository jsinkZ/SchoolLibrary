using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolLibrary3.Models;
using SchoolLibrary3.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolLibrary3.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationContext _appContext;

        public AccountController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, ApplicationContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _appContext = context;
        }
        [HttpGet] //, Authorize(Roles = "admin")]
        public IActionResult PreRegister(PreRegisterViewModel model)
        {
            model.Roles = _roleManager.Roles.ToList();
            return View(model);
        }
        [HttpPost] //, Authorize(Roles = "admin")]
        public async Task<IActionResult> PreRegister(PreRegisterViewModel model, String RoleName)
        {
            model.Roles = _roleManager.Roles.ToList();
            model.RoleName = RoleName;
            if (ModelState.IsValid)
            {
                model.Email = model.Email.Trim();
                //Если такой пользователь уже есть, ругаемся
                User user = await _userManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    ModelState.AddModelError(string.Empty, "Пользователь с таким именем уже существует!");
                }
                else
                {
                    user = new User { Email = model.Email, UserName = model.Email, PhoneNumber = String.Empty };
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // добавляем Приглашённого в таблицу Invities
                    SqlParameter[] param = {
                            new SqlParameter("@Email", model.Email),
                            new SqlParameter("@TheToken", code),
                            new SqlParameter("@RoleName", RoleName)
                        };

                    Int32 yes = await _appContext.Database.ExecuteSqlRawAsync("Exec AddInvitee @Email, @TheToken, @RoleName", param);
                    if (yes != 1) //Если он не добавился, ругаемся
                    {
                        ModelState.AddModelError(string.Empty, "Не удалось добавить Приглашённого в таблицу Invities");
                    }
                    else // Всё в порядке...
                    {
                        // ...создаем ссылку для подтверждения...
                        var callbackUrl = Url.Action("Register", "Account", new { code = code }, protocol: HttpContext.Request.Scheme);
                        //...и отправляем письмо
                        EmailService emailService = new EmailService();
                        await emailService.SendEmailAsync(model.Email, "Школьная библиотека: Приглашение для регистрации в качестве пользователя системы",
                            $"Для регистрации личного кабинета на сайте schl.dialog-el.ru перейдите по ссылке: <a href='{callbackUrl}'>Регистрация</a>");
                        return View("DisplayEmail");
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register(RegisterViewModel model, string code)
        {
            // Это будет справочник классов ViewBag.Classes = new SelectList(_appContext.Classes, "Id", "Name", 2);
            ViewBag.Allow = false; // Флаг позволения на добавление к чисду зарегистрированных поьзователей
            code = code == null ? "incorrect" : System.Net.WebUtility.HtmlDecode(code);
            SqlParameter[] param = {
                            new SqlParameter("@TheToken", code),
                            new SqlParameter { ParameterName = "@Email", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Output, Size = 50 },
                            new SqlParameter { ParameterName = "@RoleName", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Output, Size = 20 }
                        };
            // Проверяем наличие приглашения. Результат прилетит в возвращаемый параметр
            _appContext.Database.ExecuteSqlRaw("exec CheckInvitee @TheToken, @Email output, @RoleName output", param);
            // Если приглашение имеется, даём зарегистрироваться
            if (param[1].Value != DBNull.Value)
            {
                model.Email = param[1].Value.ToString();
                model.TheToken = code;
                ModelState.Clear();
                ViewBag.Allow = true;
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Ваше приглашение недействительно. Обратитесь к Администратору Школной библиотеки");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Это будет справочник классов ViewBag.Classes = new SelectList(_appContext.Classes, "Id", "Name", 2);
            ViewBag.Allow = true;  // Если уж попали сюда, то код приглашения при вызове метода GET был указан корректно
            if (ModelState.IsValid)
            {
                SqlParameter[] param = {
                            new SqlParameter("@TheToken", model.TheToken),
                            new SqlParameter { ParameterName = "@Email", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Output, Size = 50 },
                            new SqlParameter { ParameterName = "@RoleName", SqlDbType = System.Data.SqlDbType.VarChar, Direction = System.Data.ParameterDirection.Output, Size = 20 }
                        };
                // Проверяем наличие приглашения
                _appContext.Database.ExecuteSqlRaw("exec CheckInvitee @TheToken, @Email output, @RoleName output", param);
                // Если приглашение имеется, даём зарегистрироваться
                if (param[1].Value != DBNull.Value)
                {
                    model.Email = param[1].Value.ToString();

                    User user = new User { Email = model.Email, UserName = model.Email, PhoneNumber = model.PhoneNumber, FirstName = model.FirstName, LastName = model.SecondName, PatronymicName = model.PatronymicName };
                    // добавляем пользователя
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // Роль остаётся такой же, как в приглашении
                        await _userManager.AddToRoleAsync(user, param[2].Value.ToString());
                        // Сразу подтверждаем почту: он попал сюда по ссылке, присланной на его почту - значит, она рабочая
                        model.TheToken = await _userManager.GenerateEmailConfirmationTokenAsync(user); // Заново генерим токен (ибо старый не прокатит, т.к. пароль изменился и т.д.
                        var emailResult = await _userManager.ConfirmEmailAsync(user, model.TheToken); // Подтвердили...
                        if (!emailResult.Succeeded) //Если что-то пошло не так...
                        {
                            ModelState.Clear();
                            ModelState.AddModelError(string.Empty, "При подтверждении адреса электронной почты произошла ошибка. Сообщите об этом Администратору Школьной библиотеки");
                            ViewBag.Allow = false;
                            return View(model);
                        }

                        // Теперь Удаляем приглашение
                        Int32 yes = await _appContext.Database.ExecuteSqlRawAsync("delete from Invities where TheToken = @TheToken", param);
                        if (yes != 1)
                        {
                            ModelState.Clear();
                            ModelState.AddModelError(string.Empty, "При удалении приглашения произошла ошибка. Сообщите об этом Администратору Школьной библиотеки");
                            ViewBag.Allow = false; //Запрещаем добавление
                            return View(model);
                        }
                        // установка куки (то есть вход в систему)
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
                    ModelState.AddModelError(string.Empty, "Ваше приглашение недействительно. Обратитесь к Администратору Школьной библиотеки");
                    ViewBag.Allow = false;
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByEmailAsync(model.Email);

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
                        return RedirectToAction("Private", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
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

        [HttpGet, Authorize(Roles = "admin")]
        public ActionResult Accounts()
        {
            //У нас нет ФИО полей. Нужно будет добавить. Если они будут прямо в AspNetUsers, от запроса можно будет избавиться и использовать простой DBSet
            //List<AccountsViewModel> usrs = _appContext.AccountsViewModel.FromSqlRaw("Select Id, UserName, EmailConfirmed, PhoneNumber from AspNetUsers").ToList<AccountsViewModel>();
            //return View(usrs); // _appContext.Users);
            return View(_appContext.Users.ToList()); // _appContext.Users);
        }

        [HttpGet, Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteAccount(String Id)
        {
            User chel = await _userManager.FindByIdAsync(Id);
            if (chel == null) return RedirectToAction("NullReference", "Home");
            return View(chel);
        }

        [HttpPost, ActionName("DeleteAccount"), Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteUserConfirmed(User chel)
        {
            User user = await _userManager.FindByIdAsync(chel.Id);
            if (chel == null) return RedirectToAction("NullReference", "Home");
            var userRoles = await _userManager.GetRolesAsync(user); // Получаем список всех ролей пользователя
            IdentityResult result = await _userManager.RemoveFromRolesAsync(user, userRoles.ToList()); // Удаляем его изо всех ролей
            result = await _userManager.DeleteAsync(user); // Удаляем Пользователя
            return RedirectToAction("Accounts");
        }
    }
}
