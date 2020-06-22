using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLibrary
{
    public class GroupConversation : Conversation
    {
        private string conversationName;
        public GroupConversation(List<Consumer> memberList)
        {
            this.memberList = memberList;
            this.Type = "group";
        }

        public GroupConversation(long conversationID, string conversationName, List<Consumer> memberList)
            : base(conversationID, "group")
        {
            this.memberList = memberList;
            this.conversationName = conversationName;
        }

        private List<Consumer> memberList;

        public List<Consumer> MemberList
        { get { return this.memberList; } }

        public void AddMember(params Consumer[] list)
        {
            foreach (Consumer item in list)
            {
                this.memberList.Add(item);
            }
        }

        public string ConversationName
        {
            get { return this.conversationName; }
        }
    }
}
