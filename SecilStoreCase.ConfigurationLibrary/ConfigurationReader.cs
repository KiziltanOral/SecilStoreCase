using Microsoft.Data.SqlClient;
using SecilStoreCase.ConfigurationLibrary.DTOs;
using System.Text.Json;

public class ConfigurationReader
{
    private readonly string _applicationName;
    private readonly string _connectionString;
    private readonly int _refreshTimerIntervalInMs;
    private List<ConfigurationDTO> _configCache;
    private readonly System.Timers.Timer _refreshTimer;

    public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
    {
        _applicationName = applicationName;
        _connectionString = connectionString;
        _refreshTimerIntervalInMs = refreshTimerIntervalInMs;
        _configCache = new List<ConfigurationDTO>();

        LoadInitialConfigurations().Wait();

        _refreshTimer = new System.Timers.Timer(_refreshTimerIntervalInMs);
        _refreshTimer.Elapsed += async (sender, e) => await RefreshConfigurations();
        _refreshTimer.Start();
    }

    public T GetValue<T>(string key)
    {
        var config = _configCache.FirstOrDefault(c => c.Name == key);
        if (config == null)
        {
            throw new KeyNotFoundException($"Key '{key}' not found in the configuration.");
        }

        try
        { 
            return (T)Convert.ChangeType(config.Value, typeof(T));
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Value for key '{key}' cannot be converted to {typeof(T).Name}.", ex);
        }
    }


    public async Task<List<ConfigurationDTO>> GetAllConfigurations()
    {
        return _configCache;
    }

    private async Task LoadInitialConfigurations()
    {
        try
        {
            var configs = await FetchConfigurationsFromDatabase();
            _configCache = configs;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading configurations: {ex.Message}");
        }
    }

    private async Task RefreshConfigurations()
    {
        try
        {
            var configs = await FetchConfigurationsFromDatabase();
            _configCache = configs;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error refreshing configurations, using cached values: {ex.Message}");
        }
    }

    private async Task<List<ConfigurationDTO>> FetchConfigurationsFromDatabase()
    {
        var configurations = new List<ConfigurationDTO>();

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT Id, Name, Type, Value, IsActive, ApplicationName FROM Configurations WHERE ApplicationName = @ApplicationName AND IsActive = 1";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationName", _applicationName);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var type = reader.GetString(2);
                            var value = GetTypedValue(type, reader.GetValue(3));

                            var config = new ConfigurationDTO
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Type = type,
                                Value = value?.ToString(),
                                IsActive = reader.GetBoolean(4),
                                ApplicationName = reader.GetString(5)
                            };

                            configurations.Add(config);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching configurations: {ex.Message}");
        }

        return configurations;
    }

    private object GetTypedValue(string type, object value)
    {
        try
        {
            if (value is JsonElement jsonElement)
            {
                switch (jsonElement.ValueKind)
                {
                    case JsonValueKind.String:
                        return jsonElement.GetString();
                    case JsonValueKind.Number:
                        if (type == "int" || type == "integer")
                        {
                            return jsonElement.GetInt32();
                        }
                        else if (type == "double")
                        {
                            return jsonElement.GetDouble();
                        }
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        return jsonElement.GetBoolean();
                    default:
                        return jsonElement.ToString();
                }
            }
             
            switch (type.ToLower())
            {
                case "int":
                case "integer":
                    return Convert.ToInt32(value);
                case "double":
                    return Convert.ToDouble(value);
                case "bool":
                    return Convert.ToBoolean(value);
                case "string":
                default:
                    return value?.ToString();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting value: {value} to type: {type}. Error: {ex.Message}");
            return null;
        }
    }
}
