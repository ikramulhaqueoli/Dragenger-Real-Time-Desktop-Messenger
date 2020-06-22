using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace EntityLibrary
{
    public class DuetConversation : Conversation
    {
        private Consumer member1, member2;

        public DuetConversation(Consumer member1, Consumer member2)
        {
            this.member1 = member1;
            this.member2 = member2;
            this.Type = "duet";
        }

        public DuetConversation(long conversationID, Consumer member1, Consumer member2)
            : base(conversationID, "duet")
        {
            this.member1 = member1;
            this.member2 = member2;
        }

        public Consumer Member1
        {
            set { this.member1 = value; }
            get { return this.member1; }
        }

        public Consumer Member2
        {
            set { this.member2 = value; }
            get { return this.member2; }
        }

        public string Nickname1
        {
            set;
            get;
        }

        public string Nickname2
        {
            set;
            get;
        }
    }
}
