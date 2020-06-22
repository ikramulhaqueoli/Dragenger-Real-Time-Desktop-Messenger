using Display;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRepositories
{
    public class ImageFileAccess : FileIOAccess
    {
        //profile image file access APIs
        public static Image GetProfileImage(string profileImageId)
        {
            return GetImage(ProfileImagePath(profileImageId));
        }
        public static bool SaveProfileImage(Image profileImage, string profileImageId)
        {
            return SaveImage(profileImage, ImageFormat.Png, ProfileImagePath(profileImageId));
        }
        public static Image GetImage(string address)
        {
            try
            {
                Image img = Image.FromFile(address);
                return img;
            }
            catch
            {
                return null;
            }
        }
        public static bool EraseProfileImage(string profileImageId)
        {
            return FileIOAccess.EraseFile(ProfileImagePath(profileImageId));
        }
        public static string ProfileImagePath(string profileImageId)
        {
            return FileIOAccess.ProfileImageDirectory + profileImageId;
        }

        //file access APIs
        public static bool SaveImage(Image img, ImageFormat format, string targetFilePath)
        {
            try
            {
                using(FileStream fs = File.OpenWrite(targetFilePath))
                {
                    img.Save(fs, format);
                    fs.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
