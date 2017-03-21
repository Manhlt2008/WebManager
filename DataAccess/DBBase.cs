using System;
using System.Data;

namespace DataAccess
{
    /// <summary>
    /// The base class for the <see cref="DB"/> class that 
    /// represents a connection to the <c>DB</c> database. 
    /// </summary>
    /// <remarks>
    /// Do not change this source code. Modify the DB class
    /// if you need to add or change some functionality.
    /// </remarks>
    public abstract class DBBase : CollectionBase, IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        // Stored procedure
        private StoredProcedures _storedProcedures;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBBase"/> 
        /// class and opens the database connection.
        /// </summary>
        protected DBBase(bool isMaster = false)
        {
            if (isMaster)
            {
                InitMasterConnection();
            }
            else
            {
                InitConnection();
            }
        }

        /// <summary>
        /// Initializes the database connection.
        /// </summary>
        protected void InitConnection()
        {
            _connection = CreateConnection();
            _connection.Open();
        }

        /// <summary>
        /// Creates a new connection to the database.
        /// </summary>
        /// <returns>A reference to the <see cref="System.Data.IDbConnection"/> object.</returns>
        protected abstract IDbConnection CreateConnection();

        /// <summary>
        /// Initializes the database connection.
        /// </summary>
        protected void InitMasterConnection()
        {
            _connection = CreateMasterConnection();
            _connection.Open();
        }

        /// <summary>
        /// Creates a new connection to the database.
        /// </summary>
        /// <returns>A reference to the <see cref="System.Data.IDbConnection"/> object.</returns>
        protected abstract IDbConnection CreateMasterConnection();

        /// <summary>
        /// Returns a SQL statement parameter name that is specific for the data provider.
        /// For example it returns ? for OleDb provider, or @paramName for MS SQL provider.
        /// </summary>
        /// <param name="paramName">The data provider neutral SQL parameter name.</param>
        /// <returns>The SQL statement parameter name.</returns>
        protected internal abstract string CreateSqlParameterName(string paramName);

        /// <summary>
        /// Creates <see cref="System.Data.IDataReader"/> for the specified DB command.
        /// </summary>
        /// <param name="command">The <see cref="System.Data.IDbCommand"/> object.</param>
        /// <returns>A reference to the <see cref="System.Data.IDataReader"/> object.</returns>
        protected internal virtual IDataReader ExecuteReader(IDbCommand command)
        {
            return command.ExecuteReader();
        }

        /// <summary>
        /// Adds a new parameter to the specified command. It is not recommended that 
        /// you use this method directly from your custom code. Instead use the 
        /// <c>AddParameter</c> method of the DBBase classes.
        /// </summary>
        /// <param name="cmd">The <see cref="System.Data.IDbCommand"/> object to add the parameter to.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="System.Data.DbType"/> values. </param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>A reference to the added parameter.</returns>
        internal IDbDataParameter AddParameter(IDbCommand cmd, string paramName,
                                                DbType dbType, object value)
        {
            IDbDataParameter parameter = cmd.CreateParameter();
            parameter.ParameterName = CreateCollectionParameterName(paramName);
            parameter.DbType = dbType;
            parameter.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Adds a new parameter to the specified command. It is not recommended that 
        /// you use this method directly from your custom code. Instead use the 
        /// <c>AddParameter</c> method of the DBBase classes.
        /// </summary>
        /// <param name="cmd">The <see cref="System.Data.IDbCommand"/> object to add the parameter to.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="System.Data.DbType"/> values. </param>
        /// <param name="value">The value of the parameter.</param>
        /// <param name="paraDirection">The direction of the parameter.</param>
        /// <returns>A reference to the added parameter.</returns>
        internal IDbDataParameter AddParameter(IDbCommand cmd, string paramName,
                                                DbType dbType, object value, ParameterDirection paraDirection)
        {
            IDbDataParameter parameter = cmd.CreateParameter();
            parameter.ParameterName = CreateCollectionParameterName(paramName);
            parameter.DbType = dbType;
            parameter.Value = value ?? DBNull.Value;
            parameter.Direction = paraDirection;
            cmd.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Creates a .Net data provider specific name that is used by the 
        /// <see>
        ///     <cref>AddParameter</cref>
        /// </see>
        ///     method.
        /// </summary>
        /// <param name="baseParamName">The base name of the parameter.</param>
        /// <returns>The full data provider specific parameter name.</returns>
        protected abstract string CreateCollectionParameterName(string baseParamName);

        /// <summary>
        /// Gets <see cref="System.Data.IDbConnection"/> associated with this object.
        /// </summary>
        /// <value>A reference to the <see cref="System.Data.IDbConnection"/> object.</value>
        public IDbConnection Connection
        {
            get { return _connection; }
        }

        /// <summary>
        /// Gets an object that represents the <c>StoredProcedures</c> table.
        /// </summary>
        /// <value>A reference to the <see cref="StoredProcedures"/> object.</value>
        public StoredProcedures StoredProcedures
        {
            get
            {
                if (null == _storedProcedures)
                    _storedProcedures = new StoredProcedures((DB)this);
                return _storedProcedures;
            }
        }

        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        /// <seealso cref="CommitTransaction"/>
        /// <seealso cref="RollbackTransaction"/>
        /// <returns>An object representing the new transaction.</returns>
        public IDbTransaction BeginTransaction()
        {
            CheckTransactionState(false);
            _transaction = _connection.BeginTransaction();
            return _transaction;
        }

        /// <summary>
        /// Begins a new database transaction with the specified 
        /// transaction isolation level.
        /// <seealso cref="CommitTransaction"/>
        /// <seealso cref="RollbackTransaction"/>
        /// </summary>
        /// <param name="isolationLevel">The transaction isolation level.</param>
        /// <returns>An object representing the new transaction.</returns>
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            CheckTransactionState(false);
            _transaction = _connection.BeginTransaction(isolationLevel);
            return _transaction;
        }

        /// <summary>
        /// Commits the current database transaction.
        /// <seealso>
        ///     <cref>BeginTransaction</cref>
        /// </seealso>
        ///     <seealso cref="RollbackTransaction"/>
        /// </summary>
        public void CommitTransaction()
        {
            CheckTransactionState(true);
            _transaction.Commit();
            _transaction = null;
        }

        /// <summary>
        /// Rolls back the current transaction from a pending state.
        /// <seealso>
        ///     <cref>BeginTransaction</cref>
        /// </seealso>
        ///     <seealso cref="CommitTransaction"/>
        /// </summary>
        public void RollbackTransaction()
        {
            CheckTransactionState(true);
            _transaction.Rollback();
            _transaction = null;
        }

        // Checks the state of the current transaction
        private void CheckTransactionState(bool mustBeOpen)
        {
            if (mustBeOpen)
            {
                if (null == _transaction)
                    throw new InvalidOperationException("Transaction is not open.");
            }
            else
            {
                if (null != _transaction)
                    throw new InvalidOperationException("Transaction is already open.");
            }
        }

        /// <summary>
        /// Creates and returns a new <see cref="System.Data.IDbCommand"/> object.
        /// </summary>
        /// <param name="sqlText">The text of the query.</param>
        /// <returns>An <see cref="System.Data.IDbCommand"/> object.</returns>
        internal IDbCommand CreateCommand(string sqlText)
        {
            return CreateCommand(sqlText, false);
        }

        /// <summary>
        /// Creates and returns a new <see cref="System.Data.IDbCommand"/> object.
        /// </summary>
        /// <param name="sqlText">The text of the query.</param>
        /// <param name="procedure">Specifies whether the sqlText parameter is 
        /// the name of a stored procedure.</param>
        /// <returns>An <see cref="System.Data.IDbCommand"/> object.</returns>
        internal IDbCommand CreateCommand(string sqlText, bool procedure)
        {
            IDbCommand cmd = _connection.CreateCommand();
            cmd.CommandText = sqlText;
            cmd.Transaction = _transaction;
            if (procedure)
                cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        /// <summary>
        /// Rolls back any pending transactions and closes the DB connection.
        /// An application can call the <c>Close</c> method more than
        /// one time without generating an exception.
        /// </summary>
        public virtual void Close()
        {
            if (null != _connection)
                _connection.Close();
        }

        /// <summary>
        /// Rolls back any pending transactions and closes the DB connection.
        /// </summary>
        public virtual void Dispose()
        {
            Close();
            if (null != _connection)
                _connection.Dispose();
        }
    } // End of DBBase class
}
