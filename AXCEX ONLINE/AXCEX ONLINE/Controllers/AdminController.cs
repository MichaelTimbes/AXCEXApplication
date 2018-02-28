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
        const string MODEL_ROLE = "Administrator";
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
                var user = new AdminModel
                {
                   
                   
                   UserName = model.UserName,
                   Email = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Session Information
                    HttpContext.Session.SetString(SessionUserName, user.UserName);
                    HttpContext.Session.SetString(SessionUserId, user.Id);
                    _logger.LogInformation("New Admin Account Created!");

                    // Email Confirmation
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    // Signin New User
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("New User Signed In.");

                    // Add to Role
                    IdentityResult RoleRes = await _userManager.AddToRoleAsync(user, MODEL_ROLE);

                    // If All Went Well- Go to Employee Home Page
                    return RedirectToLocal(returnUrl);
                }
                // Else
                RedirectToAction(controllerName: "Home", actionName: "Index");
            }

            // If we got this far, something failed, redisplay form
            //return View(model);
            return RedirectToAction(controllerName: "Home", actionName: "About");
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
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Admin_Uname, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Session Information
                    var admin = await _context.AdminModel.FirstOrDefaultAsync(m => m.UserName == model.Admin_Uname);
                    HttpContext.Session.SetString(SessionUserName, admin.UserName);
                    HttpContext.Session.SetString(SessionUserEmail, admin.Email);
                    HttpContext.Session.SetString(SessionUserId, admin.Id);
                    
                    // Log Info
                    _logger.LogInformation("Admin logged in!");
                    _logger.LogDebug("USING THE LOG DEBUGGER!!!!");
                    // Return to Home
                    return RedirectToLocal(returnUrl);
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Admin account locked out... See webmaster.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {

                    ModelState.AddModelError(string.Empty, "Invalid Username or Password...");
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
