using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FileIOAccess;
using EntityLibrary;
using ServerConnections;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace StandardAssuranceLibrary
{
    public class Checker
    {
        public static string CheckUsernameValidity(ref string input)
        {
            input = input.ToLower();
            if (input.Length < 5) return "Too short";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == ' ') return "Username contains white space";
                if (!((input[i] >= 'a' && input[i] <= 'z') || (input[i] >= 'A' && input[i] <= 'Z') || input[i] == '_' || (input[i] >= '0' && input[i] <= '9'))) return "contains invalid characters";
            }
            if (input.Length > 20) return "Too long";
            bool? existsOnDatabase = ServerRequest.UsernameAlreadyExists(input);
            if (existsOnDatabase == true) return "Username already reserved";
            else if (existsOnDatabase == null) return "Availability checking failed";
            return null;
        }

        public static string CheckEmailValidity(ref string input)
        {
            if (input == null) return "No email found as input";
            input = input.ToLower();
            if (input.Length < 10) return "Too short";
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == ' ') return "Email contains white space";
                if (!((input[i] >= 'a' && input[i] <= 'z') || input[i] == '_' || input[i] == '.' || input[i] == '@' || (input[i] >= '0' && input[i] <= '9'))) return "Contains invalid characters";
            }
            int itr = 0;
            while (itr < input.Length && input[itr] != '@') itr++;
            if (itr >= input.Length) return "Incomplete";
            itr++;
            int domainPrefix = 0;
            while (itr < input.Length && ((input[itr] >= 'a' && input[itr] <= 'z') || input[itr] == '_' || (input[itr] >= '0' && input[itr] <= '9')))
            {
                domainPrefix++;
                itr++;
            }
            if (domainPrefix < 2) return "Incomplete email domain";
            if (itr >= input.Length) return "Incomplete";
            if (input[itr] != '.') return "Domain contains invalid characters";
            itr++;
            char last = '.';
            while (itr < input.Length)
            {
                if (!((input[itr] >= 'a' && input[itr] <= 'z') || (input[itr] >= '0' && input[itr] <= '9') || input[itr] == '.' || input[itr] == '_')) return "Domain contains invalid characters";
                if (last == input[itr] && last == '.') return "Domain contains invalid characters";
                last = input[itr];
                itr++;
            }
            if (!((input[input.Length - 1] >= 'a' && input[input.Length - 1] <= 'z') || (input[input.Length - 1] >= '0' && input[input.Length - 1] <= '9') || input[input.Length - 1] == '_')) return "Domain contains invalid characters";
            if (input.Length > 45) return "Too long";

            bool? existsOnDatabase = ServerRequest.EmailAlreadyExists(input);
            if (existsOnDatabase == true) return "Email already belongs to a user";
            else if (existsOnDatabase == null) return "Availability checking failed";

            return null;
        }

        public static string CheckDeviceIDValidity(string input)
        {
            if (input.Length == 0) return "Check Internet connection status";
            if (input.Length < 12) return "Error in fetching device id";
            return null;
        }

        public static JObject ValidLoginCookieData(string nativeMacAddress)
        {
            List<double> encryptedCredentials = LocalDataFileAccess.GetCredentialsFromLoginCookie();
            if (encryptedCredentials != null)
            {
                JObject cookieDataJson = JObject.Parse(MathLaboratory.MatrixCryptography.Decrypt(encryptedCredentials));
                string deviceMac = cookieDataJson["mac_address"].ToString();
                string password = cookieDataJson["password"].ToString();
                string lastActiveTimeStamp = cookieDataJson["last_login_time"].ToString();
                if (deviceMac == nativeMacAddress)
                {
                    return cookieDataJson;
                }
            }
            return null;
        }

        public static Int16 DeterminePasswordStrength(string input)
        {
            if (input == null || input.Length < 8) return 0;
            else if (input.Length > 25) return -2;
            bool capital = false, small = false, specialChar = false, numeric = false;
            foreach (char item in input)
            {
                if (item >= 'A' && item <= 'Z') capital = true;
                else if (item >= 'a' && item <= 'z') small = true;
                else if (item >= '0' && item <= '9') numeric = true;
                else if (item >= '!' && item <= '~') specialChar = true;
                else return -1;
            }
            return (Int16)(Convert.ToInt16(capital) + Convert.ToInt16(small) + Convert.ToInt16(numeric) + Convert.ToInt16(specialChar) + Convert.ToInt16(input.Length > 10));
        }

        public static string CheckNameValidity(string input)
        {
            input = input.ToLower();
            if (input.Length < 7) return "too short";
            if (input.Length > 35) return "too long";
            for (int i = 0; i < input.Length; i++) if (!((input[i] >= 'a' && input[i] <= 'z') || input[i] == ' ')) return "contains invalid characters";
            int itr = 0, wordCount = 0;
            while (itr < input.Length)
            {
                while (itr < input.Length && input[itr] == ' ') itr++;
                bool newWord = false;
                while (itr < input.Length && input[itr] != ' ')
                {
                    newWord = true;
                    itr++;
                }
                if (newWord) wordCount++;
                while (itr < input.Length && input[itr] == ' ') itr++;
            }
            if (wordCount < 2) return "must contain at least 2 words";
            if (wordCount > 4) return "must contain maximum 4 words";

            return null;
        }

        public static string CheckOldPasswordMatch(string passwordInput)
        {
            bool? secretKeyMatches = ServerRequest.PasswordMatches(Consumer.LoggedIn.Id, passwordInput);
            if (secretKeyMatches == true) return null;
            else if (secretKeyMatches == false) return "Password doesn't match";
            else return "Checking falied";
        }
    }
}
