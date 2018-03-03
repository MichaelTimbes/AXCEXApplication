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
            //HttpContext.Session.Clear();

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeLogin(EMPLoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            // Log Info
            _logger.LogInformation("Employee logging in...");
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout!!
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true!!!!
                //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                var activeUser = await _context.EmployeeModel.FirstOrDefaultAsync(m => m.Email == model.Email );
                var result = await _signInManager.CheckPasswordSignInAsync(activeUser,model.Password,false);
                if (result.Succeeded)
                {
                   await _signInManager.PasswordSignInAsync(activeUser,model.Password,false,false);

                    _logger.LogInformation("In the success condition!!");
                    // Session Information
                    //var activeUser = await _context.AdminModel.FirstOrDefaultAsync(m => m.Email == model.Email);
                    HttpContext.Session.SetString(SessionUserName, activeUser.UserName);
                    HttpContext.Session.SetString(SessionUserEmail, activeUser.Email);
                    //HttpContext.Session.SetString(SessionUserId, activeUser.Id); // Particularly Sensitive Data- Only Use for Debug
                    
                    // Log Info
                    _logger.LogInformation("User logged in!");
                    _logger.LogInformation("USING THE LOG DEBUGGER!!!!");
                    // Return to Home
                    return RedirectToLocal(returnUrl);
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Employee account locked out... See webmaster.");
                    return RedirectToAction(nameof(Lockout));
}
                else
                {

                    ModelState.AddModelError(string.Empty, "Invalid Username or Password...");
                    return View(model);
                }
            }
            _logger.LogInformation("Invalid State Condition!");
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private object Lockout()
        {
            throw new NotImplementedException();
        }




        // GET Employee/EmployeeHome/

        public async Task<IActionResult> EmployeeHome(string id = null)
        {
            // Extract UserID
            if (String.IsNullOrEmpty(id))
            {
                //id = HttpContext.Session.GetString(SessionUserId);
                id = _userManager.GetUserId(User);
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



        #region EDIT_METHOD
        // GET: Employee/EditEmployee/?
        //[Authorize(Roles = "Employee")]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<IActionResult> EditEmployee(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var user = await _userManager.GetUserAsync(User);
            var userContext = _context.EmployeeModel.Where(m => m.Id == user.Id).First();
            var updateForm = new EMPEditViewModel
            {
                Employee_fname = userContext.EMP_FNAME,
                Employee_lname = userContext.EMP_LNAME,
                Employee_userName = userContext.UserName,
                Email = userContext.Email,
                PhoneNumber = userContext.PhoneNumber
            };
            return View(updateForm);
        }

      
        // POST: EmployeeModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Employee")]
        [Authorize(Roles = "Administrator,Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployee(EMPEditViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var email = user.Email;
                if (model.Email != email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                    }
                }

                var phoneNumber = user.PhoneNumber;
                if (model.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                    if (!setPhoneResult.Succeeded)
                    {
                        throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                    }
                }

                var Uname = user.UserName;
                if (model.Employee_userName != Uname)
                {
                    var setUnameResult = await _userManager.SetUserNameAsync(user, model.Employee_userName);
                    if (!setUnameResult.Succeeded)
                    {
                        throw new ApplicationException($"Unexpected error occurred setting new user name for user with ID '{user.Id}'.");
                    }
                }
                    
                if (user != null)
                {
                    var userUpdate = _context.EmployeeModel.Where(m => m.Id == user.Id).First();
                    userUpdate.EMP_FNAME = model.Employee_fname;
                    userUpdate.EMP_LNAME = model.Employee_lname;
                    userUpdate.Email = model.Email;
                    userUpdate.PhoneNumber = model.PhoneNumber;

                    // UPDATE THE EMPLOYEE
                    _context.EmployeeModel.Update(userUpdate);
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(e.ToString());

                        return View();
                    }
                    return RedirectToAction(actionName: "EmployeeHome");

                }
                else
                {
                    return NotFound();
                }
            }
            // Something went terribly wrong
            _logger.LogCritical("Something is wrong with the model state in Employee controller-EditEmployee.");
            return View();
        }
        #endregion EDIT_METHOD

        #region DELETE_METHOD
        [Authorize(Roles = "Administrator")]
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
        // Route Masking for Delete Confirmation
        [HttpPost, ActionName("Delete")]
        // Only An Admin can delete an Employee
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var employeeModel = await _context.EmployeeModel.SingleOrDefaultAsync(m => m.Id == id);
            _context.EmployeeModel.Remove(employeeModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion DELETE_METHOD

        // GET: EmployeeModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.EmployeeModel.ToListAsync());
        }
        #region HELPER_METHODS
        private bool EmployeeModelExists(string id)
        {
            return _context.EmployeeModel.Any(e => e.Id == id);
        }
        #endregion HELPER_METHODS
    }
}
