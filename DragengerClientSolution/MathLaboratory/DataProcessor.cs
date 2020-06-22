using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLaboratory
{
    public class DataProcessor
    {
        public static Dictionary<string, string> ParseLoginData(List<double> encryptedCredentials)
        {
            string credentials = MathLaboratory.MatrixCryptography.Decrypt(encryptedCredentials);
            int itr = 0;
            string deviceMac = "", password = "", lastActiveTimeStamp = "";
            while (itr < credentials.Length && credentials[itr] != ' ') deviceMac += credentials[itr++];
            if (itr < credentials.Length && credentials[itr] == ' ') itr++;
            while (itr < credentials.Length && credentials[itr] != ' ') password += credentials[itr++];
            if (itr < credentials.Length && credentials[itr] == ' ') itr++;
            while (itr < credentials.Length && credentials[itr] != ' ') lastActiveTimeStamp += credentials[itr++];

            Dictionary<string, string> loginData = new Dictionary<string, string>();
            if (deviceMac.Length != 0) loginData["deviceMac"] = deviceMac;
            if (password.Length != 0) loginData["password"] = password;
            if (lastActiveTimeStamp.Length != 0) loginData["lastActiveTimeStamp"] = lastActiveTimeStamp;
            return loginData;
        }
    }
}
