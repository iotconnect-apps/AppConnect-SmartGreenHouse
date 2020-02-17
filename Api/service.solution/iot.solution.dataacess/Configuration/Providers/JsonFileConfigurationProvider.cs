using iot.solution.data.Utilities;

namespace iot.solution.data.Configuration
{
    public class JsonFileConfigurationProvider<TAppConfiguration> : ConfigurationProviderBase<TAppConfiguration>
        where TAppConfiguration : AppConfiguration, new()
    {

        public string JsonConfigurationFile
        {
            get { return _JsonConfigurationFile; }
            set { _JsonConfigurationFile = value; }
        }
        private string _JsonConfigurationFile = "applicationConfiguration.json";
        public override bool Read(AppConfiguration config)
        {
            var newConfig = JsonSerializationUtils.DeserializeFromFile(JsonConfigurationFile, typeof(TAppConfiguration)) as TAppConfiguration;
            if (newConfig == null)
            {
                if (Write(config))
                    return true;
                return false;
            }
            DecryptFields(newConfig);
            DataUtils.CopyObjectData(newConfig, config, "Provider,ErrorMessage");

            return true;
        }
        public override TAppConfig Read<TAppConfig>()
        {
            var result = JsonSerializationUtils.DeserializeFromFile(JsonConfigurationFile, typeof(TAppConfig)) as TAppConfig;
            if (result != null)
                DecryptFields(result);

            return result;
        }
        public override bool Write(AppConfiguration config)
        {
            EncryptFields(config);

            bool result = JsonSerializationUtils.SerializeToFile(config, JsonConfigurationFile, false, true);

            // Have to decrypt again to make sure the properties are readable afterwards
            DecryptFields(config);

            return result;
        }
    }

}
