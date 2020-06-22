using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace EntityLibrary
{
    public class Consumer : User
    {
        private string name, email, dragengerEmail, phone;
        private string profileImageId;
        private Time birthdate;
        private int genderIndex;
        private List<Conversation> conversationList;

        public Consumer(long userId)
            : base(userId, "consumer")
        {

        }

        public Consumer(JObject userJson)
            : base((long)userJson["id"], (string)userJson["username"], null, "consumer")
        {
            if (userJson["name"] != null) this.Name = (string)userJson["name"];
            if (userJson["email"] != null) this.Email = (string)userJson["email"];
            if (userJson["phone"] != null) this.Phone = (string)userJson["phone"];
            if (userJson["birthdate"] != null) this.Birthdate = new Time((string)userJson["birthdate"]);
            if (userJson["gender_index"] != null) this.GenderIndex = int.Parse(userJson["gender_index"].ToString());
            if (userJson["profile_img_id"] != null)
            {
                this.profileImageId = userJson["profile_img_id"].ToString();
            }
        }

        public Consumer(long userId, string username, string password, string email, string name)
            : base(userId, username, password, "consumer")
        {
            this.Name = name;
            this.email = email;
            this.conversationList = null;
        }

        public Consumer(string username, string email, string name)
            : base(username, "consumer")
        {
            this.Name = name;
            this.email = email;
            this.conversationList = null;
        }

        public JObject ToJson()
        {
            JObject jsonData = new JObject();
            jsonData["type"] = "consumer";
            jsonData["id"] = this.Id;
            jsonData["user_id"] = this.Id;
            jsonData["username"] = this.Username;
            if(this.LastActive != null) jsonData["last_active"] = this.LastActive.TimeStampString;
            jsonData["name"] = this.Name;
            jsonData["email"] = this.Email;
            if (this.Birthdate != null) jsonData["birthdate"] = this.Birthdate.TimeStampString;
            if (this.Phone != null) jsonData["phone"] = this.Phone;
            if (this.GenderIndex > 0) jsonData["gender_index"] = this.GenderIndex;
            if (this.ProfileImageId != null) jsonData["profile_img_id"] = this.ProfileImageId;
            return jsonData;
        }

        public User ConsumerUser
        {
            get
            {
                return (User)this;
            }
        }

        public string Phone 
        {
            get { return this.phone; }
            set { this.phone = value; }
        }

        public string Name
        {
            set { this.name = value; }
            get { return this.name; }
        }
        public string Email
        {
            set { this.email = value; }
            get { return this.email; }
        }
        public string DragengerEmail
        {
            set { this.dragengerEmail = value; }
            get { return this.dragengerEmail; }
        }
        public string ProfileImageId
        {
            set { this.profileImageId = value; }
            get { return this.profileImageId; }
        }

        public Time Birthdate
        {
            set { this.birthdate = value; }
            get { return this.birthdate; }
        }

        public int GenderIndex
        {
            set { this.genderIndex = value; }
            get { return this.genderIndex; }
        }

        public List<Conversation> ConversationList
        { get { return this.conversationList; } }

        public List<Consumer> FriendList
        {
            set;
            get;
        }
    }
}
