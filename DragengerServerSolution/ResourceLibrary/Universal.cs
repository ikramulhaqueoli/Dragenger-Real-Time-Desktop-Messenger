using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.IO;
using System.Drawing.Imaging;

namespace ResourceLibrary
{
    public static class Universal
    {
        private static readonly string systemMACAddress;
        static Universal()
        {
            var macAddress =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            systemMACAddress = macAddress.ToString();
        }
        public static string SystemMACAddress
        {
            get { return systemMACAddress; }
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

        public static string RandomNumericString(int length)
        {
            string generatedString = "";
            Random random = new Random();
            for(int i = 0; i < length; i++)
            {
                generatedString += (Math.Abs(random.Next()) % 10);
            }
            return generatedString;
        }
    }
}
