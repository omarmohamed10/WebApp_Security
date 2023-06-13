using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using WebApp.Services;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; }
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly IEmailService EmailService;
        public RegisterModel(UserManager<IdentityUser> userManager , IConfiguration configuration, IEmailService emailService)
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
            var user = new IdentityUser
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email
            };
            var result = await this.userManager.CreateAsync(user , RegisterViewModel.Password);
            if (result.Succeeded)
            {
              //  return RedirectToPage("/Account/Login");
              var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
              var confirmationLink =  Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values : new {userId = user.Id,token = confirmationToken });
              string Sender = configuration["SenderMail"];
   
              await EmailService.SendAsync(Sender, "om8412937@gmail.com", "Confirmation Mail",
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
    }
}
