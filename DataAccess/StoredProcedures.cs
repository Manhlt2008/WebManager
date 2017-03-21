using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public partial class StoredProcedures : CollectionBase
    {
        // Instance fields
        private DB _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedures"/> 
        /// class with the specified <see cref="DB"/>.
        /// </summary>
        /// <param name="db">The <see cref="DB"/> object.</param>
        public StoredProcedures(DB db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets the database object that this table belongs to.
        ///	</summary>
        ///	<value>The <see cref="DB"/> object.</value>
        protected DB Database
        {
            get { return _db; }
        }

        /// <summary>
        /// Mapping data reader to entity collection
        ///	</summary>
        /// <param name="command">The IDb command.</param>
        ///	<value>The entity list.</value>
        protected List<T> GetList<T>(IDbCommand command)
        {
            using (IDataReader reader = _db.ExecuteReader(command))
            {
                return MapRecords<T>(reader);
            }
        }

        /// <summary>
        /// Mapping data reader to entity collection
        ///	</summary>
        /// <param name="command">The IDb command.</param>
        ///	<value>The ReturnValue in command.</value>
        protected int ExecuteInt32(IDbCommand command)
        {
            SqlParameter returnValue = new SqlParameter("@ReturnValue", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;
            command.Parameters.Add(returnValue);
            command.ExecuteNonQuery();
            return returnValue.Value != null ? (int)returnValue.Value : -1;
        }

        protected object ExecuteScalar(IDbCommand command)
        {
            var returnValue = command.ExecuteScalar();
            return returnValue;
        }
    }
}
