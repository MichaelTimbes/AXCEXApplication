using AXCEXONLINE.Models;
using AXCEX_ONLINE.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AXCEXONLINE.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeModel> EmployeeModel { get; set; }

        public DbSet<AdminModel> AdminModel { get; set; }
    }
}
