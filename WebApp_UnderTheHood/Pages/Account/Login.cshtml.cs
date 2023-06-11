using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace WebApp_UnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; }
        public void OnGet()
        {
            this.Credential = new Credential { UserName = "admin" };
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if(Credential.UserName == "admin" && Credential.Password == "password")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,"admim"),
                    new Claim(ClaimTypes.Email,"admin@Company.com"),
                    new Claim("Department" , "HR"),
                    new Claim("Admin","true"),
                    new Claim("EmploymentDate" , "2021-05-01"),
                    new Claim("Manager","true")

                };
                var identity = new ClaimsIdentity(claims , "MyCookieAuth");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                var authProperty = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe
                };
               await HttpContext.SignInAsync("MyCookieAuth", principal , authProperty);
                return RedirectToPage("/Index");
            }
            return Page();

        }
    }

    public class Credential
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
