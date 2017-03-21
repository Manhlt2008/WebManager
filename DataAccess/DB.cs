#define SQL_CLIENT
using System.Collections.Generic;
using System.Data;

namespace DataAccess
{
    /// <summary>
    /// Represents a connection to the <c>DB</c> database.
    /// </summary>
    /// <remarks>
    /// If the <c>DB</c> goes out of scope, the connection to the 
    /// database is not closed automatically. Therefore, you must explicitly close the 
    /// connection by calling the <c>Close</c> or <c>Dispose</c> method.
    /// </remarks>
    /// <example>
    /// using(DB db = new DB())
    /// {
    ///		AboutGroupsRow[] rows = db.AboutGroupsTable.GetAll();
    /// }
    /// </example>
    public class DB : DBBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DB"/> class.
        /// </summary>
        public DB(bool isMaster = false)
            : base(isMaster)
        {
        }

        /// <summary>
        /// Creates a new connection to the database.
        /// </summary>
        /// <returns>An <see cref="System.Data.IDbConnection"/> object.</returns>
        protected override IDbConnection CreateConnection()
        {
#if ODBC
			return new System.Data.Odbc.OdbcConnection("INSERT ODBC CONNECTION STRING");
#elif SQL_CLIENT
            string strConn = System.Configuration.ConfigurationManager.AppSettings["DBSlave"];
			return new System.Data.SqlClient.SqlConnection(strConn);
				//"Integrated Security=SSPI;Initial Catalog=Adsoft;Data Source=FSERV" +
				//"ER");
#else
            return new System.Data.OleDb.OleDbConnection(
                "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Inf" +
                "o=False;Initial Catalog=Adsoft;Data Source=FSERVER");
#endif
        }

        /// <summary>
        /// Creates a new connection to the database.
        /// </summary>
        /// <returns>An <see cref="System.Data.IDbConnection"/> object.</returns>
        protected override IDbConnection CreateMasterConnection()
        {
#if ODBC
			return new System.Data.Odbc.OdbcConnection("INSERT ODBC CONNECTION STRING");
#elif SQL_CLIENT
            string strConn = System.Configuration.ConfigurationManager.AppSettings["DBMaster"];
            return new System.Data.SqlClient.SqlConnection(strConn);
            //"Integrated Security=SSPI;Initial Catalog=Adsoft;Data Source=FSERV" +
            //"ER");
#else
            return new System.Data.OleDb.OleDbConnection(
                "Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Inf" +
                "o=False;Initial Catalog=Adsoft;Data Source=FSERVER");
#endif
        }

        /// <summary>
        /// Creates a DataTable object for the specified command.
        /// </summary>
        /// <param name="command">The <see cref="System.Data.IDbCommand"/> object.</param>
        /// <returns>A reference to the <see cref="System.Data.DataTable"/> object.</returns>
        protected internal DataTable CreateDataTable(IDbCommand command)
        {
            DataTable dataTable = new DataTable();
#if ODBC
			new System.Data.Odbc.OdbcDataAdapter((System.Data.Odbc.OdbcCommand)command).Fill(dataTable);
#elif SQL_CLIENT
			new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)command).Fill(dataTable);
#else
            new System.Data.OleDb.OleDbDataAdapter((System.Data.OleDb.OleDbCommand)command).Fill(dataTable);
#endif
            return dataTable;
        }

        /// <summary>
        /// Creates a DataSet object for the specified command.
        /// </summary>
        /// <param name="command">The <see cref="System.Data.IDbCommand"/> object.</param>
        /// <returns>A reference to the <see cref="System.Data.DataTable"/> object.</returns>
        protected internal DataSet CreateDataSet(IDbCommand command)
        {
            DataSet dataSet = new DataSet();
#if ODBC
			new System.Data.Odbc.OdbcDataAdapter((System.Data.Odbc.OdbcCommand)command).Fill(dataSet);
#elif SQL_CLIENT
			new System.Data.SqlClient.SqlDataAdapter((System.Data.SqlClient.SqlCommand)command).Fill(dataSet);
#else
            new System.Data.OleDb.OleDbDataAdapter((System.Data.OleDb.OleDbCommand)command).Fill(dataSet);
#endif
            return dataSet;
        }

        /// <summary>
        /// Returns a SQL statement parameter name that is specific for the data provider.
        /// For example it returns ? for OleDb provider, or @paramName for MS SQL provider.
        /// </summary>
        /// <param name="paramName">The data provider neutral SQL parameter name.</param>
        /// <returns>The SQL statement parameter name.</returns>
        protected internal override string CreateSqlParameterName(string paramName)
        {
#if ODBC
			return "?";
#elif SQL_CLIENT
			return "@" + paramName;
#else
            return "?";
#endif
        }

        /// <summary>
        /// Creates a .Net data provider specific parameter name that is used to
        /// create a parameter object and add it to the parameter collection of
        /// <see cref="System.Data.IDbCommand"/>.
        /// </summary>
        /// <param name="baseParamName">The base name of the parameter.</param>
        /// <returns>The full data provider specific parameter name.</returns>
        protected override string CreateCollectionParameterName(string baseParamName)
        {
#if ODBC
			return "@" + baseParamName;
#else
            return "@" + baseParamName;
#endif
        }

        #region


        /// <summary>
        /// Fill a table of database to DataTable
        /// <param name="strQuery">Query string for get data</param>
        /// </summary>
        /// <returns>All content of Table</returns>
        public virtual DataTable FillDataTable(string strQuery)
        {
            //IDbConnection Conn = this.CreateConnection();
            return this.CreateDataTable(this.CreateCommand(strQuery));
        }

        /// <summary>
        /// Fill tables of database to DataSet
        /// <param name="strQuery">Query string for get data</param>
        /// </summary>
        /// <returns>DataSet with tables to got</returns>
        public virtual DataSet FillDataSet(string strQuery)
        {
            //IDbConnection Conn = this.CreateConnection();
            return this.CreateDataSet(this.CreateCommand(strQuery));
        }

        public void ExecuteNoneQuery(string strQuery)
        {
            this.CreateCommand(strQuery).ExecuteNonQuery();
        }

        /// <summary>
        /// Fill a table of database to DataTable
        /// <param name="strQuery">Query string for get data</param>
        /// </summary>
        /// <returns>All content of Table</returns>
        public virtual List<T> ExecuteList<T>(string strQuery)
        {
            using (IDataReader reader = this.ExecuteReader(CreateCommand(strQuery)))
            {
                return MapRecords<T>(reader);
            }
        }
        #endregion
    } // End of DB class
}
