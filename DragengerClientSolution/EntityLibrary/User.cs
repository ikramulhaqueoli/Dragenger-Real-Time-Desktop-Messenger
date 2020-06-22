using Newtonsoft.Json.Linq;
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
        private string username, accountType;

        public User(long id, string username, string accountType)
        {
            this.id = id;
            this.username = username;
            this.accountType = accountType;
        }

        public User(string username, string accountType)
        {
            this.username = username;
            this.accountType = accountType;
        }

        public long Id
        {
            set { this.id = value; }
            get { return this.id; }
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
        public static User LoggedIn
        {
            set;
            get;
        }
    }
}
