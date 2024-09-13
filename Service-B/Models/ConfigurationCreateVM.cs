namespace Service_B.Models
{
    public class ConfigurationCreateVM
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public string ApplicationName { get; set; } = string.Empty;
    }
}
