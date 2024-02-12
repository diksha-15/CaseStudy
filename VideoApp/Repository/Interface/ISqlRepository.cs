using System;
using System.Data.SqlClient;
using System.Data;



namespace Video_Streaming.Repository.Interface
{
    public interface ISqlRepository
    {
        DataTable ExecuteQuery( string query);
        DataTable ExecuteParameterizedQuery(string query, SqlParameter parameter);
        int ExecuteQuery(string query, SqlParameterCollection parameters);
    }
}
