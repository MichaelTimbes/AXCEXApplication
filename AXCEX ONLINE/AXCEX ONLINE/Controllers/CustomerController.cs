using Microsoft.AspNetCore.Mvc;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;
using Microsoft.AspNetCore.Identity;
using AXCEX_ONLINE.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using AXCEX_ONLINE.Models.CustomerViewModels;
using System;
using AXCEX_ONLINE.Models.AccountViewModels;

namespace AXCEXONLINE.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ProjectDbContext _context;
        private readonly ApplicationDbContext _appcontext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        const string MODEL_ROLE = "Customer";

        public CustomerController(
            ProjectDbContext context,
            ApplicationDbContext appcontext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _context = context;
            _appcontext = appcontext;

            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            
        }

        #region CUSTOMER_HOME
        [HttpGet]
        public IActionResult CustomerHome(string returnURL = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                // Find User ID
                var id = _userManager.GetUserId(User);
                var Customer_Email = _appcontext.Users.Where(u => u.Id == id).First().Email;
                var Customer_Name = _appcontext.Users.Where(u => u.Id == id).First().UserName;
                var ViewReturn = new CustomerHomeVM
                {
                    CustomerName = Customer_Name,
                    Email= Customer_Email,
                    NumProjects = _context.ProjectModel.Where(p=> p.Customer == Customer_Name).Count()
                };
                var ViewRes = _userManager.GetUserAsync(User).Result;
                return View(ViewReturn);
            }
            else
            {
                return RedirectToAction(controllerName: "Account", actionName: "Login");
            }
        }

        #endregion CUSTOMER_HOME

        #region CUSTOMER_LOGIN
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CustomerLogin(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerLogin(LoginViewModel model, string returnUrl = null)
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
                    //return RedirectToLocal(returnUrl);
                    return RedirectToAction("CustomerHome");
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
        #endregion CUSTOMER_LOGIN

        #region CUSTOMER_REGISTER
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CustomerRegister(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CustomerRegister(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var customer_entry = new CustomerModel { CUSTOMER_NAME = model.Email, CUSTOMER_ACCOUNT = "" };
                var result = await _userManager.CreateAsync(user, model.Password);
                _context.Customers.Add(customer_entry);
                await _context.SaveChangesAsync();
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Email Verification
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    // Signin
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");

                    // Assign Role
                    _logger.LogInformation("User Customer Role Assigned.");
                    IdentityResult RoleRes = await _userManager.AddToRoleAsync(user, MODEL_ROLE);

                    return RedirectToAction("CustomerHome");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        
        #endregion CUSTOMER_REGISTER


        #region CUSTOMER_LOCKOUT
        private object Lockout()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region HELPER_METHODS
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        private void AddErrors(IdentityResult result)
        {
            throw new NotImplementedException();
        }
        #endregion HELPER_METHODS























        #region CUSTOMER_CRUD_METHODS
        // GET: CustomerModels
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customers.ToListAsync());
        }

        // GET: CustomerModels/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerModel = await _context.Customers
                .SingleOrDefaultAsync(m => m.ID == id);
            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
        }

        // GET: CustomerModels/Create
        [HttpGet]
        [Authorize(Roles = "Administrator")]

        public IActionResult Create(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Administrator")]

        public async Task<IActionResult> Create(AdminCustomerRegisterVM model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var customer_entry = new CustomerModel { CUSTOMER_NAME = model.CustomerName, CUSTOMER_EMAIL = model.Email, CUSTOMER_ACCOUNT = model.CustomerAccount };
                var result = await _userManager.CreateAsync(user, model.Password);

                _context.Customers.Add(customer_entry);
                await _context.SaveChangesAsync();

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Email Verification
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Assign Role
                    _logger.LogInformation("User Customer Role Assigned.");
                    IdentityResult RoleRes = await _userManager.AddToRoleAsync(user, MODEL_ROLE);

                    return RedirectToAction("Index");
                }
                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: CustomerModels/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerModel = await _context.Customers.SingleOrDefaultAsync(m => m.ID == id);
            if (customerModel == null)
            {
                return NotFound();
            }
            return View(customerModel);
        }

        // POST: CustomerModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CUSTOMER_NAME,CUSTOMER_ACCOUNT")] CustomerModel customerModel)
        {
            if (id != customerModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerModelExists(customerModel.ID))
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
            return View(customerModel);
        }

        // GET: CustomerModels/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerModel = await _context.Customers
                .SingleOrDefaultAsync(m => m.ID == id);
            if (customerModel == null)
            {
                return NotFound();
            }

            return View(customerModel);
        }

        // POST: CustomerModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customerModel = await _context.Customers.SingleOrDefaultAsync(m => m.ID == id);
            _context.Customers.Remove(customerModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerModelExists(int id)
        {
            return _context.Customers.Any(e => e.ID == id);
        }
        #endregion CUSTOMER_CRUD_METHODS
    }
}
