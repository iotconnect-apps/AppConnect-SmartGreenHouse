using System.Data;
using System.Data.Common;

namespace component.logger.data.log.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="con">The con.</param>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection con, string procedureName, CommandType commandType)
        {
            DbCommand cmd = con.CreateCommand();
            cmd.CommandText = procedureName;
            cmd.CommandType = commandType;
            return cmd;
        }
    }
}
