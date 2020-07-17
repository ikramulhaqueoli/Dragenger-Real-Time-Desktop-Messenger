using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Media;
using System.Drawing.Text;
using ResourceLibrary;

namespace FileIOAccess
{
    public static class FileResources
    {
        static Assembly fileIoAccessAssembly;

        static FileResources()
        {
            fileIoAccessAssembly = Assembly.GetExecutingAssembly();
        }
        public static Image NullProfileImage
        {
            get
            {
                try
                {
                    Stream stream = fileIoAccessAssembly.GetManifestResourceStream(ImagesResourcesPath + "nullProfileImage.png");
                    Image img = new Bitmap(stream);
                    stream.Close();
                    return img;
                }
                catch
                {
                    return NullImage;
                }
            }
        }

        public static Image NullGroupConversationIcon
        {
            get
            {
                try
                {
                    Stream stream = fileIoAccessAssembly.GetManifestResourceStream(ImagesResourcesPath + "group_conversation_icon.png");
                    Image img = new Bitmap(stream);
                    stream.Close();
                    return img;
                }
                catch
                {
                    return NullImage;
                }
            }
        }

        public static Image Picture(string name)
        {
            try
            {
                Stream stream = fileIoAccessAssembly.GetManifestResourceStream(ImagesResourcesPath + name);
                Image img = new Bitmap(stream);
                stream.Close();
                return img;
            }
            catch
            {
                return NullImage;
            }
        }

        public static Image Icon(string name)
        {
            try
            {
                Stream stream = fileIoAccessAssembly.GetManifestResourceStream(IconsResourcesPath + name);
                Image img = new Bitmap(stream);
                stream.Close();
                return img;
            }
            catch
            {
                return NullImage;
            }
        }

        public static SoundPlayer Audio(string name)
        {
            try
            {
                Stream stream = fileIoAccessAssembly.GetManifestResourceStream(AudioResourcesPath + name);
                SoundPlayer sound = new SoundPlayer(stream);
                //stream.Close();
                //stream.Dispose();
                return sound;
            }
            catch
            {
                return null;
            }
        }

        public static Image NullImage
        {
            get
            {
                Stream stream = fileIoAccessAssembly.GetManifestResourceStream(ResourcesPath + "null.png");
                Image img = new Bitmap(stream);
                stream.Close();
                return img;
            }
        }

        public static Stream CleanLocalDbStream
        {
            get { return fileIoAccessAssembly.GetManifestResourceStream(CleanLocalDbPath); }
        }

        public static string IconsResourcesPath
        {
            get { return ResourcesPath + "icons."; }
        }
        public static string ImagesResourcesPath
        {
            get { return ResourcesPath + "images."; }
        }

        public static string AudioResourcesPath
        {
            get { return ResourcesPath + "audios."; }
        }

        public static string FontResourcesPath
        {
            get { return ResourcesPath + "fonts."; }
        }

        public static string ResourcesPath
        {
            get
            {
                return "FileIOAccess.Res.";
            }
        }

        public static string CleanLocalDbPath
        {
            get { return ResourcesPath + "LocalData.sdf"; }
        }

        public static string WindowsPicturePath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); }
        }

        public static string WindowsDocumentsPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
        }

        public static string TempFolderPath
        {
            get
            {
                try
                {
                    string tmpFolderPath = UserDataPath + "TempFileStorage\\";
                    if (!Directory.Exists(tmpFolderPath)) Directory.CreateDirectory(tmpFolderPath);
                    return tmpFolderPath;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Temp folder access failed due to: " + e.Message);
                    return null;
                }
            }
        }

        public static string ProfileImgFolderPath
        {
            get
            {
                try
                {
                    string tmpFolderPath = UserDataPath + "ProfilePictures\\";
                    if (!Directory.Exists(tmpFolderPath)) Directory.CreateDirectory(tmpFolderPath);
                    return tmpFolderPath;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Temp folder access failed due to: " + e.Message);
                    return null;
                }
            }
        }

        public static string NuntiasContentFolderPath
        {
            get
            {
                try
                {
                    string tmpFolderPath = UserDataPath + "NuntiasContents\\";
                    if (!Directory.Exists(tmpFolderPath)) Directory.CreateDirectory(tmpFolderPath);
                    return tmpFolderPath;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Temp folder access failed due to: " + e.Message);
                    return null;
                }
            }
        }

        public static string LocalDatabasePath
        {
            get
            {
                string localDbFileName = Universal.SystemMACAddress + ".sdf";
                if (LocalDataFileAccess.CopyLocalDbToAppdataIfNotExists(localDbFileName)) return UserDataPath + localDbFileName;
                else return null;
            }
        }

        public static string EraseFile(string path)
        {
            try
            {
                File.Delete(path);
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string AppDataPath
        {
            get
            {
                try
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\";
                    path += "Dragenger\\";
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    return path;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static string UserDataPath
        {
            get
            {
                try
                {
                    string path = AppDataPath + "UserData\\";
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    return path;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
