using System.Text.Json;

namespace PragueParking.DataAccess
{
    public class FileManager
    {
        private readonly string _dataFilePath;
        private readonly string _configFilePath;

        public FileManager(string dataFilePath = "parkingdata.json", string configFilePath = "config.json")
        {
            _dataFilePath = dataFilePath;
            _configFilePath = configFilePath;
        }

        public void SaveToJson<T>(T data)
        {
            string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_dataFilePath, jsonString);
        }

        public T LoadFromJson<T>() where T : new()
        {
            if (!File.Exists(_dataFilePath))
                return new T();

            string jsonString = File.ReadAllText(_dataFilePath);

            if (string.IsNullOrWhiteSpace(jsonString))
                return new T();

            return JsonSerializer.Deserialize<T>(jsonString);
        }

        public T LoadConfig<T>() where T : new()
        {
            if (!File.Exists(_configFilePath))
            {
                T defaultConfig = new T();
                SaveConfig(defaultConfig);
                return defaultConfig;
            }

            string jsonString = File.ReadAllText(_configFilePath);

            if (string.IsNullOrWhiteSpace(jsonString))
            {
                T defaultConfig = new T();
                SaveConfig(defaultConfig);
                return defaultConfig;
            }
            return JsonSerializer.Deserialize<T>(jsonString);
        }

        public void SaveConfig<T>(T config)
        {
            string jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_configFilePath, jsonString);
        }
    }
}