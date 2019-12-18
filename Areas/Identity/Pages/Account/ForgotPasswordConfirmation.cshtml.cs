using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Farmer.Areas.Identity.Pages.Account
{
    public class ForgotPasswordConfirmationModel : PageModel
    {
      public String CallbackUrl { get; set; }
        public async Task<IActionResult> OnGetAsync(string callback = null)
        {
            if(callback ==null)
            {
                return Page();
            }
            else
            {
                CallbackUrl = callback;
                return Page();
            }
        }
    }
}