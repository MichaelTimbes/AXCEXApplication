using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AXCEX_ONLINE.Data;
using AXCEX_ONLINE.Models;
using AXCEX_ONLINE.Services;

namespace AXCEX_ONLINE
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            /* Db Context
             * Use Sqlite for MACOSX
             * Use SqlServer for Windows
             */
            services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlite("Data Source=main.db"));
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<ProjectDbContext>(options =>
            //options.UseSqlite("Data Source=main.db"));
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                //options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.Name = ".AXCEXONLINE.Session";
                options.Cookie.HttpOnly = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireEmployeeRole", policy => policy.RequireRole("Employee"));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseSession();
            
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            CreateRoles(serviceProvider);
        }
        private void CreateRoles(IServiceProvider serviceProvider)
        {
            const string adminRoleName = "Administrator";
            string[] roleNames = { adminRoleName, "Employee", "Customer" };

            foreach (string roleName in roleNames)
            {
                CreateRole(serviceProvider, roleName);
            }



        }
        private static void CreateRole(IServiceProvider serviceProvider, string roleName)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            Task<bool> roleExists = roleManager.RoleExistsAsync(roleName);
            roleExists.Wait();

            if (!roleExists.Result)
            {
                Task<IdentityResult> roleResult = roleManager.CreateAsync(new IdentityRole(roleName));
                roleResult.Wait();
            }
        }
    }
}
