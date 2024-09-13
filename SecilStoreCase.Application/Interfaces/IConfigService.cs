using SecilStoreCase.Application.DTOs;

namespace SecilStoreCase.Application.Interfaces
{
    public interface IConfigService
    {
        Task<ConfigurationDTO> GetConfigByKeyAsync(string key, string applicationName);
        Task<ConfigurationDTO> GetConfigByIdAsync(int id, string applicationName);
        Task<IEnumerable<ConfigurationDTO>> GetAllConfigsAsync(string applicationName);
        Task<ConfigurationDTO> AddConfigurationAsync(ConfigurationCreateDTO configDTO);
        Task<ConfigurationDTO> UpdateConfigurationAsync(ConfigurationUpdateDTO configDTO);
        Task DeleteConfigurationAsync(int id);
    }
}
