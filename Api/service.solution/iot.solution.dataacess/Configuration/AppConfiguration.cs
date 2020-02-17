using iot.solution.data.Utilities;
using System;
using System.Xml.Serialization;

namespace iot.solution.data.Configuration
{

    public abstract class AppConfiguration
    {
        [XmlIgnore]
        [NonSerialized]
        public IConfigurationProvider Provider = null;
        
        [XmlIgnore]
        [NonSerialized]
        public string ErrorMessage = string.Empty;
        
        protected bool InitializeCalled = false;
        public AppConfiguration()
        { }
        public void Initialize(IConfigurationProvider provider = null, 
                               string sectionName = null, 
                               object configData = null)
        {
            // Initialization occurs only once
            if (InitializeCalled)
               return;
            InitializeCalled = true;  
            
            if (string.IsNullOrEmpty(sectionName))
                sectionName = this.GetType().Name;

            OnInitialize(provider,sectionName,configData);
        }

        protected virtual void OnInitialize(IConfigurationProvider provider, 
                                           string sectionName,
                                           object configData)
        {
            if (provider == null)
                provider = OnCreateDefaultProvider(sectionName, configData);

            Provider = provider;
            if (!Provider.Read(this))
                throw new InvalidOperationException(Provider.ErrorMessage);
        }

        protected virtual IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
			var providerType = typeof(JsonFileConfigurationProvider<>);
			var type = this.GetType();
            Type typeProvider = providerType.MakeGenericType(type);

            var provider = Activator.CreateInstance(typeProvider) as IConfigurationProvider;

            // if no section name is passed it goes into standard appSettings
            if (!string.IsNullOrEmpty(sectionName))
                provider.ConfigurationSection = sectionName;

            return provider;
        }

        public virtual bool Write()
        {
            Initialize();

            if (!Provider.Write(this))
            {
                ErrorMessage = Provider.ErrorMessage;
                return false;
            }
            
            return true;
        }
        public virtual string WriteAsString()
        {
            Initialize();

            string xml = string.Empty;
            Provider.EncryptFields(this);

            SerializationUtils.SerializeObject(this, out xml);

            if (!string.IsNullOrEmpty(xml))
                Provider.DecryptFields(this);

            return xml;
        }
        public virtual T Read<T>()
                where T : AppConfiguration, new()
        {
            Initialize();

            var inst = Provider.Read<T>();
            if (inst == null)
            {
                ErrorMessage = Provider.ErrorMessage;
                return null;
            }

            return inst;
        }
        public virtual bool Read()
        {
            Initialize();

            if (!Provider.Read(this))
            {
                ErrorMessage = Provider.ErrorMessage;
                return false;
            }
            return true;
        }
        public virtual bool Read(string xml)
        {
            Initialize();

            var newInstance = SerializationUtils.DeSerializeObject(xml, GetType());

            DataUtils.CopyObjectData(newInstance, this, "Provider,Errormessage");

            if (newInstance != null)
            {
                Provider.DecryptFields(this);
            }

            return true;
        }
        public static T Read<T>(IConfigurationProvider provider)
            where T : AppConfiguration, new()
        {            
            return provider.Read<T>() as T;            
        }
        public static T Read<T>(string xml, IConfigurationProvider provider)
            where T : AppConfiguration, new()
        {                        
            T result =  SerializationUtils.DeSerializeObject(xml, typeof(T)) as T;            

            if (result != null && provider != null)
                provider.DecryptFields(result);

            return result;
        }
        public static T Read<T>(string xml)
            where T : AppConfiguration, new()
        {
            return Read<T>(xml, null);
        }
    }

    class MyAppConfiguration : AppConfiguration
    {
        public MyAppConfiguration()
        {
            MyProperty = "My default property value";
            MaxPageListItems = 15;
            ApplicationTitle = "My great application!";
        }
        public string MyProperty { get; set; }
        public int MaxPageListItems { get; set; }
        public string ApplicationTitle { get; set; }
        protected override IConfigurationProvider OnCreateDefaultProvider(string sectionName, object configData)
        {
            return base.OnCreateDefaultProvider(sectionName, configData);
        }
    }
}


              