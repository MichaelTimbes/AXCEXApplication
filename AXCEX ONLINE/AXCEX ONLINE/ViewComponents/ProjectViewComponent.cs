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
    [ViewComponent(Name = "ProjectVC")]
    public class ProjectViewComponent : ViewComponent
    {
        private readonly ProjectDbContext _context;
        private readonly ApplicationDbContext _appcontext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProjectViewComponent(
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
        
        public async Task<IViewComponentResult> InvokeAsync(int? id)
        {
            await _context.SaveChangesAsync();

            // Should Never Be Null but Handles Case
            if (id == null)
            {   // Initialize List
                var CompList = new List<ProjectCompView>();

                var Res = await _context.SaveChangesAsync();
                // Select All
                var ProjList = _context.ProjectModel.Where(m => m.IsActive == true).ToList();

                foreach (ProjectModel p in ProjList)
                {
                    var list_item = new ProjectCompView()
                    {
                        ProjectName = p.ProjectName,
                        ProjectId = p.ID,
                        ActiveProj = p.IsActive
                    };
                    CompList.Add(list_item);
                }
                return View("ProjList", CompList);
            }
            else
            {
                // Find all Assignments
                var assigned = _context.ProjectAssignments.Where(a => a.EmpKey == id);
                var ViewList = new List<CompViewAssignedProj>();

                // Build
                foreach (ProjectAssignment p in assigned)
                {
                    // For this View Component, Only Interested in the Active and Assigned Projects
                    var Proj = _context.ProjectModel.Where(pro => pro.ID == p.ProjKey).First();
                    if (Proj.IsActive == true)
                    {
                        var temp = new CompViewAssignedProj
                        {
                            ProjectId = Proj.ID,
                            Project_Name = Proj.ProjectName,
                            Assigned_By = p.authorized_assignment,
                            ActiveProj = Proj.IsActive
                        };
                        ViewList.Add(temp);
                    }
                }
                return View("CompAssignedProjects", ViewList);

            }
            
        }
    }
}