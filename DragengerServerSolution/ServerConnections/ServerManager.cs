using Display;
using Microsoft.Owin.Hosting;
using System;

namespace ServerConnections
{
    public class ServerManager
    {
        private static IDisposable signalrWebAppServer;
        public static bool StartServer(string url)
        {
            try
            {
                if (url.Length < 5) throw new Exception();
                signalrWebAppServer = WebApp.Start<Startup>(url);
                return true;
            }
            catch(Exception ex)
            {
                string errorMsg = "Failed to run the server at [" + url + "].\nCheck URL validity and permissions!" + "\nException message: ";
                if (ex.InnerException != null) errorMsg += ex.InnerException.Message;
                else errorMsg += ex.Message;
                Output.Error(errorMsg);
                return false;
            }

            //Test the hub
            {
                //(new ServerHub()).LoginWithCredentials(MathLaboratory.Cryptography.Encrypt("7085C255A2A9\nallahIs1!"));
            }
        }

        public static bool TryStopServer()
        {
            ServerManager.signalrWebAppServer.Dispose();
            return true;
        }
    }
}
