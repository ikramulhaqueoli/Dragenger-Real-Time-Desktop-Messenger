using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace ResourceLibrary
{
    public static class Universal
    {
        private static readonly string systemMACAddress;
        public static Form ParentForm;
        static Universal()
        {
            var macAddress =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            systemMACAddress = macAddress.ToString();
            //systemMACAddress = "526341789852";       //for testing
            //systemMACAddress = "926341789852";       //for testing
        }
        public static string SystemMACAddress
        {
            get { return Universal.systemMACAddress; }
        }
        public static string ProcessStringToShow(string input, int lineLength)
        {
            string processed = "";
            foreach (char item in input)
            {
                if (item == '&') processed += "&&";
                else processed += item;
            }
            int start = 0;
            List<string> brokenStringList = new List<string>();
            while (true)
            {
                int breakPoint = start + lineLength;
                if (start >= processed.Length) break;
                if (breakPoint >= processed.Length)
                {
                    brokenStringList.Add(processed.Substring(start, processed.Length - start));
                    break;
                }
                int tolaracy = 0, i = breakPoint;
                while (i >= 0 && !CheckBreak(processed[i]))
                {
                    tolaracy++;
                    i--;
                }
                if (tolaracy <= 12 && i > 0) breakPoint = i;
                brokenStringList.Add(processed.Substring(start, breakPoint - start + 1));
                start = breakPoint + 1;
            }
            string output = "";
            foreach (string item in brokenStringList)
            {
                if (item[item.Length - 1] == '\n') output += item;
                else output += (item + Environment.NewLine);
            }
            return output;
        }

        public static string ProcessValidMessageText(string input)
        {
            string processedText = null;
            bool ok = false;
            foreach(char c in input)
            {
                if (c == 39) processedText += "''";
                else processedText += c;
                if (c > ' ') ok = true;
            }
            if (!ok) return null;
            return processedText;
        }

        public static void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public static void Swap<T>(ref T x, ref T y) where T : struct
        {
            T tmp = x;
            x = y;
            y = tmp;
        }

        public static void Swap<T>(T x, T y) where T : class
        {
            T tmp = x;
            x = y;
            y = tmp;
        }


        public static string NameValidator(string input)
        {
            int itr = 0;
            string validNameString = "";
            while (itr < input.Length)
            {
                while (itr < input.Length && input[itr] == ' ') itr++;
                while (itr < input.Length && input[itr] != ' ') validNameString += input[itr++];
                while (itr < input.Length && input[itr] == ' ') itr++;
                if (itr < input.Length) validNameString += ' ';
            }
            return validNameString;
        }

        //Supplementary methods
        private static bool CheckBreak(char c)
        {
            char[] breakSymbols = new char[] { ' ', '.', ',', '!', '?', '%', '$', ')', '}', '>', ']', '/', '+', '-', '\n' };
            foreach (char item in breakSymbols) if (c == item) return true;
            return false;
        }

        public static void SetLinkAreaIfLinkFound(LinkLabel inputLabel)
        {
            string text = inputLabel.Text.ToLower();
            if(text.Length > 7) for(int i = 0; i < text.Length - 7; i++)
            {
                int dotCount = 0;
                char prevChar = text[0];
                bool valid = true;
                string address = "";
                int linkTextLength = 0;
                for (int j = i; j < text.Length; j++)
                {
                    if (prevChar == '.' && text[j] == '.')
                    {
                        valid = false;
                        break;
                    }
                    if (!(text[j] == '=' || text[j] == '-' || text[j] == '.' || text[j] == '_' || text[j] == '~' || text[j] == '?' || text[j] == '/' || text[j] == '#' || text[j] == ':' || text[j] == '%' || (text[j] >= 'a' && text[j] <= 'z') || (text[j] >= '0' && text[j] <= '9') || text[j] == '\n' || text[j] == '\r'))
                    {
                        break;
                    }
                    if (text[j] != '\n' && text[j] != '\r')
                    {
                        address += text[j];
                        prevChar = text[j];
                    }
                    if (text[j] == '.') dotCount++;
                    linkTextLength++;
                }
                if (dotCount > 1 && valid && address.Length > 6)
                {
                    inputLabel.Links.Add(i, linkTextLength, address);
                    i += linkTextLength;
                }
                else if (dotCount >= 1 && valid && address.Length > 8)
                {
                    if (address.Substring(0, 7) == "http://" || address.Substring(0, 8) == "https://" || address.Substring(0, 6) == "ftp://") inputLabel.Links.Add(i, linkTextLength, address);
                    i += linkTextLength;
                }
            }
        }

		public static byte[] ImageToByteArray(Image img, ImageFormat format)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                img.Save(mStream, format);
                return mStream.ToArray();
            }
        }

		public static Image ByteArrayToImage(byte[] byteArrayIn)
		{
			using (MemoryStream mStream = new MemoryStream(byteArrayIn))
			{
				return Image.FromStream(mStream);
			}
		}

        public static byte[] FileToByteArray(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        public static string GetRandomAlphaNumericString(int length)
        {
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }

        //message_boxes

        //to show errors
        public static void ShowErrorMessage(string errorMessage, string messageBoxTitle)
        {
            if (ParentForm.InvokeRequired)
            {
                ParentForm.Invoke(new MethodInvoker(delegate
                {
                    MessageBox.Show(ParentForm, errorMessage, messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
            else
            {
                MessageBox.Show(ParentForm, errorMessage, messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static void ShowErrorMessage(string errorMessage)
        {
            ShowErrorMessage(errorMessage, "Error!");
        }

        //to show informations
        public static void ShowInfoMessage(string infoMessage, string messageBoxTitle)
        {
            if (ParentForm.InvokeRequired)
            {
                ParentForm.Invoke(new MethodInvoker(delegate
                {
                    MessageBox.Show(ParentForm, infoMessage, messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                MessageBox.Show(ParentForm, infoMessage, messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public static void ShowInfoMessage(string infoMessage)
        {
            ShowInfoMessage(infoMessage, "Information!");
        }
    }
}
