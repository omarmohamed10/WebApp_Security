using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using WebApp.Data.Account;
using WebApp.Services;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; }
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly IEmailService EmailService;
        public RegisterModel(UserManager<User> userManager , IConfiguration configuration, IEmailService emailService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            EmailService = emailService;
        }
        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync() 
        {
            if (!ModelState.IsValid) return Page();
            var user = new User
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email,
              //  Department = RegisterViewModel.Department,
              //  Position = RegisterViewModel.Position
            };
            var claimDep = new Claim("Department", RegisterViewModel.Department);
            var claimPos = new Claim("Position", RegisterViewModel.Position);

            var result = await this.userManager.CreateAsync(user , RegisterViewModel.Password);
            if (result.Succeeded)
            {
                //  return RedirectToPage("/Account/Login");
                await this.userManager.AddClaimAsync(user, claimDep);
                await this.userManager.AddClaimAsync(user, claimPos);

                var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
              var confirmationLink =  Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values : new {userId = user.Id,token = confirmationToken });
              string Sender = configuration["SenderMail"];
   
              await EmailService.SendAsync(Sender, "om644123@gmail.com", "Confirmation Mail",
                                  $"Please Click on this link to confirm your email adress: {confirmationLink}");

                return RedirectToPage("/Account/Login");
            }
            else
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                return Page();
            }
        }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage ="Invaild Email Address.")]
        public string Email { get; set; }
        [Required]
        [DataType(dataType:DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Position { get; set; }
    }
}
