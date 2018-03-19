using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AXCEX_ONLINE.Models.ComponentViewModels;
using AXCEX_ONLINE.Models.EmployeeViewModels;

namespace AXCEXONLINE.Controllers
{
    [ViewComponent(Name = "CustomerProjectVC")]
    public class CustomerProjectViewComponent : ViewComponent
    {
        private readonly ProjectDbContext _context;
        private readonly ApplicationDbContext _appcontext;
        

        public CustomerProjectViewComponent(
            ProjectDbContext context,
            ApplicationDbContext appcontext
            )
        {
            _context = context;
            _appcontext = appcontext;
        }
        /* Here, the ID is the customer email which is the key used for projects
         */
        public async Task<IViewComponentResult> InvokeAsync(string customer_email)
        {
            // Account for Null String
            if (String.IsNullOrEmpty(customer_email))
            {
                ViewData["Message"] = "No Customer Email..";
                return View("CustomerProjViewComp");
            }

            var projects = _context.ProjectModel.Where(proj => proj.Customer == customer_email);

            // Handle Case That There are No Projects
            int ProjectCount = await projects.ToAsyncEnumerable().Count();

            if (ProjectCount == 0)
            {
                ViewData["Message"] = "There are no projects.";
                return View("CustomerProjViewComp");
            }
            // Else- Customer Email isn't Null and There is at least One Project
            else
            {
                // Create Component View List
                var ViewList = new List<CustomerProjectCompVM>();

                foreach(ProjectModel p in projects)
                {
                    // Find Scope for the Project in Question
                    var ProjectScope = _context.Scopes.Where(S => S.ProjectId == p.ID).OrderBy(S=> S.ScopeVersion).Last();

                    // Initialization to Handle DividebyZero Issue
                    var Percent = (decimal) 0;

                    // Figure Percentage
                    if (ProjectScope.ScopeMaxPhaseNumber > 0 && ProjectScope.ScopePhaseNumber > 0)
                    {
                         Percent = ((decimal)ProjectScope.ScopePhaseNumber / ProjectScope.ScopeMaxPhaseNumber) * 100;
                        
                    }
                    var entry = new CustomerProjectCompVM
                    {
                        Project_Name = p.ProjectName,
                        ProgressPhaseCurrent = ProjectScope.ScopePhaseNumber,
                        ProgressPhaseMax = ProjectScope.ScopeMaxPhaseNumber,
                        ActiveProj = p.IsActive,
                        ProjectId = p.ID,
                        ScopeId = ProjectScope.ID
                    };
                    entry.ProgressPercent = Percent;
                    ViewList.Add(entry);
                }
                ViewData["Message"] = "Here are the results";
                return View("CustomerProjViewComp", ViewList);
            }
            
            

            
            
        }
    }
}