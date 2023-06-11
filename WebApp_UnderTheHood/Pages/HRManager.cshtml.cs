using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebApp_UnderTheHood.Authorization;
using WebApp_UnderTheHood.DTO;
using WebApp_UnderTheHood.Pages.Account;

namespace WebApp_UnderTheHood.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        [BindProperty]
        public List<WeatherForecastDTO> weatherForecastItems { get; set; } 

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task OnGetAsync()
        {
            weatherForecastItems = await InvokeEndPoint<List<WeatherForecastDTO>>("WebApi", "WeatherForecast");
        }
        private async Task<T> InvokeEndPoint<T>(string ClientName , string URL)
        {
            JwtToken token = null;
            var strTokenObj = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(strTokenObj))
            {
                token = await Authenticate();
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj);
            }

            if (token == null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow)
            {
                token = await Authenticate();
            }

            var httpClient = _httpClientFactory.CreateClient(ClientName);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            return await httpClient.GetFromJsonAsync<T>(URL);
        }
        private async Task <JwtToken> Authenticate()
        {
            var httpClient = _httpClientFactory.CreateClient("WebApi");
            var res = await httpClient.PostAsJsonAsync("Auth", new Credential { UserName = "Admin", Password = "password" });
            res.EnsureSuccessStatusCode();
            string strJwt = await res.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("access_token", strJwt);
            return JsonConvert.DeserializeObject<JwtToken>(strJwt);
        }
    }
}
