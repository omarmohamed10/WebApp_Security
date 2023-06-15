using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        public LoginModel(SignInManager<User> signInManager , UserManager<User> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        [BindProperty]
        public CredentialViewModel Credential { get; set; }
        public void OnGet()
        {
        
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) { return Page(); }
           var user = userManager.FindByEmailAsync(Credential.Email).Result;

           await userManager.SetTwoFactorEnabledAsync(user, true);

            var result = await signInManager.PasswordSignInAsync(this.Credential.Email,
                this.Credential.Password,
                this.Credential.RememberMe,
                false);

            var twoFactorEnabled =  userManager.GetTwoFactorEnabledAsync(user).Result;

            if (result.Succeeded && !twoFactorEnabled)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                if (twoFactorEnabled)
                {
                    return RedirectToPage("/Account/LoginTwoFactor", new
                    {
                        Email = this.Credential.Email,
                        RememberMe = this.Credential.RememberMe
                    });
                }
                if(result.IsLockedOut)
                {
                    ModelState.AddModelError("Login", "You 're Locked out");
                }
                else
                {
                    ModelState.AddModelError("Login", "Faild to login");

                }
                return Page();
            }
        }
    
    }

    public class CredentialViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
