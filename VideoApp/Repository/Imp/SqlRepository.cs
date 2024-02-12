using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Video_Streaming.DbEntity;
using Video_Streaming.Repository.Interface;

namespace Video_Streaming.Repository.Imp
{
    public class SqlRepository : ISqlRepository
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.ToString();

        public DataTable ExecuteParameterizedQuery( string query, SqlParameter parameter)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = new SqlCommand(query, connection);
                        adapter.SelectCommand.Parameters.Add(parameter);

                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
                catch (SqlException ex)
                {
                    Log.Error("SqlException Error happened while executing command {0}", ex);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error("Error happened while executing command {0}", ex);
                    throw;
                }
            }
        }

        public DataTable ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = new SqlCommand(query, connection);
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
                catch (SqlException ex)
                {
                    Log.Error("SqlException Error happened while executing command {0}", ex);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error("Error happened while executing command {0}", ex);
                    throw;
                }
            }
        }

        public static List<T> MapDataTableToClass<T>(DataTable dataTable) where T : new()
        {
            List<T> resultList = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                T item = new T();

                var properties = typeof(T).GetProperties();

                foreach (DataColumn column in dataTable.Columns)
                {
                    var propertyName = column.ColumnName;

                    var property = properties.FirstOrDefault(p => p.Name == propertyName);

                    if (property != null && row[column] != DBNull.Value)
                    {
                        var value = Convert.ChangeType(row[column], property.PropertyType);
                        property.SetValue(item, value);
                    }
                }

                resultList.Add(item);
            }

            return resultList;
        }

        public int ExecuteQuery(string query, SqlParameterCollection parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(new SqlParameter(parameter.ParameterName, parameter.Value));
                        }

                        int affectedRows = command.ExecuteNonQuery();
                        return affectedRows;
                    }
                }
                catch (SqlException ex)
                {
                    Log.Error("SqlException Error happened while executing command {0}", ex);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.Error("Error happened while executing command {0}", ex);
                    throw;
                }
            }
        }




    }
}