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
                    var entry = new CustomerProjectCompVM
                    {
                        Project_Name = p.ProjectName,
                        // CHANGE THIS WHEN IMPLEMENTATION IS DONE
                        ProgressPercentage = 50,
                        ActiveProj = p.IsActive,
                        ProjectId = p.ID,
                        // CHANGE THIS WHEN SCOPE IMPLEMENTATION IS DONE
                        ScopeId = 0
                    };
                    ViewList.Add(entry);
                }
                ViewData["Message"] = "Here are the results";
                return View("CustomerProjViewComp", ViewList);
            }
            
            

            
            
        }
    }
}