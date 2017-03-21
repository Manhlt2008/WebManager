using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccess
{
    /// <summary>
    /// Data parameter to query.
    /// </summary>
    public class DataParameter
    {
        public string ParameterName { get; set; }

        public object Value { get; set; }
    }

    public class CollectionBase
    {
        protected List<T> MapRecords<T>(IDataReader reader,
                                int startIndex, int length, ref int totalRecordCount)
        {
            var recordList = new List<T>();
            int ri = -startIndex;
            var builder = DynamicBuilder<T>.CreateBuilder(reader);
            while (reader.Read())
            {
                ri++;
                if (ri > 0 && ri <= length)
                {
                    var record = builder.Build(reader);
                    recordList.Add(record);

                    if (ri == length && 0 != totalRecordCount)
                        break;
                }
            }
            ClearInputtedData();

            totalRecordCount = 0 == totalRecordCount ? ri + startIndex : -1;
            return recordList;
        }

        protected List<T> MapRecords<T>(IDataReader reader)
        {
            var recordList = new List<T>();
            var builder = DynamicBuilder<T>.CreateBuilder(reader);
            while (reader.Read())
            {
                var record = builder.Build(reader);
                recordList.Add(record);
            }
            ClearInputtedData();

            return recordList;
        }

        protected string GetListFields<T>()
        {
            Type type = typeof(T);
            var sbFields = new StringBuilder();
            foreach (var property in type.GetProperties())
            {
                sbFields.AppendFormat(",{0}", property.Name);
            }

            return sbFields.ToString().TrimStart(',');
        }

        protected string GetListFields(string[] listFields)
        {
            // If listFields null or empty then return all(*) for select.
            if (listFields == null || listFields.Length == 0)
            {
                return "*";
            }

            var sbFields = new StringBuilder();
            for (int i = 0; i < listFields.Length; i++)
            {
                sbFields.AppendFormat(",{0}", listFields[i]);
            }

            return sbFields.ToString().TrimStart(',');
        }

        protected string GetListFields<T>(string[] listFields)
        {
            if (listFields != null)
            {
                return GetListFields(listFields);
            }

            return GetListFields<T>();
        }

        /// <summary>
        /// Clear input data
        /// </summary>
        protected virtual void ClearInputtedData()
        {
            // Do nothing
        }
    }
}
