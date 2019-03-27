using System;
using System.Web.Mvc;
using MasterChief.DotNet.Infrastructure.VerifyCode;
using MasterChief.DotNet4.Utilities;
using MasterChief.Infrastructure.MvcSample.BackHandler;
using MasterChief.Infrastructure.MvcSample.Models;

namespace MasterChief.Infrastructure.MvcSample.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        // GET: Account

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);
            if (string.Compare(Session["validateCode"].ToString(), model.VerifyCode,
                    StringComparison.OrdinalIgnoreCase) != 0)
                ModelState.AddModelError("VerifyCode", "验证码验证不通过.");
            else
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        ///     生成验证码
        /// </summary>
        /// <param name="style">验证码样式</param>
        /// <returns>ActionResult</returns>
        [AllowAnonymous]
        public ActionResult CreateVerifyCode(string style)
        {
            VerifyCodeHandler verifyCodeHandler = new MvcVerifyCodeHandler();
            var buffer = verifyCodeHandler.CreateValidateCode(style);
            return File(buffer, MimeTypes.ImageGif);
        }
    }
}