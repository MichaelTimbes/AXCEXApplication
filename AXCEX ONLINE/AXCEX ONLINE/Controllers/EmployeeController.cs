using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AXCEXONLINE.Controllers;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;
using AXCEX_ONLINE.Models.AccountViewModels;
using AXCEX_ONLINE.Models.EmployeeViewModels;
using AXCEX_ONLINE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AXCEX_ONLINE.Controllers
{
    public class EmployeeController : Controller
    {
        private const string MODEL_ROLE = "Employee";
        private const string SessionUserName = "_UserName";
        private const string SessionUserId = "_UserId";
        private const string SessionUserEmail = "_Email";
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly ProjectDbContext _projectcontext;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeController(
            ApplicationDbContext context,
            ProjectDbContext projectcontext,
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
            _projectcontext = projectcontext;
        }

        /*

        VIEW ASSIGNED PROJECTS REGION

            */

        #region VIEW_ASSIGNED_PROJECTS

        [HttpGet]
        public async Task<IActionResult> ViewAssignedProjects(int? id)
        {
            if (id == null)
            {
                // Use Current User
                var USR = await _userManager.GetUserAsync(User);
                // Find FK for User Id
                var emp = _context.EmployeeModel.Where(e => e.Id == USR.Id);
                // Pull Employee Id
                id = emp.First().EMPID;
            }

            // Find all Assignments
            var assigned = _projectcontext.ProjectAssignments.Where(a => a.EmpKey == id);
            var ViewList = new List<EmployeeViewAssignedProjectsVM>();

            // Build
            foreach (var p in assigned)
            {
                var Proj = _projectcontext.ProjectModel.First(pro => pro.ID == p.ProjKey);
                var temp = new EmployeeViewAssignedProjectsVM
                {
                    Project_Name = Proj.ProjectName,
                    Assigned_By = p.authorized_assignment,
                    Start_Date = Proj.StartDate,
                    Due_Date = Proj.EndDate,
                    Project_ID = Proj.ID
                };
                ViewList.Add(temp);
            }

            return View(ViewList);
        }

        #endregion VIEW_ASSIGNED_PROJECTS

        /*

        EMPLOYEE HOME CONTROLLER METHOD REGION

            */

        #region EMPLOYEE_HOME

        // GET Employee/EmployeeHome/

        public async Task<IActionResult> EmployeeHome(string id = null)
        {
            // Extract UserID
            if (string.IsNullOrEmpty(id)) id = _userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(id))
            {
                // Pull Profile and Return Model
                var EmpResult = await _context.EmployeeModel.SingleOrDefaultAsync(m => m.Id == id);

                return View("EmployeeHome", EmpResult);
            }

            return NotFound();
        }

        #endregion EMPLOYEE_HOME

        /*

        EMPLOYEE LOGIN REGION

            */

        #region EMPLOYEE_LOGIN

        #region IFrameVersion

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> EmployeeLoginFrameable(string returnUrl = null)
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
        public async Task<IActionResult> EmployeeLoginWithLoginModel(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            // Log Info
            _logger.LogInformation("Employee logging in...");
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout!!
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true!!!!
                //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                var activeUser = await _context.EmployeeModel.FirstOrDefaultAsync(m => m.Email == model.Email);
                if (activeUser == null)
                {
                    ViewData["ErrorMsg"] = "Email is incorrect or user doesn't exist!";
                    return View(model);
                }

                var result = await _signInManager.CheckPasswordSignInAsync(activeUser, model.Password, false);

                if (result.Succeeded)
                {
                    await _signInManager.PasswordSignInAsync(activeUser, model.Password, false, false);

                    _logger.LogInformation("In the success condition!!");
                    // Session Information
                    //var activeUser = await _context.AdminModel.FirstOrDefaultAsync(m => m.Email == model.Email);
                    HttpContext.Session.SetString(SessionUserName, activeUser.UserName);
                    HttpContext.Session.SetString(SessionUserEmail, activeUser.Email);
                    //HttpContext.Session.SetString(SessionUserId, activeUser.Id); // Particularly Sensitive Data- Only Use for Debug

                    // Log Info
                    _logger.LogInformation("User logged in!");
                    _logger.LogInformation("USING THE LOG DEBUGGER!!!!");
                    LocalRedirect("EmployeeHome");
                    // Return to Home
                    return RedirectToAction("EmployeeHome", "Employee", new {id = activeUser.Id});
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Employee account locked out... See webmaster.");
                    return RedirectToAction(nameof(Lockout));
                }

                ModelState.AddModelError(string.Empty, "Invalid Username or Password...");
                return View(model);
            }

            _logger.LogInformation("Invalid State Condition!");
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion IFrameVersion

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
                var activeUser = await _context.EmployeeModel.FirstOrDefaultAsync(m => m.Email == model.Email);
                if (activeUser == null)
                {
                    ViewData["ErrorMsg"] = "Email is incorrect or user doesn't exist!";
                    return View(model);
                }

                var result = await _signInManager.CheckPasswordSignInAsync(activeUser, model.Password, false);

                if (result.Succeeded)
                {
                    await _signInManager.PasswordSignInAsync(activeUser, model.Password, false, false);

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

                ModelState.AddModelError(string.Empty, "Invalid Username or Password...");
                return View(model);
            }

            _logger.LogInformation("Invalid State Condition!");
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private object Lockout()
        {
            throw new NotImplementedException();
        }

        #endregion EMPLOYEE_LOGIN

        /*

        CREATE EMPLOYEE (CUSTOM VERSION) REGION

            */


        #region CREATE_EMP_ADMIN

        // GET: EmployeeModels/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var employeeModel = await _context.EmployeeModel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (employeeModel == null) return NotFound();

            return View(employeeModel);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult RegisterEmployeeAdmin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterEmployeeAdmin(EMPRegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            _logger.LogDebug("Checking Model State");
            if (ModelState.IsValid)
            {
                _logger.LogDebug("Model is Valid");
                _logger.LogDebug("Creating Employee");

                var checkid = _context.EmployeeModel.Where(e => e.EMPID == model.Empid);
                var checkemail = _context.EmployeeModel.Where(e => e.Email == model.Email);

                if (checkid.Any())
                {
                    ViewData["Error"] = "Employee ID Already Exists";
                    return View(model);
                }

                if (checkemail.Any())
                {
                    ViewData["Error"] = "Employee Email Already Exists";
                    return View(model);
                }

                var user = new EmployeeModel
                {
                    EMP_FNAME = model.FNAME,
                    EMPID = model.Empid,
                    EMP_LNAME = model.LNAME,
                    UserName = model.FNAME,
                    Email = model.Email
                };

                _logger.LogDebug("Creating User");
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add to Role
                    var roleRes = await _userManager.AddToRoleAsync(user, MODEL_ROLE);
                    // Session Information
                    HttpContext.Session.SetString(SessionUserName, user.UserName);
                    HttpContext.Session.SetString(SessionUserId, user.Id);
                    _logger.LogInformation("User created a new account with password.");

                    // Email Confirmation
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    _logger.LogInformation("User created a new account with password.");


                    // If All Went Well- Go to Employee Home Page
                    return RedirectToAction("Index");
                }

                _logger.LogDebug("Couldn't Create User");
                // Else
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            _logger.LogDebug("Model Invalid in Register Employee");
            return RedirectToAction(controllerName: "Home", actionName: "Index");
        }

        #endregion CREATE_EMP_ADMIN

        /*

        REGISTER EMPLOYEE REGION

            */

        #region REGISTER_EMPLOYEE

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
            _logger.LogDebug("Checking Model State");
            if (ModelState.IsValid)
            {
                _logger.LogDebug("Model is Valid");
                _logger.LogDebug("Creating Employee");

                var checkid = _context.EmployeeModel.Where(e => e.EMPID == model.Empid);
                var checkemail = _context.EmployeeModel.Where(e => e.Email == model.Email);

                if (checkid.Any())
                {
                    ViewData["Error"] = "Employee ID Already Exists";
                    return View(model);
                }

                if (checkemail.Any())
                {
                    ViewData["Error"] = "Employee Email Already Exists";
                    return View(model);
                }

                var user = new EmployeeModel
                {
                    EMP_FNAME = model.FNAME,
                    EMPID = model.Empid,
                    EMP_LNAME = model.LNAME,
                    UserName = model.FNAME,
                    Email = model.Email
                };

                _logger.LogDebug("Creating User");
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add to Role
                    var roleRes = await _userManager.AddToRoleAsync(user, MODEL_ROLE);
                    // Session Information
                    HttpContext.Session.SetString(SessionUserName, user.UserName);
                    HttpContext.Session.SetString(SessionUserId, user.Id);
                    _logger.LogInformation("User created a new account with password.");

                    // Email Confirmation
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    // Signin New User
                    await _signInManager.SignInAsync(user, false);
                    _logger.LogInformation("User created a new account with password.");


                    // If All Went Well- Go to Employee Home Page
                    return RedirectToLocal(returnUrl);
                }

                _logger.LogDebug("Couldn't Create User");
                // Else
                RedirectToAction(controllerName: "Home", actionName: "Index");
            }

            // If we got this far, something failed, redisplay form
            _logger.LogDebug("Model Invalid in Register Employee");
            return RedirectToAction(controllerName: "Home", actionName: "Index");
        }

        #endregion REGISTER_EMPLOYEE

        /*
         
         EDIT EMPLOYEE REGION
         
             */

        #region EDIT_METHOD

        // GET: Employee/EditEmployee/?
        //[Authorize(Roles = "Employee")]
        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        public IActionResult EditEmployee(int? empId)
        {
            // If there isn't a vaule passed, then use the active user
            if (empId == null)
            {
                var uid = _userManager.GetUserId(User);
                empId = _context.EmployeeModel.First(E => E.Id == uid).EMPID;
            }

            var userContext = _context.EmployeeModel.First(m => m.EMPID == empId);

            var updateForm = new EMPEditViewModel
            {
                EmpID = userContext.EMPID,
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
            var userUpdate = _context.EmployeeModel.First(m => m.EMPID == model.EmpID);

            // Things to Update
            userUpdate.EMP_FNAME = model.Employee_fname;
            userUpdate.EMP_LNAME = model.Employee_lname;
            userUpdate.UserName = model.Employee_userName;
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

            ViewData["Msg"] = "User Updated";
            return View(model);
        }

        #endregion EDIT_METHOD

        /*

        DELETE EMPLOYEE REGION

            */

        #region DELETE_METHOD

        [Authorize(Roles = "Administrator")]
        // GET: EmployeeModels/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var employeeModel = await _context.EmployeeModel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (employeeModel == null) return NotFound();

            return View(employeeModel);
        }

        // POST: EmployeeModels/Delete/5
        // Route Masking for Delete Confirmation
        [HttpPost]
        [ActionName("Delete")]
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

        /*

        HELPER METHODS AND CREATE METHOD REGION

            */

        #region HELPER_METHODS

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(controllerName: "Employee", actionName: "EmployeeHome");
        }

        // GET: EmployeeModels/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmployeeModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "ID,EMP_FNAME,EMP_LNAME,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")]
            EmployeeModel employeeModel)
        {
            if (!ModelState.IsValid) return View(employeeModel);
            _context.Add(employeeModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: EmployeeModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.EmployeeModel.Where(e => !e.EMPID.Equals(null)).OrderBy(e => e.EMPID).ToListAsync());
        }
        #endregion HELPER_METHODS
    }
}