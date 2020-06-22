using Display;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileRepositories
{
    public class FileIOAccess
    {
        public static bool EraseFile(string targetFilePath)
        {
            try
            {
                File.Delete(targetFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SaveByteArrayToFile(byte[] byteArray, string filePath)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    fileStream.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in SaveByteArrayToFile(): {0}", ex);
                return false;
            }
        }
        public static string ProfileImageDirectory
        {
            get
            {
                string directoryPath = FileIOAccess.ParentDirectory + "ProfileImages\\";
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
                return directoryPath;
            }
        }
        public static string ImageDirectory
        {
            get
            {
                string directoryPath = FileIOAccess.ParentDirectory + "Images\\";
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
                return directoryPath;
            }
        }
        public static string ContentFileDirectory
        {
            get
            {
                string directoryPath = FileIOAccess.ParentDirectory + "ContentFiles\\";
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
                return directoryPath;
            }
        }

        public static string ParentDirectory
        {
            get
            {
                string directoryPath = ConfigurationManager.AppSettings["ParentDirectory"];
                if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
                return directoryPath;
            }
        }
    }
}
