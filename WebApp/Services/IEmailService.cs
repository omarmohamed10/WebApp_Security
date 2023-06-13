namespace WebApp.Services
{
    public interface IEmailService
    {
        public Task SendAsync(string from, string to, string subject, string body);
    }
}
