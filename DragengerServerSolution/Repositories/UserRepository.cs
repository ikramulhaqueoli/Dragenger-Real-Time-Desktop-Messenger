using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLibrary;
using Interfaces;
using System.Data.SqlClient;
using ResourceLibrary;

namespace Repositories
{
    public class UserRepository : DatabaseAccess, IUserRepository
    {
        public long? Insert(User user)
        {
            string sql = "INSERT INTO Users (Username,User_type) output Inserted.Id values ('" + user.Username+"','"+user.AccountType+"')";
            return long.Parse(this.ExecuteSqlScalar(sql));
        }

        public bool? Update(User item)
        {
            throw new NotImplementedException();
        }

        public bool? Delete(long primarykey)
        {
            throw new NotImplementedException();
        }

        public User Get(long primarykey)
        {
            throw new NotImplementedException();
        }

        public string UserLastActiveTime(long userId)
        {
            string query = "SELECT Last_Active FROM Users WHERE Last_Active IS NOT NULL and Id = " + userId;
            SqlDataReader data = this.ReadSqlData(query);
            if (data.Read())
            {
                return data["Last_Active"].ToString();
            }
            return "";
        }

        public string SetUserActive(long userId)
        {
            int? resultValue = this.ExecuteSqlQuery("UPDATE Users SET Last_Active = 'active' where Id = " + userId);
            if (resultValue == null) return "Database connection failed";
            if (resultValue <= 0) return "Login ID not found";
            else return null;
        }

        public bool? SetUserLastActiveTimeStamp(long userId)
        {
            string query = "UPDATE Users set Last_Active = '" + Time.CurrentTime.TimeStampString + "' where Id = '" + userId + "' and Last_Active = 'active'";
            int? success = this.ExecuteSqlQuery(query);
            if (success == null) return null;
            return (success > 0);
        }

        public User GetByCredentials(string deviceMac, string password)
        {
            string uidQuery = "select User_Id from Devices_Bind_Map where Device_Mac = '" + deviceMac + "'";
            string userQuery = "select Id, User_type from Users where Id = (" + uidQuery + ") and Password = '" + password + "'";
            SqlDataReader data = this.ReadSqlData(userQuery);
            if (data == null) return null;
            while (data.Read())
            {
                User foundUser = null;
                long userId = long.Parse(data["Id"].ToString());
                string userType = data["User_type"].ToString();
                if (userType == "consumer")
                {
                    this.CloseConnection();
                    foundUser = ConsumerRepository.Instance.Get(userId);
                }
                return foundUser;
            }
            return null;
        }

        public bool? MacAddressExists(string deviceMac)
        {
            string query = "select Device_Mac from Devices_Bind_Map where Device_Mac = '" + deviceMac + "'";
            SqlDataReader data = this.ReadSqlData(query);
            if (data == null) return null;
            while (data.Read())
            {
                if (data.GetString(0) == deviceMac) return true;
            }
            return false;
        }
        public bool? UserNameAlreadyExists(string username)
        {
            SqlDataReader reader = this.ReadSqlData("SELECT Username FROM Users WHERE Username = '" + username + "'");
            if (reader == null) return null;
            return reader.Read();
        }

        public bool? BindMacAddressById(long userId, string macAddress)
        {
            string query = "INSERT INTO Devices_Bind_Map(User_id, Device_Mac) values(" + userId + ", '" + macAddress + "')";
            int? success = this.ExecuteSqlQuery(query);
            if (success == null) return null;
            if (success > 0) return true;
            return false;
        }

        public bool? BindMacAddressByUsername(string username, string macAddress)
        {
            string query = "INSERT INTO Devices_Bind_Map(User_id, Device_Mac) values((SELECT Id FROM Users WHERE Username = '" + username + "'), '" + macAddress + "')";
            int? success = this.ExecuteSqlQuery(query);
            if (success == null) return null;
            if (success > 0) return true;
            return false;
        }

        public long? GetUserIdByMacAddress(string macAddress)
        {
            string query = "SELECT User_Id from Devices_Bind_Map where Device_Mac = '" + macAddress + "'";
            SqlDataReader data = this.ReadSqlData(query);
            while (data.Read()) return long.Parse(data["User_Id"].ToString());
            return null;
        }

        public int? UserVerifiedOrPasswordIsSet(long userId)
        {
            string query = "IF((SELECT Verified FROM Users WHERE Id = " + userId + ") = 0)\n";
            query += "BEGIN SELECT '0' as result; END\n";
            query += "ELSE IF((SELECT Password FROM Users WHERE Id = " + userId + ") IS NULL) BEGIN SELECT '1' as result; END \n";
            query += "ELSE BEGIN SELECT '2' as result END\n";
            string result = this.ExecuteSqlScalar(query);
            try { return int.Parse(result); }
            catch { return null; }
        }

        public int VerifyVerificationCode(string deviceMac, string verificationCode, string purpose)
        {
            string sql = "";
            sql += " DECLARE @user_Id BIGINT; ";
            sql += " SELECT @user_Id = User_Id from Devices_Bind_Map where Device_Mac = '" + deviceMac + "';";
            sql += " IF((SELECT COUNT(Verification_Code) FROM Verification_Codes WHERE User_Id = @user_Id AND Purpose = '" + purpose + "' AND verification_Code = '" + verificationCode +"' AND DATEDIFF(minute, Assigned_Time, SYSDATETIME()) <= 60 AND Times_Checked < 5) > 0)";
            sql += " BEGIN";
            sql += " SELECT '1' AS result;";
            sql += " UPDATE Users SET Verified = 1 WHERE Id = @user_Id;";
            sql += " DELETE FROM Verification_Codes WHERE User_Id = @user_Id AND Purpose = '" + purpose + "';";
            sql += " END";
            sql += " ELSE";
            sql += " BEGIN";
            sql += " UPDATE Verification_Codes SET Times_Checked = (SELECT Times_Checked FROM Verification_Codes WHERE User_Id = @user_Id AND Purpose = '" + purpose + "')+1 WHERE User_Id = @user_Id AND Purpose = '" + purpose + "';";
            sql += " IF((SELECT COUNT(Verification_Code) FROM Verification_Codes WHERE Purpose = '" + purpose + "' AND User_Id = @user_id AND Times_Checked >= 5) > 0) BEGIN SELECT '4' AS result; END ";
            sql += " ELSE IF((SELECT COUNT(Verification_Code) FROM Verification_Codes WHERE Purpose = '" + purpose + "' AND User_Id = @user_id AND Verification_Code = '" + verificationCode + "' AND DATEDIFF(minute, Assigned_Time, SYSDATETIME()) > 60) > 0) BEGIN SELECT '3' AS result; END ";
            sql += " ELSE IF((SELECT COUNT(Verification_Code) FROM Verification_Codes WHERE Purpose = '" + purpose + "' AND User_Id = @user_id AND Verification_Code = '" + verificationCode + "') = 0) BEGIN SELECT '2' AS result; END ";
            sql += " ELSE BEGIN SELECT '0' AS result; END ";
            sql += " DELETE FROM Verification_Codes WHERE Times_Checked >= 5 OR DATEDIFF(minute, Assigned_Time, SYSDATETIME()) > 60;";
            sql += " END";
            string result = this.ExecuteSqlScalar(sql);
            //Display.Output.ShowLog(sql+ " " + result);
            try { return int.Parse(result); } catch{ return 0; };
            //1 means verification code is verified successfully
            //2 means verification code is not valid
            //3 means verification code is expried
            //4 means too many wrong (more than 5) attempts
        }

        public string AssignEmailVerificationCode(long userId, string purpose)
        {
            string generatedCode = Universal.RandomNumericString(6);
            string sql = " DELETE FROM Verification_Codes WHERE Times_Checked >= 5 OR DATEDIFF(minute, Assigned_Time, SYSDATETIME()) > 60; ";
            sql += " IF((SELECT COUNT(Verification_Code) FROM Verification_Codes WHERE user_Id = " + userId + " AND Purpose = '" + purpose + "') = 0) BEGIN INSERT INTO Verification_Codes (User_Id, Purpose, Verification_Code) VALUES (" + userId + ", '" + purpose + "', '" + generatedCode + "'); END \n";
            sql += " SELECT Verification_Code FROM Verification_Codes WHERE user_Id = " + userId + ";";
            return this.ExecuteSqlScalar(sql);
        }

        public string GetPassword(long userID)
        {
            SqlDataReader data = this.ReadSqlData("SELECT Password from Users where Id = " + userID);
            if (data == null) return null;
            if (data.Read())
            {
                return data["Password"].ToString();
            }
            return null;
        }

        public string SetPassword(string deviceMac, string password)
        {
            string uidQuery = "SELECT User_Id from Devices_Bind_Map where Device_Mac = '" + deviceMac + "'";
            int? resultValue = this.ExecuteSqlQuery("UPDATE Users SET Password = '" + password + "' where Id = (" + uidQuery + ")");
            if (resultValue == null) return "Database connection failed";
            if (resultValue <= 0) return "Login ID not found";
            else return null;
        }

        public bool? UnbindMacAddressFromUserAccount(long userId, string macAddress)
        {
            string query = "DELETE FROM Devices_Bind_Map WHERE User_id = " + userId + " and Device_Mac = '"+macAddress+"';";
            int? result = ExecuteSqlQuery(query);
            if (result == null) return null;
            if (result == 0) return false; ;
            return true;
        }

        new public static UserRepository Instance
        {
            get
            {
                return new UserRepository();
            }
        }
    }

}
