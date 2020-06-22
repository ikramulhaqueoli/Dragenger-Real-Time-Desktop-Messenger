using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLibrary
{
    public class User
    {
        private long id;
        private string username, accountType, password;

        public User(long id, string username, string password, string accountType)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.accountType = accountType;
        }

        public User(string username, string accountType)
        {
            this.username = username;
            this.accountType = accountType;
        }

        public User(long userId, string type)
        {
            this.id = userId;
            this.accountType = type;
        }

        public long Id
        {
            set { this.id = value; }
            get { return this.id; }
        }

        public string Password
        {
            set { this.password = value; }
            get { return this.password; }
        }

        public string Username
        {
            set { this.username = value; }
            get { return this.username; }
        }

        public string AccountType
        {
            set { this.accountType = value; }
            get { return this.accountType; }
        }

        public Time LastActive
        {
            set;
            get;
        }
    }
}
