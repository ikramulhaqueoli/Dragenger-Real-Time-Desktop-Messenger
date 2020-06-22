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
        public Conversation()
        {

        }
        public Conversation(long conversationID, string type)
        {
            this.conversationID = conversationID;
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
