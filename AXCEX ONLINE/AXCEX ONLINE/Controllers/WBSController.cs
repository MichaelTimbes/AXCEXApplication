using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;
using AXCEX_ONLINE.Models.WBSViewModels;
using Microsoft.AspNetCore.Identity;

namespace AXCEXONLINE.Controllers
{
    public class WBSController : Controller
    {
        private readonly ProjectDbContext _context;
        private readonly ApplicationDbContext _appcontext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public WBSController(
            ProjectDbContext context,
            ApplicationDbContext appcontext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
            _context = context;
            _appcontext = appcontext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region CREATE_WBS
        // GET
        public IActionResult CreateWBS(int? ProjectID)
        {
            // Invalid Route ID
            if (ProjectID == null)
            {
                ViewData["Msg"] = "No Value for Project ID";
                return View();
            }

            else
            {

                IEnumerable<ApplicationUser> EMod = _appcontext.Users.ToList();
                var ViewMod = new WBSCreateVM
                {

                    // Declare Block
                    ProjectId = (int)ProjectID,
                    AssignedBy = _userManager.GetUserName(User),
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today,
                    WBSCost = 0,
                    WBSHours = 0,
                    WBSSummary = "",
                };

                return View(ViewMod);
            }
        }
        // POST
        [HttpPost]
        public async Task<IActionResult> CreateWBS(WBSCreateVM model)
        {
            var WBS = new WBSModel
            {
                AssignedBy = model.AssignedBy,
                ProjectId = model.ProjectId,
                WBSSummary = model.WBSSummary,
                WBSCost = model.WBSCost,
                WBSHours = model.WBSHours,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            await _context.WorkBreakDowns.AddAsync(WBS);
            await _context.SaveChangesAsync();
            ViewBag.Msg = "Successfully Added New WBS";

            return View(model);
        }
        #endregion CREATE_WBS

        #region VIEW_WBS
        #endregion VIEW_WBS

        #region EDIT_WBS
        #endregion EDIT_WBS

        #region WBS_CRUD
        // GET: WBSModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkBreakDowns.ToListAsync());
        }

        // GET: WBSModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wBSModel = await _context.WorkBreakDowns
                .SingleOrDefaultAsync(m => m.ID == id);
            if (wBSModel == null)
            {
                return NotFound();
            }

            return View(wBSModel);
        }

        // GET: WBSModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WBSModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProjectId,AssignedBy,WBSSummary,WBSHours,StartDate,EndDate,WBSCost")] WBSModel wBSModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wBSModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wBSModel);
        }

        // GET: WBSModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wBSModel = await _context.WorkBreakDowns.SingleOrDefaultAsync(m => m.ID == id);
            if (wBSModel == null)
            {
                return NotFound();
            }
            return View(wBSModel);
        }

        // POST: WBSModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ProjectId,AssignedBy,WBSSummary,WBSHours,StartDate,EndDate,WBSCost")] WBSModel wBSModel)
        {
            if (id != wBSModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wBSModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WBSModelExists(wBSModel.ID))
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
            return View(wBSModel);
        }

        // GET: WBSModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wBSModel = await _context.WorkBreakDowns
                .SingleOrDefaultAsync(m => m.ID == id);
            if (wBSModel == null)
            {
                return NotFound();
            }

            return View(wBSModel);
        }

        // POST: WBSModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wBSModel = await _context.WorkBreakDowns.SingleOrDefaultAsync(m => m.ID == id);
            _context.WorkBreakDowns.Remove(wBSModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WBSModelExists(int id)
        {
            return _context.WorkBreakDowns.Any(e => e.ID == id);
        }
        #endregion WBS_CRUD
    }
}
