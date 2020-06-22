using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLibrary
{
    public class Nuntias
    {
        private long id;
        private string text;
        private long senderId;
        private Time sentTime, deliveryTime, seenTime;
        private long nativeConversationID;
        private string contentFileId;
        public Nuntias(string text, long senderId, Time sentTime, long nativeConversationID)
        {
            this.text = text;
            this.senderId = senderId;
            this.sentTime = sentTime;
            this.nativeConversationID = nativeConversationID;
        }
        public Nuntias(long id, string text, long senderId, Time sentTime, Time deliveryTime, Time seenTime, string contentPath, long nativeConversationID)
            : this(text, senderId, sentTime, nativeConversationID)
        {
            this.deliveryTime = deliveryTime;
            this.seenTime = seenTime;
            this.contentFileId = contentPath;
            this.id = id;
        }
        public Nuntias(JObject nuntiasJson)
        {
            try { this.id = long.Parse(nuntiasJson["id"].ToString()); }
            catch { }
            try { this.text = nuntiasJson["text"].ToString(); }
            catch { }
            try { this.senderId = long.Parse(nuntiasJson["sender_id"].ToString()); }
            catch { }
            try { this.sentTime = new Time(nuntiasJson["sent_time"].ToString()); }
            catch { }
            try { this.deliveryTime = new Time(nuntiasJson["delivery_time"].ToString()); }
            catch { }
            try { this.seenTime = new Time(nuntiasJson["seen_time"].ToString()); }
            catch { }
            try { this.contentFileId = nuntiasJson["content_file_id"].ToString(); }
            catch { }
            try { this.nativeConversationID = long.Parse(nuntiasJson["conversation_id"].ToString()); }
            catch { }
        }

        public JObject ToJson()
        {
            JObject jsonData = new JObject();
            jsonData["id"] = this.id;
            jsonData["text"] = this.text;
            jsonData["sender_id"] = this.senderId;
            if (this.sentTime != null) jsonData["sent_time"] = this.sentTime.TimeStampString;
            if (this.deliveryTime != null) jsonData["delivery_time"] = this.deliveryTime.TimeStampString;
            if (this.seenTime != null) jsonData["seen_time"] = this.seenTime.TimeStampString;
            if (this.contentFileId != null) jsonData["content_file_id"] = this.contentFileId;
            jsonData["conversation_id"] = this.nativeConversationID;
            return jsonData;
        }
        public long Id
        {
            set { this.id = value; }
            get { return this.id; }
        }
        public string Text
        {
            set { this.text = value; }
            get { return this.text; }
        }
        public Time SentTime
        {
            private set { this.sentTime = value; }
            get { return this.sentTime; }
        }
        public Time SeenTime
        {
            set { this.seenTime = value; }
            get { return this.seenTime; }
        }
        public Time DeliveryTime
        {
            set { this.deliveryTime = value; }
            get { return this.deliveryTime; }
        }
        public long NativeConversationID
        {
            set { this.nativeConversationID = value; }
            get { return this.nativeConversationID; }
        }
        public long SenderId
        {
            set { this.senderId = value; }
            get { return this.senderId; }
        }
        public string ContentFileId
        {
            set { this.contentFileId = value; }
            get { return this.contentFileId; }
        }
    }
}
