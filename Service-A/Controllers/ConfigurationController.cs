using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service_A.Models;
using System.Text;

namespace ServiceA.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ConfigurationReader _configurationReader;

        public ConfigurationController(HttpClient httpClient, IConfiguration configuration, ConfigurationReader configurationReader)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _configurationReader = configurationReader;
            _httpClient.BaseAddress = new Uri("https://localhost:7020/"); 
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var configurations = await _configurationReader.GetAllConfigurations();                 
                var configurationVMs = configurations.Select(config => new ConfigurationVM
                {
                    Id = config.Id,
                    Name = config.Name,
                    Type = config.Type,
                    Value = config.Value,
                    IsActive = config.IsActive,
                    ApplicationName = config.ApplicationName
                }).ToList();


                var siteName = _configurationReader.GetValue<string>("SiteName");                 
                ViewBag.SiteName = siteName;

                return View(configurationVMs); 
            }
            catch (Exception ex)
            {
                return View(new List<ConfigurationVM>()); 
            }
        }

        public IActionResult Create()
        {
            return PartialView("_CreateModalPartial", new ConfigurationCreateVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ConfigurationCreateVM configVM)
        {
            if (ModelState.IsValid)
            {
                configVM.ApplicationName = _configuration["ApplicationSettings:ApplicationName"];

                if (configVM.ApplicationName == null)
                {
                    return BadRequest("ApplicationName is missing.");
                }

                var jsonContent = JsonConvert.SerializeObject(configVM);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/configuration", content);
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return BadRequest(errorMessage);
                }
            }

            return BadRequest("Invalid model state");
        }

        public async Task<IActionResult> Update(int id)
        {
            var applicationName = _configuration["ApplicationSettings:ApplicationName"];
            if (applicationName == null)
            {
                return BadRequest("ApplicationName is missing.");
            }
            _httpClient.DefaultRequestHeaders.Remove("ApplicationName"); 
            _httpClient.DefaultRequestHeaders.Add("ApplicationName", applicationName);

            var response = await _httpClient.GetAsync($"api/configuration/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var configVM = JsonConvert.DeserializeObject<ConfigurationUpdateVM>(data);
                 
                return Json(configVM);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> Update([FromBody] ConfigurationUpdateVM configVM)
        {
            configVM.ApplicationName = _configuration["ApplicationSettings:ApplicationName"];

            if (configVM.ApplicationName == null)
            {
                return BadRequest("ApplicationName is missing.");
            }

            if (ModelState.IsValid)
            {
                var jsonContent = JsonConvert.SerializeObject(configVM);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/configuration/{configVM.Id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/configuration/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
