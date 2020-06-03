using component.logger.data.log.Helper;
using component.logger.data.log.Model;
using component.logger.data.log.Request;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace component.logger.data.log.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TK">The type of the k.</typeparam>
    public abstract class BaseRepository<T, TK>
            where T : class
    {
        #region Variables
        /// <summary>
        /// The connection
        /// </summary>
        protected SqlConnection connection;

        /// <summary>
        /// The database command
        /// </summary>
        protected DbCommand dbCommand;

        /// <summary>
        /// Gets or sets the invoking user identifier.
        /// </summary>
        /// <value>
        /// The invoking user identifier.
        /// </value>
        public Guid InvokingUserId { get; set; }


        /// <summary>
        /// Gets or sets the application code.
        /// </summary>
        /// <value>
        /// The application code.
        /// </value>
        public string ApplicationCode { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T, TK}"/> class.
        /// </summary>
        /// <param name="sqlConnectionString">The SQL connection string.</param>
        protected BaseRepository(string sqlConnectionString)
        {
            if (!string.IsNullOrWhiteSpace(sqlConnectionString))
            {
                ConnectionString = sqlConnectionString;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T, TK}"/> class.
        /// </summary>
        /// <param name="invokingUserId">The invoking user identifier.</param>
        /// <param name="applicationCode">The application code.</param>
        /// <param name="version">The version.</param>
        /// <param name="sqlConnectionString">The SQL connection string.</param>
        protected BaseRepository(Guid invokingUserId, string applicationCode, string version, string sqlConnectionString)
            : this(invokingUserId, version, sqlConnectionString)
        {
            ApplicationCode = applicationCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository{T, TK}"/> class.
        /// </summary>
        /// <param name="invokingUserId">The invoking user identifier.</param>
        /// <param name="version">The version.</param>
        /// <param name="sqlConnectionString">The SQL connection string.</param>
        public BaseRepository(Guid invokingUserId, string version, string sqlConnectionString)
            : this(sqlConnectionString)
        {
            InvokingUserId = invokingUserId;
            Version = version;
        }
        #endregion

        #region Method
        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual DataResponse<TK> Add(T value)
        {
            return null;
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual DataResponse<TK> Delete(TK id)
        {
            return null;
        }

        /// <summary>
        /// Deletes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual DataResponse<string> Delete(T value)
        {
            return null;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual DataResponse<T> Get(TK id)
        {
            return null;
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public virtual DataResponse<List<T>> GetList(ListRequest request)
        {
            return null;
        }

        /// <summary>
        /// Updates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual DataResponse<TK> Update(T value)
        {
            return null;
        }

        #endregion

        #region CommonMethod

        /// <summary>
        /// Converts the enum.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static K ConvertEnum<K>(object value)
        {
            return (K)System.Enum.Parse(typeof(K), value.ToString());
        }
        #endregion        

        #region sql queries extentions

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        protected string ConnectionString { get; private set; }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        private SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(this.ConnectionString);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return connection;
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        protected int ExecuteNonQuery(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            int returnValue = -1;

            try
            {
                using (DbConnection connection = this.GetConnection())
                {
                    DbCommand cmd = connection.CreateCommand(procedureName, commandType);

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    returnValue = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return returnValue;
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        protected object ExecuteScalar(string procedureName, List<DbParameter> parameters)
        {
            object returnValue = null;

            try
            {
                using (DbConnection connection = this.GetConnection())
                {
                    DbCommand cmd = connection.CreateCommand(procedureName, CommandType.StoredProcedure);
                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    returnValue = cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return returnValue;
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="readData">The read data.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="commandType">Type of the command.</param>
        protected void ExecuteReader(Action<DbDataReader> readData, string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            DbDataReader dr = null;
            using (DbConnection connection = this.GetConnection())
            {
                DbCommand cmd = connection.CreateCommand(procedureName, commandType);
                if (parameters != null && parameters.Count > 0)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                readData?.Invoke(dr);
                if (!dr.IsClosed)
                {
                    connection.Close();
                }
            }
        }
        #endregion
    }
}
