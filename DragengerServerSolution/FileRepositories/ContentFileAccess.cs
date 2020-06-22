using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRepositories
{
    public class ContentFileAccess : FileIOAccess
    {
        public static bool StoreNuntiasContentFile(byte[] fileBytes, long nuntiasId)
        {
            string targetFilePath = FileIOAccess.ContentFileDirectory + nuntiasId;
            return FileIOAccess.SaveByteArrayToFile(fileBytes, targetFilePath);
        }

        public static string GetNuntiasContentFilePath(long nuntiasId)
        {
            if (!File.Exists(FileIOAccess.ContentFileDirectory + nuntiasId)) return null;
            return FileIOAccess.ContentFileDirectory + nuntiasId;
        }
    }
}
