using Display;
using EntityLibrary;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResourceLibrary
{
    public class DataProcessor
    {
        public static List<double> ProcessLoggedInUserData(User user)
        {
            JObject dataJson = new JObject();
            dataJson["found"] = false;
            if (user != null)
            {
                if (user.AccountType == "consumer")
                {
                    Consumer consumer = (Consumer)user;
                    dataJson = consumer.ToJson();
                }
                dataJson["found"] = true;
                Output.ShowLog(dataJson);
            }
            return MatrixCryptography.Encrypt(dataJson.ToString());
        }
    }
}
