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
using Microsoft.AspNetCore.Http;

namespace AXCEX_ONLINE.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ProjectDbContext _context;
        private readonly ApplicationDbContext _appcontext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;
        const string ProjectID = "_PojID";

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
        public async Task<IActionResult> AllProjectsPartial()
        {
            return View(await _context.ProjectModel.ToListAsync());
        }
        #region ASSIGNED_EMPLOYEES
        [HttpGet]
        public IActionResult AssignedEmployees(int? id)
        {
            if (id != null)
            {
                HttpContext.Items.TryAdd(ProjectID,id);
                HttpContext.Session.SetInt32(ProjectID, (int)id);
            }
            else
            {
                id = HttpContext.Session.GetInt32(ProjectID);
            }
            // Find All Employees Assigned to the Project id 
            var assigned_to = _context.ProjectAssignments.Where(p => p.ProjKey == id).ToList().OrderBy(p=>p.EmpKey);
                // Begin the return view 
                var emp_list = new List<ProjectEmpsViewModel>();

                foreach (ProjectAssignment p in assigned_to)
                {
                    var emp = _appcontext.EmployeeModel.Where(e => e.EMPID == p.EmpKey).First();

                    var temp = new ProjectEmpsViewModel
                    {
                        Project_ID = (int)id,
                        Assignment_ID = p.ID,
                        Employee_ID = emp.EMPID,
                        Employee_Name = String.Concat(emp.EMP_FNAME + " ", emp.EMP_LNAME)
                    };
                    emp_list.Add(temp);
                }

                return View(emp_list);
            
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
                return RedirectToAction("AssignedEmployees", routeValues: new { id = HttpContext.Session.GetInt32(ProjectID) });
            }
            else
            {


                var projectAssignment = await _context.ProjectAssignments
               
                    .SingleOrDefaultAsync(m => m.ID == id);

                if (projectAssignment == null)
                {
                    return NotFound();
                }
                _context.ProjectAssignments.Remove(projectAssignment);
                await _context.SaveChangesAsync();

                //return RedirectToPage("AssignedEmployees", new { id = HttpContext.Session.GetInt32(ProjectID) });
                return RedirectToAction("AssignedEmployees", routeValues: new { id = HttpContext.Session.GetInt32(ProjectID) });
            }
            
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
                HttpContext.Items.TryAdd(ProjectID, id);

                HttpContext.Session.SetInt32(ProjectID, (int)id);
            }
            else
            {
                id = HttpContext.Session.GetInt32(ProjectID);
            }
            // Find the current assignments
            var candidates = _context.ProjectAssignments.Where(m => m.ProjKey == id).ToList();
            var empIds = new List<int>();
            foreach (ProjectAssignment p in candidates)
            {
                empIds.Add(p.EmpKey);
            }
            // Prep Return View
            var ReturnView = new List<ProjectAssignEmpViewModel>();
            // Select all employees
            var emps = _appcontext.EmployeeModel.ToList().OrderBy(e=>e.EMPID);

            foreach (EmployeeModel temp in emps)
            {

                // Check if employee is already assigned
                if (!empIds.Contains(temp.EMPID))
                {
                    var tempentry = new ProjectAssignEmpViewModel
                    {
                        Project_ID = (int)id,
                        Employee_ID = temp.EMPID,
                        Employee_Name = temp.EMP_FNAME,
                        IsAssigned = false
                    };

                    ReturnView.Add(tempentry);
                }
            }

            return View(ReturnView);
        }
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        // POST: /Project/AssignEmployeeto/{EMPID}
        public async Task<IActionResult> AssignEmployeeto(int empid)
        {

            // Find Project
            int projid = (int)HttpContext.Session.GetInt32(ProjectID);

            // Check if a Relationship Already Exists
            var RelationResult = _context.ProjectAssignments.Where(assignment => assignment.EmpKey == empid && assignment.ProjKey == projid).Any();

            // If the Assignment Already Exists
            if (RelationResult)
            {
                ViewData["Message"] = "Employee Already Assigned.";
                return RedirectToAction("AssignEmployee");
            }
            // Otherwise Create the Assignment
            else
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
            ViewData["Message"] = "Employee Assigned.";
            return RedirectToAction("AssignEmployee");
        }
        #endregion ASSIGN_EMPLOYEE

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
            // Find all the assignments
            var ProjectsAssignment = _context.ProjectAssignments.Where(P => P.ProjKey == id);

            var ViewModel = new ProjectDetailsViewClass
            {
                ActiveProj = projectModel.IsActive,
                Custid = projectModel.Customer,
                ProjBudget = projectModel.ProjBudget,
                ProjCost = projectModel.ProjCurentCost,
                ProjectName = projectModel.ProjectName,
                ProjEnd = projectModel.EndDate,
                ProjStart = projectModel.StartDate,



            };
            ViewModel.SetProjectID(projectModel.ID);

            // Grab Scope and Have Most Reccent be the Most Updated One
            var ScopeProj = _context.Scopes.Where(S=> S.ProjectId == projectModel.ID).OrderBy(S=> S.ScopeVersion);

            // If The Scope Does Exist, Add it
            if (ScopeProj.SingleOrDefault() == null)
            {
                // Add a Temp Scope
                var TempScope = new ScopeModel
                {
                    ProjectId = projectModel.ID,
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
                _context.Scopes.Add(TempScope);
                await _context.SaveChangesAsync();

                ViewModel.CurrentScope = TempScope;
            }
            else
            {
                ViewModel.CurrentScope = ScopeProj.First();
            }

            foreach (ProjectAssignment P in ProjectsAssignment)
            {
                var Employee =  _appcontext.EmployeeModel.Where(U => U.EMPID == P.EmpKey);

                if(Employee != null)
                {
                    ViewModel.Employees.Add(Employee.First());
                }
                
            }
            return View(ViewModel);
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
            // Check if Customer Exists
            var ResultCheck = _appcontext.Users.Where(c=> c.Email == model.CustEmail).Count();
            // Check that model is good to go
            if (ModelState.IsValid && ResultCheck > 0)
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
                var ProjectContext = _context.ProjectModel.Where(P => P.ProjectName == project.ProjectName && P.ProjBudget == project.ProjBudget).First();

                var DefaultScope = new ScopeModel
                {
                    ProjectId = ProjectContext.ID,
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
                _context.Scopes.Add(DefaultScope);
                await _context.SaveChangesAsync();

                // Redirect to the details if success.
                return RedirectToAction(actionName: "ProjectDetails", routeValues: new { id = project.ID });

            }
            else if(ResultCheck == 0)
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

        //// GET: ProjectModels/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: ProjectModels/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,ApplicationUserID,ProjBudget,ProjCurentCost")] ProjectModel projectModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(projectModel);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(AllProjectsPartial));
        //    }
        //    return View(projectModel);
        //}

        // GET: ProjectModels/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var projectModel = await _context.ProjectModel.SingleOrDefaultAsync(m => m.ID == id);
        //    if (projectModel == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(projectModel);
        //}

        //// POST: ProjectModels/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ID,ApplicationUserID,ProjBudget,ProjCurentCost")] ProjectModel projectModel)
        //{
        //    if (id != projectModel.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(projectModel);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProjectModelExists(projectModel.ID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(AllProjectsPartial));
        //    }
        //    return View(projectModel);
        //}
        #region DELETE_PROJECT
        // GET: ProjectModels/Delete/5
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // First Delete All Entries Where Employees Were Assigned to Project
            var emps = _context.ProjectAssignments.Where(a => a.ProjKey == id);
            // And Delete All the Scopes
            var scopes = _context.Scopes.Where(P => P.ProjectId == id);
            // If there are emps
            if (emps != null)
            {
                // Remove Each One
                foreach (ProjectAssignment p in emps)
                {
                    _context.Remove(p);
                }
                // Update the Db
                await _context.SaveChangesAsync();
            }

            // There is at least one Scope associated with a Project
            foreach (ScopeModel s in scopes)
            {
                _context.Remove(s);
            }

            await _context.SaveChangesAsync();
            
            
            // Then Delete the project
            var projectModel = await _context.ProjectModel.SingleOrDefaultAsync(m => m.ID == id);
            _context.ProjectModel.Remove(projectModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllProjectsPartial));
        }
        #endregion DELETE_PROJECT
        private bool ProjectModelExists(int id)
        {
            return _context.ProjectModel.Any(e => e.ID == id);
        }
    }
}
