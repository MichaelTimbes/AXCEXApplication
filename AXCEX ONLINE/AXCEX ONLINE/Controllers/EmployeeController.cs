using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;
using AXCEX_ONLINE.Models.EmployeeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AXCEX_ONLINE.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;

namespace AXCEX_ONLINE.Controllers
{
    //[Authorize(Roles = "EmployeeUser")]

    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        const string MODEL_ROLE = "Employee";
        const string SessionUserName = "_UserName";
        const string SessionUserId = "_UserId";
        const string SessionUserEmail = "_Email";

        public EmployeeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> EmployeeLogin(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            // Be sure to clear the cache too!
            HttpContext.Session.Clear();

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeLogin(EMPLoginViewModel model, string returnUrl = null)
        {
            //ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    // Extract User for Session Seed
                    var Usess = await _context.EmployeeModel.FirstOrDefaultAsync((m => m.Email == model.Email));
                    // Set Context
                    HttpContext.Session.SetString(SessionUserId,Usess.Id);
                    HttpContext.Session.SetString(SessionUserName, Usess.UserName);
                    HttpContext.Session.SetString(SessionUserName, Usess.Email);

                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                   // return RedirectToAction(controllerName:"Employee", actionName:"EmployeeHome");
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(AccountController.LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(AccountController.Lockout));
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
        
        

        // GET Employee/EmployeeHome/
        
        public async Task<IActionResult> EmployeeHome(string id = null)
        {
            // Extract UserID
            if (String.IsNullOrEmpty(id))
            {
                id = HttpContext.Session.GetString(SessionUserId);
            }

            if (!String.IsNullOrEmpty(id))
            {
                // Pull Profile and Return Model
                var EmpResult = await _context.EmployeeModel.SingleOrDefaultAsync(m => m.Id == id);

                return View(viewName: "EmployeeHome", model: EmpResult);
            }
            else
            {
                return NotFound();
            }
        }
        
        // GET: EmployeeModels/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeModel = await _context.EmployeeModel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (employeeModel == null)
            {
                return NotFound();
            }

            return View(employeeModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterEmployee(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterEmployee(EMPRegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new EmployeeModel
                {
                   
                    EMP_FNAME = model.FNAME,
                    EMP_LNAME = model.LNAME,
                    UserName = model.FNAME,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Session Information
                    HttpContext.Session.SetString(SessionUserName,user.UserName);
                    HttpContext.Session.SetString(SessionUserId, user.Id);
                    _logger.LogInformation("User created a new account with password.");
                    
                    // Email Confirmation
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);
                    
                    // Signin New User
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");

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

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(controllerName:"Employee", actionName:"EmployeeHome");
            }
        }

        // GET: EmployeeModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmployeeModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,EMP_FNAME,EMP_LNAME,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] EmployeeModel employeeModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employeeModel);
        }

        // GET: EmployeeModels/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeModel = await _context.EmployeeModel.SingleOrDefaultAsync(m => m.Id == id);
            if (employeeModel == null)
            {
                return NotFound();
            }
            return View(employeeModel);
        }

        
        private bool EmployeeModelExists(string id)
        {
            return _context.EmployeeModel.Any(e => e.Id == id);
        }

        // POST: EmployeeModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "EmployeeUser")]
        //[Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,EMP_FNAME,EMP_LNAME,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] EmployeeModel employeeModel)
        {
            if (id != employeeModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeModelExists(employeeModel.Id))
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
            return View(employeeModel);
        }

        // GET: EmployeeModels/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeModel = await _context.EmployeeModel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (employeeModel == null)
            {
                return NotFound();
            }

            return View(employeeModel);
        }

        // POST: EmployeeModels/Delete/5
        [HttpPost, ActionName("Delete")]
        // Only An Admin can delete an Employee
        //[Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var employeeModel = await _context.EmployeeModel.SingleOrDefaultAsync(m => m.Id == id);
            _context.EmployeeModel.Remove(employeeModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: EmployeeModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.EmployeeModel.ToListAsync());
        }
    }
}
