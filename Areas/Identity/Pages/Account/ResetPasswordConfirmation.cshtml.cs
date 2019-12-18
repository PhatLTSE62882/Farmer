using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utility;
using Microsoft.AspNetCore.Http;

namespace Farmer.Areas.Identity.Pages.Account
{
    public class ResetPasswordConfirmationModel : PageModel
    {
        public void OnGet()
        {
            HttpContext.Session.Remove(SD.ResetPassword);
        }
    }
}