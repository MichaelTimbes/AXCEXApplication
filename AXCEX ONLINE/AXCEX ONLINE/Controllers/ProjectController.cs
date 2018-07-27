using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AXCEXONLINE.Controllers;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;
using AXCEX_ONLINE.Models.ProjectViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AXCEX_ONLINE.Controllers
{
    public class ProjectController : Controller
    {
        private const string ProjectId = "_PojID";
        private readonly ApplicationDbContext _appcontext;
        private readonly ProjectDbContext _context;
        private readonly ILogger _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(
            ProjectDbContext context,
            ApplicationDbContext appcontext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger
        )
        {
            _context = context;
            _appcontext = appcontext;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: ProjectModels
        /// <summary>
        ///     GET request that returns all project to a list.
        /// </summary>
        /// <returns>The view 'AllProjectsPartial' with a list of projects as its model.</returns>
        public async Task<IActionResult> AllProjectsPartial()
        {
            return View(await _context.ProjectModel.ToListAsync());
        }

        // GET: ProjectModels/Details/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ProjectDetails(int? id)
        {
            if (id == null) return NotFound();

            var projectModel = await _context.ProjectModel
                .SingleOrDefaultAsync(m => m.ID == id);
            if (projectModel == null) return NotFound();
            // Find all the assignments
            var projectsAssignment = _context.ProjectAssignments.Where(P => P.ProjKey == id);

            var viewModel = new ProjectDetailsViewClass
            {
                ActiveProj = projectModel.IsActive,
                Custid = projectModel.Customer,
                ProjBudget = projectModel.ProjBudget,
                ProjCost = projectModel.ProjCurentCost,
                ProjectName = projectModel.ProjectName,
                ProjEnd = projectModel.EndDate,
                ProjStart = projectModel.StartDate
            };
            viewModel.SetProjectID(projectModel.ID);

            // Grab Scope and Have Most Reccent be the Most Updated One
            var scopeProj = _context.Scopes.Where(S => S.ProjectId == projectModel.ID).OrderBy(S => S.ScopeVersion);

            // If The Scope Does Exist, Add it
            viewModel.CurrentScope = scopeProj.First();

            foreach (var p in projectsAssignment)
            {
                var employee = _appcontext.EmployeeModel.Where(u => u.EMPID == p.EmpKey);

                viewModel.Employees.Add(employee.First());
            }

            return View(viewModel);
        }

        #region ASSIGNED_EMPLOYEES

        /// <summary>
        ///     A function that will return a list of assigned employees given a project id.
        /// </summary>
        /// <param name="id">The project ID as defined in the database.</param>
        /// <returns>The view 'AssignedEmployees' with a list of employees as the data view model.</returns>
        [HttpGet]
        public IActionResult AssignedEmployees(int? id)
        {
            if (id != null)
            {
                HttpContext.Items.TryAdd(ProjectId, id);
                HttpContext.Session.SetInt32(ProjectId, (int) id);
            }
            else
            {
                id = HttpContext.Session.GetInt32(ProjectId);
            }

            // Find All Employees Assigned to the Project id 
            var assignedTo = _context.ProjectAssignments.Where(p => p.ProjKey == id).ToList().OrderBy(p => p.EmpKey);
            // Begin the return view 
            var empList = (from p in assignedTo
                let emp = _appcontext.EmployeeModel.First(e => e.EMPID == p.EmpKey)
                select new ProjectEmpsViewModel
                {
                    Project_ID = (int) id,
                    Assignment_ID = p.ID,
                    Employee_ID = emp.EMPID,
                    Employee_Name = string.Concat(emp.EMP_FNAME + " ", emp.EMP_LNAME)
                }).ToList();

            return View(empList);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        // Supports the Delete Option in Admin View of AssignedEmployees
        public async Task<IActionResult> DelAssignedEmployee(int? id)
        {
            if (id == null)
            {
                // Redirect to the assigned employee list
                _logger.LogInformation("ID IS NULL ERROR IN DELASSIGNEEEMP");
                return RedirectToAction("AssignedEmployees", new {id = HttpContext.Session.GetInt32(ProjectId)});
            }

            var projectAssignment = await _context.ProjectAssignments
                .SingleOrDefaultAsync(m => m.ID == id);

            if (projectAssignment == null) return NotFound();
            _context.ProjectAssignments.Remove(projectAssignment);
            await _context.SaveChangesAsync();

            //return RedirectToPage("AssignedEmployees", new { id = HttpContext.Session.GetInt32(ProjectID) });
            return RedirectToAction("AssignedEmployees", new {id = HttpContext.Session.GetInt32(ProjectId)});
        }

        #endregion ASSIGNED_EMPLOYEES

        #region ASSIGN_EMPLOYEE

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        //GET: /Project/AssignEmployee/{ProjID}
        public IActionResult AssignEmployee(int? id)
        {
            if (id != null)
            {
                HttpContext.Items.TryAdd(ProjectId, id);

                HttpContext.Session.SetInt32(ProjectId, (int) id);
            }
            else
            {
                id = HttpContext.Session.GetInt32(ProjectId);
            }

            // Find the current assignments
            var candidates = _context.ProjectAssignments.Where(m => m.ProjKey == id).ToList();
            var empIds = new List<int>();

            foreach (var p in candidates) empIds.Add(p.EmpKey);
            // Prep Return View
            // Select all employees
            var emps = _appcontext.EmployeeModel.ToList().OrderBy(e => e.EMPID);

            var returnView = (from temp in emps
                where !empIds.Contains(temp.EMPID)
                select new ProjectAssignEmpViewModel
                {
                    Project_ID = (int) id,
                    Employee_ID = temp.EMPID,
                    Employee_Name = temp.EMP_FNAME,
                    IsAssigned = false
                }).ToList();

            return View(returnView);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        // POST: /Project/AssignEmployeeto/{EMPID}
        public async Task<IActionResult> AssignEmployeeto(int empid)
        {
            // Find Project
            if (HttpContext != null)
            {
                var projid = (int) HttpContext.Session.GetInt32(ProjectId);

                // Check if a Relationship Already Exists
                var relationResult = _context.ProjectAssignments.Any(assignment =>
                    assignment.EmpKey == empid && assignment.ProjKey == projid);

                // If the Assignment Already Exists
                if (relationResult)
                {
                    ViewData["Message"] = "Employee Already Assigned.";
                    return RedirectToAction("AssignEmployee");
                }
                // Otherwise Create the Assignment

                {
                    _logger.LogInformation($"NOW ADDING {empid} ASSIGNED TO {projid}");
                    var assignment = new ProjectAssignment
                    {
                        EmpKey = empid,
                        ProjKey = projid,
                        authorized_assignment = _userManager.GetUserName(User)
                    };
                    _context.ProjectAssignments.Add(assignment);
                    await _context.SaveChangesAsync();
                }
            }

            ViewData["Message"] = "Employee Assigned.";
            return RedirectToAction("AssignEmployee");
        }

        #endregion ASSIGN_EMPLOYEE

        #region EDIT_PROJECT

        //GET
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult EditProject(int? id)
        {
            // Verify that id is not Null
            if (id == null) return View();

            var projectContext = _context.ProjectModel.First(p => p.ID == id);

            if (projectContext == null) return View();
            var modelView = new ProjectEditViewClass
            {
                ProjectName = projectContext.ProjectName,
                Custid = projectContext.Customer,
                ActiveProj = projectContext.IsActive,
                ProjBudget = projectContext.ProjBudget,
                ProjCost = projectContext.ProjCurentCost,
                ProjStart = projectContext.StartDate,
                ProjEnd = projectContext.EndDate
            };
            return View(modelView);
        }

        //POST
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
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
                _context.ProjectModel.Update(UpdateModel);
                await _context.SaveChangesAsync();
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
            // Check if Customer Exists
            var resultCheck = _appcontext.Users.Count(c => c.Email == model.CustEmail);
            // Check that model is good to go
            if (ModelState.IsValid && resultCheck > 0)
            {
                // Create New Project
                var project = new ProjectModel
                {
                    ProjectName = model.ProjectName,
                    Customer = model.CustEmail,
                    ProjBudget = model.ProjBudget,
                    ProjCurentCost = model.ProjCost,
                    IsActive = model.ActiveProj,
                    StartDate = model.ProjStart,
                    EndDate = model.ProjEnd
                };

                // Try to add to the database

                _context.Add(project);

                await _context.SaveChangesAsync();

                // Start a Scope for this Project
                var projectContext = _context.ProjectModel.First(P =>
                    P.ProjectName == project.ProjectName && P.ProjBudget == project.ProjBudget);

                var defaultScope = new ScopeModel
                {
                    ProjectId = projectContext.ID,
                    ScopeVersion = 1,
                    ScopeAuthor = "AUTHOR",
                    ScopeGoals = "",
                    ScopeExpectations = "",
                    ScopeLimitations = "",
                    ScopeManager = "",
                    ScopePhase = "1",
                    ScopeSummary = "",
                    ScopeEndDate = DateTime.Today,
                    ScopeStartDate = DateTime.Today
                };
                _context.Scopes.Add(defaultScope);
                await _context.SaveChangesAsync();

                // Redirect to the details if success.
                return RedirectToAction("ProjectDetails", new {id = project.ID});
            }

            if (resultCheck == 0)
            {
                _logger.LogCritical("Something went wrong in creating a new project.");
                ViewData["ErrorMsg"] = "Customer Email Doesn't Exist";
                return View(model);
            }

            _logger.LogCritical("Something went wrong in creating a new project.");
            ViewData["ErrorMsg"] = "Server Side Error- Model State Invalid. Contact website manager.";
            return View(model);
        }

        #endregion CREATE_PROJECT

        #region DELETE_PROJECT

        // GET: ProjectModels/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var projectModel = await _context.ProjectModel
                .SingleOrDefaultAsync(m => m.ID == id);
            if (projectModel == null) return NotFound();

            return View(projectModel);
        }

        // POST: ProjectModels/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // First Delete All Entries Where Employees Were Assigned to Project
            var emps = _context.ProjectAssignments.Where(a => a.ProjKey == id);
            // And Delete All the Scopes
            var scopes = _context.Scopes.Where(P => P.ProjectId == id);
            // If there are emps
            // Remove Each One

            foreach (var p in emps) _context.Remove(p);
            // Update the Db
            await _context.SaveChangesAsync();

            // There is at least one Scope associated with a Project
            foreach (var s in scopes) _context.Remove(s);

            await _context.SaveChangesAsync();


            // Then Delete the project
            var projectModel = await _context.ProjectModel.SingleOrDefaultAsync(m => m.ID == id);
            _context.ProjectModel.Remove(projectModel);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllProjectsPartial));
        }

        #endregion DELETE_PROJECT
    }
}