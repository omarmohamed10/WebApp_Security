using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
    [Authorize]
    public class UserProfileModel : PageModel
    {
        private readonly UserManager<User> userManager;
        [BindProperty]
        public UserProfileViewModel UserProfile { get; set; }
        [BindProperty]
        public string? SuccessMessage { get; set; }
        public UserProfileModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.UserProfile = new UserProfileViewModel();
            this.SuccessMessage = string.Empty;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            this.SuccessMessage = string.Empty;
            var (user, depClaim, posClaim) = await GetUserInfo();
            UserProfile.Department = depClaim?.Value;
            UserProfile.Position = posClaim?.Value;
            UserProfile.Email = User.Identity.Name;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();
            try
            {
                var (user, depClaim, posClaim) = await GetUserInfo();
                await userManager.ReplaceClaimAsync(user, depClaim, new Claim(depClaim.Type, UserProfile.Department));
                await userManager.ReplaceClaimAsync(user, posClaim, new Claim(posClaim.Type, UserProfile.Position));
                
            }
            catch
            {
                ModelState.AddModelError("UserProfile", "Error Occured when saving user profile.");
            }
            this.SuccessMessage = "The user profile is saved successfully.";

            return Page();
        }
        private async Task<(User , Claim , Claim)> GetUserInfo()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var claims = await userManager.GetClaimsAsync(user);
            var depClaim = claims.FirstOrDefault(x => x.Type == "Department");
            var posClaim = claims.FirstOrDefault(x => x.Type == "Position");
            return (user , depClaim, posClaim);
        } 
    }
    public class UserProfileViewModel
    {
        public string Email { get; set;}
        [Required]
        public string Department { get; set;}
        [Required]
        public string Position { get; set;}
    }

}
