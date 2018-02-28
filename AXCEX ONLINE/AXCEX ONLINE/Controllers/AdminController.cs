using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;
using AXCEX_ONLINE.Models.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using AXCEX_ONLINE.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace AXCEX_ONLINE.Controllers
{
    // Give Explicit Authorization as Admin in Application
    //[Authorize(Roles = "Administrator")]

    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        const string SessionUserName = "_UserName";
        const string SessionUserId = "_UserId";
        const string SessionUserEmail = "_Email";

        public AdminController(
            ApplicationDbContext context,
             UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger
            )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }
        public async Task<IActionResult> AdminHome(string id = null)
        {
            // Extract UserID
            if (String.IsNullOrEmpty(id))
            {
                id = HttpContext.Session.GetString(SessionUserId);
            }

            if (!String.IsNullOrEmpty(id))
            {
                // Pull Profile and Return Model
                var EmpResult = await _context.AdminModel.SingleOrDefaultAsync(m => m.Id == id);

                return View(viewName: "AdminHome", model: EmpResult);
            }
            else
            {
                return NotFound();
            }
        }
        // GET 
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterAdmin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(AdminRegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var TEMP_EMP = new AdminModel
                {
                    ADMIN_NAME = model.UserName,
                    UserName = model.UserName,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(TEMP_EMP, model.Password);

                if (result.Succeeded)
                {
                    // Session Information
                    HttpContext.Session.SetString(SessionUserName, TEMP_EMP.UserName);
                    HttpContext.Session.SetString(SessionUserId, TEMP_EMP.Id);
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(TEMP_EMP);
                    var callbackUrl = Url.EmailConfirmationLink(TEMP_EMP.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(TEMP_EMP, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");
                    //return RedirectToAction(controllerName: "Admin", actionName: "AdminHome");
                    return RedirectToLocal(returnUrl);
                }
               // RedirectToAction(controllerName:"Account",actionName: "AddErrors",routeValues: new { result });
            }

            // If we got this far, something failed, redisplay form
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
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private object Lockout()
        {
            throw new NotImplementedException();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(controllerName: "Admin", actionName: "AdminHome");
            }
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdminModel.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminModel = await _context.AdminModel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (adminModel == null)
            {
                return NotFound();
            }

            return View(adminModel);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ADMIN_ID,ADMIN_NAME,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AdminModel adminModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adminModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adminModel);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminModel = await _context.AdminModel.SingleOrDefaultAsync(m => m.Id == id);
            if (adminModel == null)
            {
                return NotFound();
            }
            return View(adminModel);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ADMIN_ID,ADMIN_NAME,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AdminModel adminModel)
        {
            if (id != adminModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminModelExists(adminModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(adminModel);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminModel = await _context.AdminModel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (adminModel == null)
            {
                return NotFound();
            }

            return View(adminModel);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var adminModel = await _context.AdminModel.SingleOrDefaultAsync(m => m.Id == id);
            _context.AdminModel.Remove(adminModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminModelExists(string id)
        {
            return _context.AdminModel.Any(e => e.Id == id);
        }
    }
}
