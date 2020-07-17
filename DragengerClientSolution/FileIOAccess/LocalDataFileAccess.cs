using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using ResourceLibrary;

namespace FileIOAccess
{
    public static class LocalDataFileAccess
    {
        public static string LoginCookiePath
        {
            get { return FileResources.UserDataPath + "login.dat"; }
        }

        public static bool CreateNewLoginCoockie(List<double> encryptedCredetials)
        {
            FileStream coockieFile = File.Create(LocalDataFileAccess.LoginCookiePath);
            LocalDataFileAccess.WriteDoubleBinaryTo(coockieFile, encryptedCredetials);
            coockieFile.Close();
            return true;
        }

        private static void WriteDoubleBinaryTo(FileStream fStream, List<double> listOfDoubles)
        {
            using (BinaryWriter binWriter = new BinaryWriter(fStream))
            {
                foreach (double item in listOfDoubles)
                {
                    binWriter.Write(item);
                }
            }
        }

        public static List<double> GetCredentialsFromLoginCookie()
        {
            List<double> credentialsDoubleList = new List<double>();
            FileStream fstream = null;
            try
            {
                fstream = File.Open(LoginCookiePath, FileMode.Open);
                BinaryReader binReader = new BinaryReader(fstream);
                {
                    int length = (int)binReader.BaseStream.Length, index = 0;
                    while (index < length)
                    {
                        credentialsDoubleList.Add(binReader.ReadDouble());
                        index += sizeof(double);
                    }
                }
                return credentialsDoubleList;
            }
            catch
            {
                return null;
            }
            finally
            {
                if(fstream != null) fstream.Close();
            }
        }

        public static void EraseContentFile(string fileId)
        {
            string filePath = GetFilePathInLocalData(fileId);
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        public static Image ProfileImage(string file_ID)
        {
            string directoryPath = FileResources.UserDataPath + "ProfileImages\\";
            if (Directory.Exists(directoryPath) && File.Exists(directoryPath + file_ID + ".jpg")) return Image.FromFile(directoryPath + file_ID + ".jpg");
            return FileResources.NullProfileImage;
        }

        public static bool SaveProfileImageToLocal(Image profileImg, string fileKey)
        {
            try
            {
                string path = FileResources.ProfileImgFolderPath + fileKey;
                FileResources.EraseFile(path);
                using (FileStream imgStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    profileImg.Save(imgStream, ImageFormat.Png);
                    imgStream.Close();
                    profileImg.Dispose();
                }
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("Saving profile image to folder failed due to: " + e.Message);
                return false;
            }
        }

        public static bool ProfileImgExistsInLocalData(string profileImgFileId)
        {
            string path = FileResources.ProfileImgFolderPath + profileImgFileId;
            return File.Exists(path);
        }

        public static bool EraseOldProfileImageFromLocalData(string profileImgFileId)
        {
            string path = FileResources.ProfileImgFolderPath + profileImgFileId;
            string error = FileResources.EraseFile(path);
            return (error == null);
        }

        public static Image GetProfileImgFromLocalData(string fileKey)                  //here, an exception comes that, the file is in use.
        {
            if (fileKey.Length < 5) return FileResources.NullProfileImage;
            string path = FileResources.ProfileImgFolderPath + fileKey;
            if (!File.Exists(path)) return null;
            using (FileStream fstream = new FileStream(path,FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Image img = new Bitmap(fstream);
                fstream.Close();
                return img;
            }
        }

        public static Image GetConversationImageFromLocalData(string fileKey, string type)                  //here, an exception comes that, the file is in use.
        {
            if (fileKey.Length < 5)
            {
                if(type == "group") return FileResources.NullGroupConversationIcon;
                if(type == "duet") return FileResources.NullProfileImage;
            }
            string path = FileResources.ProfileImgFolderPath + fileKey;
            if (!File.Exists(path)) return null;
            using (FileStream fstream = new FileStream(path,FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Image img = new Bitmap(fstream);
                fstream.Close();
                return img;
            }
        }

        public static string GetFilePathInLocalData(string fileId)
        {
            string path = FileResources.NuntiasContentFolderPath + fileId;
            if (!File.Exists(path)) return null;
            return path;
        }

        public static bool WriteTo(FileStream file, string text)
        {
            try
            {
                byte[] textData = new UTF8Encoding(true).GetBytes(text + '\n');
                file.Write(textData, 0, textData.Length);
                file.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string EraseCurrentLoginCookie()
        {
            return FileResources.EraseFile(LoginCookiePath);
        }

        public static string ReadLineFrom(string path)
        {
            try
            {
                StreamReader streamReader = new StreamReader(path);
                string line = streamReader.ReadLine();
                streamReader.Close();
                return line;
            }
            catch
            {
                return null;
            }
        }

        public static bool SaveNuntiasContentToLocal(MemoryStream sourceMemoryStream, string fileName)
        {
            try
            {
                string path = FileResources.NuntiasContentFolderPath + fileName;
                FileResources.EraseFile(path);
                using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    sourceMemoryStream.Position = 0;
                    sourceMemoryStream.CopyTo(fileStream);
                    fileStream.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in SaveNuntiasContentToLocal() => : " + e.Message);
                return false;
            }
        }

        public static string GetContentPathFromLocalData(string fileName)
        {
            if (fileName == null || fileName.Length == 0) return null;
            if (File.Exists(FileResources.NuntiasContentFolderPath + fileName)) return FileResources.NuntiasContentFolderPath + fileName;
            return null;
        }

        public static bool ContentExistsInLocalData(string fileKey)
        {
            string path = FileResources.NuntiasContentFolderPath + fileKey;
            return File.Exists(path);
        }

        internal static bool CopyLocalDbToAppdataIfNotExists(string localDbFileName)
        {
            try
            {
                if (!File.Exists(FileResources.UserDataPath + localDbFileName))
                {
                    CopyFreshLocalDbFile(localDbFileName);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in CopyLocalDbToAppdataIfNotExists() => " + ex.Message);
                return false;
            }
        }

        public static bool ResetUserData()
        {
            try
            {
                Console.WriteLine("ResetUserData()");
                string dataDirectoryPath = FileResources.UserDataPath.Substring(0, FileResources.UserDataPath.Length - 1);
                if(Directory.Exists(dataDirectoryPath)) Directory.Delete(dataDirectoryPath, true);
                string localDbFileName = Universal.SystemMACAddress + ".sdf";
                if (File.Exists(FileResources.UserDataPath + localDbFileName)) File.Delete(FileResources.UserDataPath + localDbFileName);
                CopyFreshLocalDbFile(localDbFileName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in ResetUserData() => " + ex.Message);
                return false;
            }
        }

        private static void CopyFreshLocalDbFile(string localDbFileName)
        {
            using (Stream stream = FileResources.CleanLocalDbStream)
            using (FileStream fileStream = new FileStream(FileResources.UserDataPath + localDbFileName, FileMode.Create))
            {
                stream.Position = 0;
                stream.CopyTo(fileStream);
            }
        }
    }
}
