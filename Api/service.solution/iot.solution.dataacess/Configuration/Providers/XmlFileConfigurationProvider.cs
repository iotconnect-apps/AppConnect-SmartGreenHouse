using System;
using System.IO;
using iot.solution.data.Utilities;
using iot.solution.dataacess.Properties;

namespace iot.solution.data.Configuration
{
    public class XmlFileConfigurationProvider<TAppConfiguration> : ConfigurationProviderBase<TAppConfiguration>
        where TAppConfiguration: AppConfiguration, new()
    {
        public string XmlConfigurationFile
        {
            get { return _XmlConfigurationFile; }
            set { _XmlConfigurationFile = value; }
        }
        private string _XmlConfigurationFile = string.Empty;
        public bool UseBinarySerialization
        {
          get { return _UseBinarySerialization; }
          set { _UseBinarySerialization = value; }
        }
        private bool _UseBinarySerialization = false;
        public override bool Read(AppConfiguration config)
        {
            var newConfig = SerializationUtils.DeSerializeObject(XmlConfigurationFile,typeof(TAppConfiguration),UseBinarySerialization) as TAppConfiguration;

            if (File.Exists(XmlConfigurationFile) && newConfig == null)
                throw new ArgumentException(string.Format(Resources.InvalidXMLConfigurationFileFormat,XmlConfigurationFile));

            if (newConfig == null)
            {
                if(Write(config))
                    return true;
                return false;
            }

            DecryptFields(newConfig);
            DataUtils.CopyObjectData(newConfig, config, "Provider,ErrorMessage");
            
            return true;
        }
        public override TAppConfig Read<TAppConfig>()
        {
            var result = SerializationUtils.DeSerializeObject(XmlConfigurationFile,typeof(TAppConfig),UseBinarySerialization) as TAppConfig;
            if (result != null)
                DecryptFields(result);

            return result;
        }
        public override bool  Write(AppConfiguration config)
        {
            EncryptFields(config);
            
            bool result = SerializationUtils.SerializeObject(config, XmlConfigurationFile, UseBinarySerialization);
            
            // Have to decrypt again to make sure the properties are readable afterwards
            DecryptFields(config);

            return result;
 	    }
    }

}
