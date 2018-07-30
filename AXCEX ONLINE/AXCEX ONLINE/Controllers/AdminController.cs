using System.Linq;
using System.Threading.Tasks;
using AXCEXONLINE.BusinessLogic.Admin;
using AXCEXONLINE.Data;
using AXCEXONLINE.Models;
using AXCEXONLINE.Models.AdminViewModels;
using AXCEX_ONLINE.Models;
using AXCEX_ONLINE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace AXCEXONLINE.Controllers
{
    // Give Explicit Authorization as Admin in Application
    //[Authorize(Roles = "Administrator")]

    public class AdminController : Controller
    {
        #region LocalConstantsAndMemebers
        private const string ModelRole = "Administrator";
        private const string SessionUserName = "_UserName";
        private const string SessionUserEmail = "_Email";
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion LocalConstantsAndMemebers

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger
        )
        {
            _context = context;
            AdminAccountLayer.InitAdminAccountLayer(context);
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> AdminHome(string id = null)
        {
            // Extract UserID
            if (string.IsNullOrEmpty(id)) id = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(id)) return NotFound();
            
            // Pull Profile and Return Model
            var empResult = await _context.AdminModel.SingleOrDefaultAsync(m => m.Id == id);

            return View($"AdminHome", empResult);

        }

        // GET 
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterAdmin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var model = new AdminRegisterViewModel();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(AdminRegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) return RedirectToAction(controllerName: $"Home", actionName: "About");

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);


            if (result.Succeeded)
            {
                // Session Information
                HttpContext.Session.SetString(SessionUserName, user.UserName);
                
                _logger.LogInformation("New Admin Account Created!");

                // Signin New User
                await _signInManager.SignInAsync(user, false);
                _logger.LogInformation("New User Signed In.");

                // Add to Role
                await _userManager.AddToRoleAsync(user, ModelRole);

                // If All Went Well- Go to Employee Home Page
                return RedirectToLocal(returnUrl);
            }

            if (result.Succeeded) return RedirectToAction(controllerName: $"Home", actionName: "About");

           
            model.Errors = AdminAccountLayer.FormatIdentityResultErrors(result);

            return View(model);
        }

        // GET Login View
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AdminLogin(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(AdminLoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            // Log Info
            _logger.LogInformation("Admin logging in...");
            if (ModelState.IsValid)
            {
                var activeUser = await _context.Users.FirstOrDefaultAsync(m => m.UserName == model.AdminEmail);

                if (activeUser == null)
                {
                    ViewData["ErrorMsg"] = "Email is incorrect or user doesn't exist!";
                    return View(model);
                }

                var result = await _signInManager.CheckPasswordSignInAsync(activeUser, model.Password, false);

                if (result.Succeeded)
                {
                    await _signInManager.PasswordSignInAsync(activeUser, model.Password, false, false);
                    HttpContext.Session.SetString(SessionUserName, activeUser.UserName);
                    HttpContext.Session.SetString(SessionUserEmail, activeUser.Email);
                    
                    return RedirectToAction($"AdminHome", $"Admin", new {id = activeUser.Id});
                }

                ModelState.AddModelError(string.Empty, "Invalid Username or Password...");
                return View(model);
            }

            _logger.LogInformation("Invalid State Condition!");
            // If we got this far, something failed, redisplay form
            return View(model);
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(controllerName: "Admin", actionName: "AdminHome");
        }


        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,[FromForm] AdminModel adminModel)
        {
            if (id != adminModel.Id) return NotFound();

            if (!ModelState.IsValid) return NotFound();

            try
            {
                _context.Update(adminModel);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminModelExists(adminModel.Id)) return NotFound();

                throw;
            }

            return RedirectToAction(controllerName: "Admin", actionName: "AdminHome");

        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var adminModel = await _context.AdminModel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (adminModel == null)
                return NotFound();

            return RedirectToAction("Delete", adminModel.Id);
        }

        // POST: Admin/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var adminModel = await _context.AdminModel.SingleOrDefaultAsync(m => m.Id == id);
            _context.AdminModel.Remove(adminModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool AdminModelExists(string id)
        {
            return _context.AdminModel.Any(e => e.Id == id);
        }
    }
}