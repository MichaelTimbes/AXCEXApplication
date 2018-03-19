using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;
using AXCEX_ONLINE.Models.ScopeViewModels;
using Microsoft.AspNetCore.Authorization;

namespace AXCEXONLINE.Controllers
{
    public class ScopeController : Controller
    {
        private readonly ProjectDbContext _context;

        public ScopeController(ProjectDbContext context)
        {
            _context = context;
        }

        /*
         
         VIEW SCOPE REGION
         
             */

        #region VIEW_SCOPE
            /* View Scope
             * Takes a Project ID and Finds the Most Reccent Scope for the Project
             * If the Id is Null, There will be Nothing Shown but an Error
             */
        [HttpGet]
        public IActionResult ViewScope(int? id)
        {
            if (id == null)
            {
                ViewData["ErrorMsg"] = "No Project ID";
                return View();

            }
            // Find the most reccent scope
            var ViewModel = _context.Scopes.Where(S => S.ProjectId == id).OrderBy(S => S.ScopeVersion);

            return View(ViewModel.Last());
        }
        #endregion VIEW_SCOPE

        /*
         
         EDIT SCOPE REGION
         
             */

        #region EDIT_SCOPE
        /* Edit Scope Does the Following:
         * Takes a Project ID Argument
         * Finds the Most Reccent Scope 
         * Returns the View with the Information from the Current Scope
         * Creates a New Scope and Saves it as an Updated Scope
         */

        // GET
        [HttpGet]
        public IActionResult EditScope(int? id)
        {
            if (id == null)
            {
                ViewData["ErrorMsg"] = "No Project Selected.";
                return View("Edit");
            }
            // Grabbing the Most Reccent Scope
            var scope = _context.Scopes.Where(S => S.ProjectId == id).OrderByDescending(S=>S.ScopeVersion).First();
            var projectname = _context.ProjectModel.Where(P => P.ID == scope.ProjectId).First().ProjectName;
            var CurrentScope = new EditScopeViewModel
            {
                ProjectId = scope.ProjectId,
                ScopeParent = scope.ID, // Hidden
                ProjectName = projectname,
                ScopeAuthor = User.Identity.Name,// Hidden
                ScopeCurrentVersion = scope.ScopeVersion,
                UpdatedScopeExpectations = scope.ScopeExpectations,
                UpdatedScopeGoals = scope.ScopeGoals,
                UpdatedScopeLimitations = scope.ScopeLimitations,
                UpdatedScopePhase = scope.ScopePhase,
                UpdatedScopePhaseNum = scope.ScopePhaseNumber,
                UpdatedScopePhaseNumberMax = scope.ScopeMaxPhaseNumber,
                UpdatedScopeSummary = scope.ScopeSummary,
                ScopeStartDate = scope.ScopeStartDate,
                ScopeEndDate = scope.ScopeEndDate,
                ScopeManager = scope.ScopeManager
            };

            return View(CurrentScope);
        }
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        
        public async Task<IActionResult> EditScope(EditScopeViewModel scope)
        {
            // INCREMENT SCOPE VERSION
           

            var NewScope = new ScopeModel
            {
                ProjectId = scope.ProjectId,
                ParentScope = scope.ScopeParent,
                ScopeAuthor = scope.ScopeAuthor,
               
                ScopeExpectations = scope.UpdatedScopeExpectations,
                ScopeGoals = scope.UpdatedScopeGoals,
                ScopeLimitations = scope.UpdatedScopeLimitations,
                ScopeManager = scope.ScopeManager,
                ScopeSummary = scope.UpdatedScopeSummary,
                ScopeVersion = _context.Scopes.Where(S=> S.ProjectId == scope.ProjectId).Count(),
                ScopeMaxPhaseNumber = scope.UpdatedScopePhaseNumberMax,
                ScopePhaseNumber = scope.UpdatedScopePhaseNum,
                ScopePhase = scope.UpdatedScopePhase,
                ScopeEndDate = scope.ScopeEndDate,
                ScopeStartDate = scope.ScopeStartDate,
                
            };
            _context.Scopes.Add(NewScope);
            await _context.SaveChangesAsync();
            ViewData["Msg"] = "Updated Scope and Added to Database";

            return RedirectToAction("Index");
        }
        //POST
        #endregion EDIT_SCOPE

        /*
         SCOPE CRUD REGION- THE BASIC CREATE/READ/UPDATE/DELETE METHODS
             
             */
        #region SCOPE_CRUD
        // GET: ScopeModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Scopes.OrderBy(S=>S.ProjectId).ToListAsync());
        }

        // GET: ScopeModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scopeModel = await _context.Scopes
                .SingleOrDefaultAsync(m => m.ID == id);
            if (scopeModel == null)
            {
                return NotFound();
            }

            return View(scopeModel);
        }

        // GET: ScopeModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ScopeModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProjectId,ScopeVersion,ScopeAuthor,ScopeManager,ScopeExpectations,ScopeLimitations,ScopeSummary,ScopeGoals,ScopePhase,ScopeStartDate,ScopeEndDate")] ScopeModel scopeModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scopeModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(scopeModel);
        }

        // GET: ScopeModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scopeModel = await _context.Scopes.SingleOrDefaultAsync(m => m.ID == id);
            if (scopeModel == null)
            {
                return NotFound();
            }
            return View(scopeModel);
        }

        // POST: ScopeModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ProjectId,ScopeVersion,ScopeAuthor,ScopeManager,ScopeExpectations,ScopeLimitations,ScopeSummary,ScopeGoals,ScopePhase,ScopeStartDate,ScopeEndDate")] ScopeModel scopeModel)
        {
            if (id != scopeModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scopeModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScopeModelExists(scopeModel.ID))
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
            return View(scopeModel);
        }

        // GET: ScopeModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scopeModel = await _context.Scopes
                .SingleOrDefaultAsync(m => m.ID == id);
            if (scopeModel == null)
            {
                return NotFound();
            }

            return View(scopeModel);
        }

        // POST: ScopeModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scopeModel = await _context.Scopes.SingleOrDefaultAsync(m => m.ID == id);
            _context.Scopes.Remove(scopeModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScopeModelExists(int id)
        {
            return _context.Scopes.Any(e => e.ID == id);
        }
        #endregion SCOPE_CRUD
    }
}
