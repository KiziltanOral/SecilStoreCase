using AutoMapper;
using SecilStoreCase.Application.DTOs;
using SecilStoreCase.Application.Interfaces;
using SecilStoreCase.Domain.Entities;
using SecilStoreCase.Infrastructure.Repositories;

namespace SecilStoreCase.Application.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IRepository<Configuration> _repository;
        private readonly IMapper _mapper;

        public ConfigService(IRepository<Configuration> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ConfigurationDTO> GetConfigByKeyAsync(string key, string applicationName)
        {
            var config = await _repository.GetAsync(c => c.Name == key && c.ApplicationName == applicationName && c.IsActive);
            return _mapper.Map<ConfigurationDTO>(config);
        }

        public async Task<ConfigurationDTO> GetConfigByIdAsync(int id, string applicationName)
        {
            var config = await _repository.GetAsync(c => c.Id == id && c.ApplicationName == applicationName && c.IsActive);
            if (config == null) return null;

            return _mapper.Map<ConfigurationDTO>(config);
        }


        public async Task<IEnumerable<ConfigurationDTO>> GetAllConfigsAsync(string applicationName)
        {
            var configs = await _repository.GetAllAsync(c => c.ApplicationName == applicationName && c.IsActive);
            return _mapper.Map<IEnumerable<ConfigurationDTO>>(configs);
        }

        public async Task<ConfigurationDTO> AddConfigurationAsync(ConfigurationCreateDTO configDTO)
        {
            var config = _mapper.Map<Configuration>(configDTO);
            await _repository.AddAsync(config);
            return _mapper.Map<ConfigurationDTO>(config);
        }

        public async Task<ConfigurationDTO> UpdateConfigurationAsync(ConfigurationUpdateDTO configDTO)
        {
            var config = _mapper.Map<Configuration>(configDTO);
            await _repository.UpdateAsync(config);
            return _mapper.Map<ConfigurationDTO>(config);
        }

        public async Task DeleteConfigurationAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
