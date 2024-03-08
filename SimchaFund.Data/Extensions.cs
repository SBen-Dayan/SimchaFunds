using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public static class Extensions
    {
        public static T GetValue<T>(this SqlDataReader reader, string columnName)
        {
            var value = reader[columnName];
            if (value == DBNull.Value)
            {
                return default;
            }
            return (T)value;
        }

        public static void AddRangeWithValues(this SqlParameterCollection collection, Dictionary<string, object> parameters)
        {
            foreach (var (key, value) in parameters)
            {
                collection.AddWithValue(key, value);
            }
        }
    }
}
