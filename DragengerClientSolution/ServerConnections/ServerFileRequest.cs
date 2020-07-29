using EntityLibrary;
using FileIOAccess;
using Newtonsoft.Json.Linq;
using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
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
                    string fileLink = "http://" + ConfigurationManager.AppSettings["serverIp"] + ":" + ConfigurationManager.AppSettings["xamppPort"] + "/ProfileImages/" + profileImageId;
                    Console.WriteLine("ServerFileRequest.cs line 85: " + fileLink);
                    string targetLocalPath = FileResources.ProfileImgFolderPath + profileImageId;
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.DownloadFile(fileLink, targetLocalPath);
                    }
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
            try
            {
                string fileLink = "http://" + ConfigurationManager.AppSettings["serverIp"] + ":" + ConfigurationManager.AppSettings["xamppPort"] + "/ContentFiles/" + nuntias.Id;
                Console.WriteLine("ServerFileRequest.cs line 107: " + fileLink);
                string targetLocalPath = FileResources.NuntiasContentFolderPath + nuntias.ContentFileId;
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(fileLink, targetLocalPath);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception in ServerFileRequest:DownloadAndStoreContentFile() => " + ex.Message);
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
