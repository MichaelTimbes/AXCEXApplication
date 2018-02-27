using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AXCEX_ONLINE.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
