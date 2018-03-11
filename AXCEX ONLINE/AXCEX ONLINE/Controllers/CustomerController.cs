using Microsoft.AspNetCore.Mvc;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;
using Microsoft.AspNetCore.Identity;
using AXCEX_ONLINE.Services;
using Microsoft.Extensions.Logging;

namespace AXCEXONLINE.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ProjectDbContext _projectcontext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        public CustomerController(
            ApplicationDbContext context,
            ProjectDbContext projectcontext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AXCEX_ONLINE.Controllers.AccountController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _projectcontext = projectcontext;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region CUSTOMER_HOME
        [HttpGet]
        public IActionResult CustomerHome(string returnURL = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                // Find User ID
                var id = _userManager.GetUserId(User);
                //var ViewResult = _cont
                return View();
            }
            else
            {
                return RedirectToAction(controllerName: "Account", actionName: "Login");
            }
        }

        #endregion CUSTOMER_HOME

    }
}