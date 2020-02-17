using System;
using System.Reflection;

namespace iot.solution.data.Configuration
{
    public interface IConfigurationProvider
    {
        string ErrorMessage { get; set; }
        string PropertiesToEncrypt { get; set; }
        string EncryptionKey { get; set; }
        string ConfigurationSection { get; set; }
        T Read<T>() where T : AppConfiguration, new();
        bool Read(AppConfiguration config);
        T Read<T>(string xml) where T : AppConfiguration, new();
        bool Read(AppConfiguration config, string xml);
        bool Write(AppConfiguration config);
        string WriteAsString(AppConfiguration config);
        void EncryptFields(AppConfiguration config);
        void DecryptFields(AppConfiguration config);
    }

}
