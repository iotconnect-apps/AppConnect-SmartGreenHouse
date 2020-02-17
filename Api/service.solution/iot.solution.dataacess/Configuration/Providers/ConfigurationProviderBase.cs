using System;
using iot.solution.data.Utilities;
using iot.solution.dataacess.Properties;

namespace iot.solution.data.Configuration
{
    public abstract class ConfigurationProviderBase<TAppConfiguration> : IConfigurationProvider
        where TAppConfiguration : AppConfiguration, new()
    {

        public virtual string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }
        private string _ErrorMessage = string.Empty;

        public virtual string PropertiesToEncrypt
        {
            get { return _PropertiesToEncrypt; }
            set { _PropertiesToEncrypt = value; }
        }
        private string _PropertiesToEncrypt = string.Empty;

        public virtual string EncryptionKey
        {
            get { return _EncryptionKey; }
            set { _EncryptionKey = value; }
        }
        private string _EncryptionKey = "x@3|zg?4%ui*";

        public string ConfigurationSection {get; set; }
        
        public abstract T Read<T>()
                where T : AppConfiguration, new();

        public abstract bool Read(AppConfiguration config);
        public abstract bool Write(AppConfiguration config);
        public virtual T Read<T>(string xml)
            where T : AppConfiguration, new()
        {
            if (string.IsNullOrEmpty((xml)))
            {                
                return null;
            }

            T result;
            try
            {
                result = SerializationUtils.DeSerializeObject(xml, typeof(T)) as T;
            }
            catch (Exception ex)
            {
                SetError(ex);
                return null;
            }
            if (result != null)
                DecryptFields(result);

            return result;
        }
        public virtual bool Read(AppConfiguration config, string xml)
        {
            TAppConfiguration newConfig = null;

            // if no data was passed leave the object
            // in its initial state.
            if (string.IsNullOrEmpty(xml))
                return true;

            try
            {
                newConfig = SerializationUtils.DeSerializeObject(xml, config.GetType()) as TAppConfiguration;
                if (newConfig == null)
                {
                    SetError(Resources.ObjectCouldNotBeDeserializedFromXml);
                    return false;
                }
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }
            if (newConfig != null)
            {
                DecryptFields(newConfig);
                DataUtils.CopyObjectData(newConfig, config, "Provider,ErrorMessage");
                return true;
            }
            return false;
        }
        public virtual string WriteAsString(AppConfiguration config)
        {
            string xml = string.Empty;
            EncryptFields(config);

            try
            {
                SerializationUtils.SerializeObject(config, out xml, true);
            }
            catch (Exception ex)
            {
                SetError(ex);
                return string.Empty;
            }
            finally
            {
                DecryptFields(config);
            }

            return xml;
        }
        public virtual void EncryptFields(AppConfiguration config)
        {
            if (string.IsNullOrEmpty(PropertiesToEncrypt))
                return;

            string encryptFieldList = "," + PropertiesToEncrypt.ToLower() + ",";
            string[] fieldTokens = encryptFieldList.Split(new char[1] {','}, StringSplitOptions.RemoveEmptyEntries);            

            foreach(string fieldName in fieldTokens)
            {
                // Encrypt the field if in list
                if (encryptFieldList.Contains("," + fieldName.ToLower() + ","))
                {
                    object val = string.Empty;
                    try
                    {
                       val = ReflectionUtils.GetPropertyEx(config, fieldName);
                    }
                    catch
                    {
                        throw new ArgumentException(string.Format("{0}: {1}",Resources.InvalidEncryptionPropertyName,fieldName));
                    }

                    // only encrypt string values
                    var strVal = val as string;
                    if (string.IsNullOrEmpty(strVal))
                        continue;

                    val = Encryption.EncryptString(strVal, EncryptionKey);
                    try
                    {
                        ReflectionUtils.SetPropertyEx(config, fieldName, val);
                    }
                    catch
                    {
                        throw new ArgumentException(string.Format("{0}: {1}", Resources.InvalidEncryptionPropertyName, fieldName));
                    }
                }
            }
        }
        public virtual void DecryptFields(AppConfiguration config)
        {
            if (string.IsNullOrEmpty(PropertiesToEncrypt))
                return;

            string encryptFieldList = "," + PropertiesToEncrypt.ToLower() + ",";
            string[] fieldTokens = encryptFieldList.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string fieldName in fieldTokens)
            {
                // Encrypt the field if in list
                if (encryptFieldList.Contains("," + fieldName.ToLower() + ","))
                {
                    object val = string.Empty;
                    try
                    {
                        val = ReflectionUtils.GetPropertyEx(config, fieldName);
                    }
                    catch
                    {
                        throw new ArgumentException(string.Format("{0}: {1}", Resources.InvalidEncryptionPropertyName, fieldName));
                    }

                    // only encrypt string values
                    var strVal = val as string;
                    if (string.IsNullOrEmpty(strVal))
                        continue;

                    val = Encryption.DecryptString(strVal, EncryptionKey);
                    try
                    {
                        ReflectionUtils.SetPropertyEx(config, fieldName, val);
                    }
                    catch
                    {
                        throw new ArgumentException(string.Format("{0}: {1}", Resources.InvalidEncryptionPropertyName, fieldName));
                    }
                }
            }
        }
        protected virtual void SetError(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                ErrorMessage = string.Empty;
                return;
            }

            ErrorMessage = message;
        }
        protected virtual void SetError(Exception ex)
        {
            string message = ex.Message;
            if (ex.InnerException != null)
                message += " " + ex.InnerException.Message;
            SetError(message);
        }
        protected TAppConfiguration CreateConfigurationInstance()
        {
            return Activator.CreateInstance(typeof(TAppConfiguration)) as TAppConfiguration;
        }
    }

}
