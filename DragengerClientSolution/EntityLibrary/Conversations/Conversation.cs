using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace EntityLibrary
{
    public abstract class Conversation
    {
        protected long conversationID;
        protected Image conversationIcon;

        public Conversation()
        {
            this.conversationIcon = null;
        }

        public Conversation(long conversationID, Image conversationIcon, string type)
        {
            this.conversationID = conversationID;
            this.conversationIcon = conversationIcon;
            this.Type = type;
        }

        public long ConversationID
        {
            set { this.conversationID = value; }
            get { return this.conversationID; } 
        }

        public string Type
        {
            set;
            get;
        }

        public abstract Image ConversationIcon
        {
            get;
        }

        public abstract string ConversationName
        {

            get;
        }

        public static Conversation Last
        {
            set;
            get;
        }

        public static Conversation TopConversation
        {
            set;
            get;
        }
    }
}
