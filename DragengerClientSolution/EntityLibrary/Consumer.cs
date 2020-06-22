using System;
using System.Collections.Generic;
using System.Drawing;
using FileIOAccess;
using Newtonsoft.Json.Linq;

namespace EntityLibrary
{
    public class Consumer : User
    {
        private string name, email, dragengerEmail, phone, profileImageId;
        private Image profileImage;
        private Time birthdate;
        private int genderIndex;

        public Consumer(long userId, string username, string email, string name)
            : base(userId, username, "consumer")
        {
            this.Name = name;
            this.email = email;
            this.ProfileImage = FileResources.NullProfileImage;
        }

        public Consumer(string username, string email, string name)
            : base(username, "consumer")
        {
            this.Name = name;
            this.email = email;
            this.ProfileImage = FileResources.NullProfileImage;
        }

        public Consumer(JObject userJson)
            : base((long)userJson["id"], (string)userJson["username"], "consumer")
        {
            if (userJson["name"] != null) this.Name = (string)userJson["name"];
            if (userJson["email"] != null) this.Email = (string)userJson["email"];
            this.ProfileImage = FileResources.NullProfileImage;
            if (userJson["phone"] != null) this.Phone = (string)userJson["phone"];
            if (userJson["birthdate"] != null) this.Birthdate = new Time((string)userJson["birthdate"]);
            if (userJson["gender_index"] != null) this.GenderIndex = int.Parse(userJson["gender_index"].ToString());
            if (userJson["profile_img_id"] != null)
            {
                this.profileImageId = userJson["profile_img_id"].ToString();
                this.SetProfileImage(this.profileImageId);
            }
        }

        public bool SetProfileImage(string profileImageId)
        {
            if (LocalDataFileAccess.ProfileImgExistsInLocalData(profileImageId))
            {
                this.profileImageId = profileImageId;
                this.profileImage = LocalDataFileAccess.GetProfileImgFromLocalData(this.profileImageId);
                return true;
            }
            return false;
        }

        public JObject ToJson()
        {
            JObject jsonData = new JObject();
            jsonData["type"] = "consumer";
            jsonData["id"] = this.Id;
            jsonData["user_id"] = this.Id;
            jsonData["username"] = this.Username;
            jsonData["name"] = this.Name;
            jsonData["email"] = this.Email;
            if(this.LastActive != null) jsonData["last_active"] = this.LastActive.TimeStampString;
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

        public Image ProfileImage
        {
            set { this.profileImage = ResourceLibrary.GraphicsStudio.ClipToCircle(value); }
            get { return this.profileImage; }
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

        public List<Consumer> FriendList
        {
            set;
            get;
        }

        new public static Consumer LoggedIn
        {
            set;
            get;
        }
    }
}
