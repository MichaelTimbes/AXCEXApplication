using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;
using Microsoft.AspNetCore.Authorization;
using AXCEX_ONLINE.Models.ProjectViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace AXCEX_ONLINE.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ProjectDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public ProjectController(ProjectDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger
            )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: ProjectModels
        public async Task<IActionResult> AllProjectsPartial()
        {
            return View(await _context.ProjectModel.ToListAsync());
        }

        // GET: ProjectModels/Details/5
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> ProjectDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectModel = await _context.ProjectModel
                .SingleOrDefaultAsync(m => m.ID == id);
            if (projectModel == null)
            {
                return NotFound();
            }

            return View(projectModel);
        }
        #region EDIT_PROJECT
        //GET
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult EditProject(int? id)
        {
            // Verify that id is not Null
            if (id == null)
            {
                return View();
            }
            else
            {
                var ProjectContext = _context.ProjectModel.Where(p => p.ID == id).First();
                if (ProjectContext != null)
                {
                    var modelView = new ProjectEditViewClass
                    {

                        ProjectName = ProjectContext.ProjectName,
                        Custid = ProjectContext.Customer,
                        ActiveProj = ProjectContext.IsActive,
                        ProjBudget = ProjectContext.ProjBudget,
                        ProjCost = ProjectContext.ProjCurentCost,
                        ProjStart = ProjectContext.StartDate,
                        ProjEnd = ProjectContext.EndDate
                    };
                    return View(model: modelView);
                }
                else
                {
                    return View();
                }
            }
        }
        //POST
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditProject(ProjectEditViewClass model, int? id)
        {
            
            var UpdateModel = await _context.ProjectModel.Where(m => m.ID == id).FirstOrDefaultAsync();
            // Verify it exists
            if (UpdateModel != null)
            {

                UpdateModel.ProjectName = model.ProjectName;
                UpdateModel.IsActive = model.ActiveProj;
                UpdateModel.ProjBudget = model.ProjBudget;
                UpdateModel.ProjCurentCost = model.ProjCost;
                UpdateModel.Customer = model.Custid;
                UpdateModel.StartDate = model.ProjStart;
                UpdateModel.EndDate = model.ProjEnd;
                try
                {
                    _context.ProjectModel.Update(UpdateModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(AllProjectsPartial));
            }
           
                _logger.LogError($"NO VALUE FOUND FOR MODEL WITH ID '{id}'");
                return View(model);
          
        }
        #endregion EDIT_PROJECT

        /* 
         */

        #region CREATE_PROJECT

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        
        public IActionResult CreateProject(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
       
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(ProjectCreateViewClass model, string returnUrl = null)
        {
            _logger.LogInformation("Creating Project");
            // 
            ViewData["ReturnUrl"] = returnUrl;
            // Check that model is good to go
            if (ModelState.IsValid)
            {
                // Create New Project
                var project = new ProjectModel
                {
                    ProjectName = model.ProjectName,
                    Customer = model.Custid,
                    ProjBudget = model.ProjBudget,
                    ProjCurentCost = model.ProjCost,
                    IsActive = model.ActiveProj,
                    StartDate = model.ProjStart,
                    EndDate = model.ProjEnd
                };

                // Try to add to the database
               
                    _context.Add(project);
                    await _context.SaveChangesAsync();
                
                // Redirect to the details if success.
                return RedirectToAction(actionName: "ProjectDetails", routeValues: new { id = project.ID });

            }
            _logger.LogCritical("Something went wrong in creating a new project.");
            return View();
        }
        #endregion CREATE_PROJECT

        // GET: ProjectModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProjectModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ApplicationUserID,ProjBudget,ProjCurentCost")] ProjectModel projectModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AllProjectsPartial));
            }
            return View(projectModel);
        }

        // GET: ProjectModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectModel = await _context.ProjectModel.SingleOrDefaultAsync(m => m.ID == id);
            if (projectModel == null)
            {
                return NotFound();
            }
            return View(projectModel);
        }

        // POST: ProjectModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ApplicationUserID,ProjBudget,ProjCurentCost")] ProjectModel projectModel)
        {
            if (id != projectModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectModelExists(projectModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AllProjectsPartial));
            }
            return View(projectModel);
        }

        // GET: ProjectModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectModel = await _context.ProjectModel
                .SingleOrDefaultAsync(m => m.ID == id);
            if (projectModel == null)
            {
                return NotFound();
            }

            return View(projectModel);
        }

        // POST: ProjectModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectModel = await _context.ProjectModel.SingleOrDefaultAsync(m => m.ID == id);
            _context.ProjectModel.Remove(projectModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllProjectsPartial));
        }

        private bool ProjectModelExists(int id)
        {
            return _context.ProjectModel.Any(e => e.ID == id);
        }
    }
}
