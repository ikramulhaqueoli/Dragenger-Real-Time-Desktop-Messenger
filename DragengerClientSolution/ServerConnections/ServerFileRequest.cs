using EntityLibrary;
using FileIOAccess;
using Newtonsoft.Json.Linq;
using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerConnections
{
    public class ServerFileRequest
    {
        public static string ChangeProfileImage(Image gotImg)
        {
            using (Bitmap roundedBmp = new Bitmap(GraphicsStudio.ClipToCircle(gotImg), new Size(200, 200)))
            {
                JObject profileImgIdJson = null;
                byte[] imgByteArray = Universal.ImageToByteArray(roundedBmp, gotImg.RawFormat);
                ServerHub.WorkingInstance.ServerHubProxy.Invoke<JObject>("ChangeProfileImage", Consumer.LoggedIn.Id, imgByteArray).ContinueWith(task =>
                {
                    if (!task.IsFaulted)
                    {
                        profileImgIdJson = task.Result;
                    }
                }).Wait();
                if (profileImgIdJson == null) return null;
                try
                {
                    string oldProfileImgId = profileImgIdJson["old_image_id"].ToString();
                    if (oldProfileImgId != null && oldProfileImgId.Length > 5)
                    {
                        LocalDataFileAccess.EraseOldProfileImageFromLocalData(oldProfileImgId);
                    }
                    string newImgId = profileImgIdJson["new_image_id"].ToString();
                    LocalDataFileAccess.SaveProfileImageToLocal(roundedBmp, newImgId);
                    return newImgId;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occured in ChangeProfileImage() => " + e.Message);
                    return null;
                }
            }
        }

        public static long? SendContentedNuntias(Nuntias newNuntias)
        {
            JObject nuntiasJsonData = newNuntias.ToJson();
            nuntiasJsonData["sender_mac_address"] = Universal.SystemMACAddress;
            long? nuntiasId = null;
            string filePath = LocalDataFileAccess.GetFilePathInLocalData(newNuntias.ContentFileId);
            if (filePath == null) return null;
            byte[] fileByte = Universal.FileToByteArray(filePath);
            KeyValuePair<JObject, byte[]> nuntiasData = new KeyValuePair<JObject,byte[]>(nuntiasJsonData,fileByte);
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<long>("SendContentedNuntias", nuntiasData).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    nuntiasId = task.Result;
                }
                else
                {
                    Console.WriteLine(task.IsFaulted);
                }
            }).Wait();
            return nuntiasId;
        }

        public static bool RefetchProfileImage(string profileImageId)
        {
            try
            {
                if (!LocalDataFileAccess.ProfileImgExistsInLocalData(profileImageId))
                {
                    if (profileImageId == null || profileImageId.Length == 0) return false;
                    byte[] fetchedImageByteArray = null;
                    ServerHub.WorkingInstance.ServerHubProxy.Invoke<byte[]>("GetProfileImageByProfileImgId", profileImageId).ContinueWith(task =>
                    {
                        if (!task.IsFaulted)
                        {
                            fetchedImageByteArray = task.Result;
                        }
                    }).Wait();
                    Image profileImg = Universal.ByteArrayToImage(fetchedImageByteArray);
                    return LocalDataFileAccess.SaveProfileImageToLocal(profileImg, profileImageId);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ServerFileRequest:RefetchProfileImage() => Exception: " + ex.Message);
                return false;
            }
        }

        public static void DownloadAndStoreContentFile(Nuntias nuntias)
        {
            if (nuntias.ContentFileId == null || nuntias.ContentFileId == "deleted" || nuntias.ContentFileId.Length == 0 || LocalDataFileAccess.ContentExistsInLocalData(nuntias.ContentFileId)) return;
            byte[] fileBytes = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<byte[]>("GetNuntiasContentFile", nuntias.Id).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    fileBytes = task.Result;
                }
            }).Wait();
            if (fileBytes == null) Console.WriteLine("File downloading failed!");
            else
            {
                Console.WriteLine("File downloading success!");
                LocalDataFileAccess.SaveNuntiasContentToLocal(new MemoryStream(fileBytes, false), nuntias.ContentFileId);
            }
        }

        //testing methods
        public static void SendFileToServer()
        {
            bool? result = null;
            ServerHub.WorkingInstance.ServerHubProxy.Invoke<bool>("ReceiveFile", Universal.ImageToByteArray(FileResources.NullProfileImage, ImageFormat.Png)).ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    result = task.Result;
                }
            }).Wait();
            if (result == null) Console.WriteLine("File sending failed");
            else Console.WriteLine("File sending success");
        }
    }
}
