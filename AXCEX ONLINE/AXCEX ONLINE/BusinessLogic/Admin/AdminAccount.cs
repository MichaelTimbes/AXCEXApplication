using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AXCEXONLINE.Data;
using AXCEXONLINE.Models;
using AXCEXONLINE.Models.AdminViewModels;
using AXCEX_ONLINE.Models;
using AXCEX_ONLINE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace AXCEXONLINE.BusinessLogic.Admin
{
    public static class AdminAccountLayer
    {
        public  static ApplicationDbContext Db { get; set; }

        public static void InitAdminAccountLayer(ApplicationDbContext adminAccountDatabase)
        {
            Db = adminAccountDatabase;
        }

        public static List<string> FormatIdentityResultErrors(IdentityResult errorResult)
        {
            var resultingErrors = errorResult.Errors.ToList();

            var returnedErrors = new List<string>();


            foreach (var resultingError in resultingErrors)
            {
                returnedErrors.Add(resultingError.Description + Environment.NewLine);
            }

            return returnedErrors;
        }
    }
}
