using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AXCEXONLINE.Data;
using AXCEXONLINE.Models.ProjectViewModels;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;
using AXCEX_ONLINE.Models.ProjectViewModels;
using Microsoft.EntityFrameworkCore;

namespace AXCEXONLINE.BusinessLogic.Project
{
    public static class ProjectBl
    {
        public static ProjectDbContext ProjectDb { get; set; }
        public static ApplicationDbContext ApplicationDb { get; set; }

        public static void ProjectLayer(ProjectDbContext projectDbContext, ApplicationDbContext appDbContext)
        {
            ProjectDb = projectDbContext;
            ApplicationDb = appDbContext;
        }

        public static IQueryable<ProjectAssignment> AllProjectAssignments(int projectId)
        {
            //var projectAssignments = ProjectDb.ProjectAssignments.Where(assignment => assignment.ProjKey == projectId);
            var projectAssignments = 

                from pa in ProjectDb.ProjectAssignments

                where pa.ProjKey == projectId

                select pa;
                                           
            return projectAssignments.Any() ? projectAssignments : null;
        }

        public static async ValueTask<ProjectDetailsViewClass> PrepareViewProjectDetails(int projectId)
        {
            var projectModel = await ProjectDb.ProjectModel.SingleOrDefaultAsync(m => m.ID == projectId);

            if (projectModel == null) return new ProjectDetailsViewClass();

            return new ProjectDetailsViewClass
            {
                ActiveProj = projectModel.IsActive,
                Custid = projectModel.Customer,
                ProjBudget = projectModel.ProjBudget,
                ProjCost = projectModel.ProjCurentCost,
                ProjectName = projectModel.ProjectName,
                ProjEnd = projectModel.EndDate,
                ProjStart = projectModel.StartDate,
                ProjectId = projectModel.ID,
                CurrentScope = GetMostUpdatedProjectScope(projectId),
                Employees = AllEmployeesAssignedToProject(projectId).Result.ToList()
        };

           
        }

        public static ScopeModel GetMostUpdatedProjectScope(int projextId)
        {
            return ProjectDb.Scopes.Where(scope =>
                    scope.ProjectId == projextId)
                        .OrderBy(scope => scope.ScopeVersion)
                            .First();
        }

        public static async ValueTask<List<EmployeeModel>> AllEmployeesAssignedToProject(int projectId)
        {
          
            var returnList = new List<EmployeeModel>();

            await AllProjectAssignments(projectId).
                ForEachAsync(a => 
                    returnList.Add(ApplicationDb.EmployeeModel.First(emp =>
                        emp.EMPID == a.EmpKey)));

            return returnList;
            
        }
    }
}
