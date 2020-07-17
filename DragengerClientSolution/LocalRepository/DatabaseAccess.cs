using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using System.Configuration;
using System.Data.SqlServerCe;
using FileIOAccess;
using System.Windows.Forms;
using ResourceLibrary;

namespace LocalRepository
{
    public class DatabaseAccess : IDisposable
    {
        private SqlCeConnection connection;
        public static string LocalDbPassword { set; private get; }
        public DatabaseAccess()
        {
            this.connection = new SqlCeConnection(this.ConnectionString);
        }

        private void OpenConnection()
        {
            try
            {
                try { this.connection.Close(); } catch { }
                this.connection.Open();
            }
            catch
            {
                //Console.WriteLine(ex.Message);
                ResetUserData();
                this.connection = new SqlCeConnection(this.ConnectionString);
                this.connection.Open();
            }
        }

        private string ConnectionString
        {
            get { return "Data Source=" + FileResources.LocalDatabasePath + "; Password=" + LocalDbPassword + ";"; }
        }

        private void ResetUserData()
        {
            //testing
            LocalDbPassword = "";
            //
            LocalDataFileAccess.ResetUserData();
            string usersDbPassword = DatabaseAccess.LocalDbPassword;
            DatabaseAccess.LocalDbPassword = null;
            this.ChangeLocalDataBasePassword(usersDbPassword);
        }

        public bool ChangeLocalDataBasePassword(string password)
        {
            //testing
            password = "";
            //
            try
            {
                SqlCeEngine engine = new SqlCeEngine(this.ConnectionString);
                engine.Compact(null);
                string operationConnectionString = "Data Source=; Password=" + password + ";";
                engine.Compact(operationConnectionString);
                DatabaseAccess.LocalDbPassword = password;
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception in ChangeLocalDataBasePassword() => " + ex.Message);
                return false;
            }
        }

        public void VerifyUserDataValidity()
        {
            this.OpenConnection();
            this.CloseConnection();
        }

        internal SqlCeDataReader ReadSqlCeData(string query)
        {
            try
            {
                this.OpenConnection();
                SqlCeCommand command = new SqlCeCommand(query);
                command.Connection = this.connection;
                SqlCeDataReader data = command.ExecuteReader();
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + query);
                return null;
            }
        }

        internal int? ExecuteSqlCeQuery(string query)
        {
            try
            {
                this.OpenConnection();
                SqlCeCommand command = new SqlCeCommand(query, this.connection);
                return command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + query);
                return null;
            }
        }

        internal string ExecuteSqlCeScalar(string query)
        {
            try
            {
                this.OpenConnection();
                SqlCeCommand command = new SqlCeCommand(query, this.connection);
                return command.ExecuteScalar() + "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace + "\n" + query);
                return null;
            }
        }

        internal object ReadSqlCeScalar(string query)
        {
            try
            {
                this.OpenConnection();
                SqlCeCommand command = new SqlCeCommand(query, this.connection);
                return command.ExecuteScalar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace + "\n" + query);
                return null;
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

    }
}


