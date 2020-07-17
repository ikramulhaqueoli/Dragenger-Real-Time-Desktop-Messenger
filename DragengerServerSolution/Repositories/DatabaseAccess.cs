using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using System.Configuration;
using System.Data.SqlClient;
using Display;
namespace Repositories
{
    public class DatabaseAccess : IDisposable
    {
        private SqlConnection connection;

        public DatabaseAccess()
        {
            connection = new SqlConnection(ConfigurationManager.ConnectionStrings["globalDB"].ConnectionString);
        }

        public SqlDataReader ReadSqlData(string query)
        {
            try
            {
                try { this.connection.Open(); } 
                catch { }
                SqlCommand command = new SqlCommand(query);
                command.Connection = this.connection;
                SqlDataReader data = command.ExecuteReader();
                return data;
            }
            catch (Exception e)
            {
                Output.ShowLog(e.Message + "\n" + query);
                return null;
            }
        }

        public int? ExecuteSqlQuery(string query)
        {
            try
            {
                try { this.connection.Open(); }
                catch { }
                SqlCommand command = new SqlCommand(query);
                command.Connection = this.connection;
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Output.ShowLog(e.Message + "\n" + query);
                return null;
            }
        }

        public string ExecuteSqlQueryAndGiveResultString(string query)
        {
            try
            {
                try { this.connection.Open(); }
                catch { }
                SqlCommand command = new SqlCommand(query);
                command.Connection = this.connection;
                return "Affected rows: " + command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return "Error: " + e.Message;
            }
        }

        public string ExecuteSqlScalar(string query)
        {
            try
            {
                try { this.connection.Open(); }
                catch { }
                SqlCommand command = new SqlCommand(query, this.connection);
                string result = command.ExecuteScalar() + "";
                return result;
            }
            catch (Exception e)
            {
                Output.ShowLog(e.Message + "\n" + query);
                return null;
            }
        }

        public string ExecuteSqlScalarAndGiveResultString(string query)
        {
            try
            {
                try { this.connection.Open(); }
                catch { }
                SqlCommand command = new SqlCommand(query, this.connection);
                string result = command.ExecuteScalar() + "";
                return result;
            }
            catch (Exception e)
            {
                return "Error: " + e.Message;
            }
        }

        void IDisposable.Dispose()
        {
            try
            {
                this.CloseConnection();
            }
            catch { }
        }

        public void Dispose()
        {
            try
            {
                this.CloseConnection();
            }
            catch { }
        }

        protected void CloseConnection()
        {
            this.connection.Close();
        }

        public static DatabaseAccess Instance
        {
            get { return new DatabaseAccess(); }
        }


        public bool AdaptSqlQuery(string query, System.Data.DataSet dataTable)
        {
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, this.connection);
                dataAdapter.Fill(dataTable);
                return true;
            }
            catch(Exception e)
            {
                Output.Error("Error executing the sql query!" + "\n\r" + query + "\r\nError message: " + e.Message);
                return false;
            }
        }
    }
}


