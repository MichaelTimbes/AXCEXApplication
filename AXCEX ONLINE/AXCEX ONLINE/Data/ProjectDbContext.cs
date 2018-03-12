using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AXCEX_ONLINE.Models;

namespace AXCEX_ONLINE.Data
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        // Connection to Projects
        public DbSet<AXCEX_ONLINE.Models.ProjectModel> ProjectModel { get; set; }
        // Connection to Project Assignmnets
        public DbSet<AXCEX_ONLINE.Models.ProjectAssignment> ProjectAssignments { get; set; }
        // Scopes of work
        public DbSet<Models.ScopeModel> Scopes { get; set; }
        // WBS Models
        public DbSet<Models.WBSModel> WorkBreakDowns { get; set; }
        // Assignments of a WBS
        public DbSet<AXCEX_ONLINE.Models.WBSAssignment> WBSAssignments { get; set; }
       

    }
}