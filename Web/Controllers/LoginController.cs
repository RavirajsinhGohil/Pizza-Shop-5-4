using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Repository.ViewModel;
using Service.Interfaces;
using Repository.Models;
using static Service.Implementations.UserService;

namespace Web.Controllers;

public class LoginController : Controller
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    public LoginController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        string request_cookie = Request.Cookies["Email"];
        if (!string.IsNullOrEmpty(request_cookie))
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _authService.Login(model.Email, model.password);
            if (user == null)
            {
                TempData["error"] = "Login Unsuccessful";
                ModelState.AddModelError("Email", "Wrong Email");
                ModelState.AddModelError("Password", "Wrong Password");
                return View();
            }
            string token = _authService.GenerateJwtToken(user.Email, user.Roleid);

            var cookie = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            var JWTtoken = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("Token", token, JWTtoken);
            Response.Cookies.Append("Username", user.Username, cookie);
            Response.Cookies.Append("Image", user.Profileimagepath, cookie);

            if (model.RememberMe)
            {
                Response.Cookies.Append("Email", model.Email, cookie);
                cookie.Expires = DateTime.UtcNow.AddDays(30);
            }
            TempData["success"] = "Login Successful";
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }
    public IActionResult Logout()
    {
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }
        return RedirectToAction("Login", "Login");
    }

    [HttpGet]
    public IActionResult ForgotPassword(string email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            ViewData["Email"] = email;
        }
        else
        {
            ViewData["Email"] = "";
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool emailExist = _authService.CheckEmailExist(model.Email);
            var user = _userService.GetUserByEmail(model.Email);
            if (emailExist)
            {
                string filePath = @"C:\Users\pct216\Downloads\Pizza Shop\Main Project\Pizza Shop\Web\EmailTemplate\ResetPasswordEmailTemplate.html";
                string emailBody = System.IO.File.ReadAllText(filePath);

                var token = _authService.GenerateJwtTokenForgot(user, false);

                var url = Url.Action("ResetPassword", "Login",new { token = Uri.EscapeDataString(token) }, Request.Scheme);
                emailBody = emailBody.Replace("{ResetLink}", url);

                string subject = "Reset Password";
                _authService.SendEmailAsync(model.Email, subject, emailBody);

                TempData["success"] = "Password reset link have been sent to your email.";
            }
            else{
                TempData["error"] = "Email is invalid";
                return RedirectToAction("ForgotPassword", "Login"); 
            }
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        var model = new ResetPasswordViewModel { Token = token };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        var token = model.Token;
        if (TokenBlacklist.Contains(token))
        {
            TempData["error"] = "Token is invalid";
            return RedirectToAction("ForgotPassword", "Login"); ; // Token is invalid
        }
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError(string.Empty, "Passwords do not match");
            TempData["error"] = "Passwords do not match";
            return View(model);
        }

        var result = await _userService.ResetPassword(model.Token, model.NewPassword);
        if (!result)
        {
            TempData["error"] = "Failed to reset password";
            ModelState.AddModelError(string.Empty, "Failed to reset password");
            return View(model);
        }
        TempData["success"] = "Reset Password Successful";
        return RedirectToAction("Login", "Login");
    }


}