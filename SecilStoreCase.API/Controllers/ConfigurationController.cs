using Microsoft.AspNetCore.Mvc;
using SecilStoreCase.Application.DTOs;
using SecilStoreCase.Application.Interfaces;

namespace SecilStoreCase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigService _configService;

        public ConfigurationController(IConfigService configService)
        {
            _configService = configService;
        }
         
        [HttpGet]
        public async Task<IActionResult> GetAllConfigs()
        {
            var applicationName = HttpContext.Request.Headers["ApplicationName"].ToString();

            if (string.IsNullOrEmpty(applicationName))
            {
                return BadRequest("ApplicationName is missing.");
            }

            var configs = await _configService.GetAllConfigsAsync(applicationName);
            if (configs == null || !configs.Any())
            {
                return NotFound($"No configurations found for application '{applicationName}'.");
            }

            return Ok(configs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConfigById(int id)
        {
            var applicationName = HttpContext.Request.Headers["ApplicationName"].ToString();

            if (string.IsNullOrEmpty(applicationName))
            {
                return BadRequest("ApplicationName is missing.");
            }

            var config = await _configService.GetConfigByIdAsync(id, applicationName);
            if (config == null)
            {
                return NotFound($"No configuration found with ID '{id}' for application '{applicationName}'.");
            }

            return Ok(config);
        }


        [HttpPost]
        public async Task<IActionResult> AddConfiguration([FromBody] ConfigurationCreateDTO config)
        {
            if (config == null)
            {
                return BadRequest("Invalid configuration.");
            }

            var addedConfig = await _configService.AddConfigurationAsync(config);
            return Ok(addedConfig);
        }
         
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateConfiguration(int id, [FromBody] ConfigurationUpdateDTO config)
        {
            if (id != config.Id)
            {
                return BadRequest("Configuration ID mismatch.");
            }

            var updatedConfig = await _configService.UpdateConfigurationAsync(config);
            return Ok(updatedConfig);
        }
         
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfiguration(int id)    
        {
            await _configService.DeleteConfigurationAsync(id);
            return Ok();
        }
    }
}
