using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utility;

namespace Farmer.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(InputModel Input)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                //if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                //{
                //    return RedirectToPage("./ForgotPasswordConfirmation");

                //}
                if(user==null)
                {
                    return NotFound();
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                  "/Account/ResetPassword",
                  pageHandler: null,
                  values: new { code },
                  protocol: Request.Scheme);
                HttpContext.Session.SetString(SD.ResetPassword,SD.ResetPassword);
                var encodeUrl = HtmlEncoder.Default.Encode(callbackUrl);
                var href = "<a href=" + encodeUrl + "> Click here to reset your password" + "</a>";
                var emailSender = _emailSender.SendEmailAsync(Input.Email, "Reset password", href);

                return RedirectToPage("ForgotPasswordConfirmation", new { callback = encodeUrl });
            }
            return Page();
        }
    }
}