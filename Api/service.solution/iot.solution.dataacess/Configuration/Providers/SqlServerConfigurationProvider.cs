using iot.solution.data.Utilities;
using System;
using System.Data.Common;
using System.Data.SqlClient;
namespace iot.solution.data.Configuration
{
    public class SqlServerConfigurationProvider<TAppConfiguration> : ConfigurationProviderBase<TAppConfiguration>
        where TAppConfiguration : AppConfiguration, new()
    {
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }
        private string _ConnectionString = string.Empty;

        public DbProviderFactory ProviderFactory { get; set; } = SqlClientFactory.Instance;
        public string Tablename
        {
            get { return _Tablename; }
            set { _Tablename = value; }
        }
        private string _Tablename = "ConfigurationSettings";
        
        public int Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
        private int _Key = 1;

        public override T Read<T>()
        {
			using (SqlDataAccess data = new SqlDataAccess(ConnectionString, ProviderFactory))
			{
                string sql = "select * from [" + Tablename + "] where id=" + Key.ToString();

                DbDataReader reader = null;
                try
                {
                    DbCommand command = data.CreateCommand(sql);
                    if (command == null)
                    {
                        SetError(data.ErrorMessage);
                        return null;
                    }
                    reader = command.ExecuteReader();
                    if (reader == null)
                    {
                        SetError(data.ErrorMessage);
                        return null;
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 208)
                    {

                        sql =
    @"CREATE TABLE [" + Tablename + @"]  
( [id] [int] , [ConfigData] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS)";
                        try
                        {
                            data.ExecuteNonQuery(sql);
                        }
                        catch
                        {
                            return null;
                        }

                        // try again if we were able to create the table 
                        return Read<T>();
                    }

                }
                catch (DbException dbEx)
                {
                    // SQL CE Table doesn't exist
                    if (dbEx.ErrorCode == -2147467259)
                    {
                        sql = String.Format(
                            @"CREATE TABLE [{0}] ( [id] [int] , [ConfigData] [ntext] )",
                            Tablename);
                        try
                        {
                            data.ExecuteNonQuery(sql);
                        }
                        catch
                        {
                            return null;
                        }

                        // try again if we were able to create the table 
                        var inst = Read<T>();

                        // if we got it write it to the db
                        Write(inst);

                        return inst;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    this.SetError(ex);
                    
                    if (reader != null)
                        reader.Close();
                    
                    data.CloseConnection();
                    return null;
                }


                string xmlConfig = null;

                if (reader.Read())
                    xmlConfig = (string)reader["ConfigData"];

                reader.Close();
                data.CloseConnection();

                if (string.IsNullOrEmpty(xmlConfig))
                {
                    T newInstance = new T();
                    newInstance.Provider = this;
                    return newInstance;
                }

                T instance = Read<T>(xmlConfig);

                return instance;
            }
        }
        public override bool Read(AppConfiguration config)
        {
            TAppConfiguration newConfig = Read<TAppConfiguration>();
            if (newConfig == null)
                return false;

            DataUtils.CopyObjectData(newConfig, config,"Provider,ErrorMessage");
            return true;
        }
        public override bool Write(AppConfiguration config)
        {
			SqlDataAccess data = new SqlDataAccess(ConnectionString, SqlClientFactory.Instance);

            string sql = String.Format(
                "Update [{0}] set ConfigData=@ConfigData where id={1}", 
                Tablename, Key);
            
            string xml = WriteAsString(config);

            int result = 0;
            try
            {
                result = data.ExecuteNonQuery(sql, data.CreateParameter("@ConfigData", xml));
            }
            catch
            {
                result = -1;
            }

            // try to create the table
            if (result == -1)
            {
                sql = String.Format(
            @"CREATE TABLE [{0}] ( [id] [int] , [ConfigData] [ntext] )",
            Tablename);
                try
                {
                    result = data.ExecuteNonQuery(sql);
                    if (result > -1)
                        result = 0;
                }
                catch (Exception ex)
                {
                    SetError(ex);
                    return false;
                }
            }

            // Check for missing record
            if (result == 0)
            {
                sql = "Insert [" + Tablename + "] (id,configdata) values (" + Key.ToString() + ",@ConfigData)";

                try
                {
                    result = data.ExecuteNonQuery(sql, data.CreateParameter("@ConfigData", xml));
                }
                catch (Exception ex)
                {
                    SetError(ex);
                    return false;
                }
                if (result == 0)
                {                   
                    return false;
                }
            }

            if (result < 0)
                return false;
            
            return true;
        }
    }
}
