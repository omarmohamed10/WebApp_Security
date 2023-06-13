using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;
        [BindProperty]
        public string Message { get; set; }
        public ConfirmEmailModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId , string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await userManager.ConfirmEmailAsync(user, token);
                if(result.Succeeded)
                {
                    Message = "Email Address is successfully confirm";
                    return Page();
                }
            }
            Message = "Faild to validate email";
            return Page();
        }
    }
}
