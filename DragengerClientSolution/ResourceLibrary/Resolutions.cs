using System;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.Runtime.InteropServices;

namespace ResourceLibrary
{
    public static class Resolutions
    {
        [DllImport("User32.dll")]
        public static extern int SetProcessDPIAware();
        private static Size nativeSize;
        private static Size defaultFormSize;

        static Resolutions()
        {
            SetProcessDPIAware();
            nativeSize = Screen.PrimaryScreen.Bounds.Size;
            Resolutions.SetDefaultFormSize();
        }

        public static Size NativeSize
        {
            get { return Resolutions.nativeSize; }
        }

        public static void SetDefaultFormSize()
        {
            //settings for different resolutions will be modified here.

            int height = 0, width = 0;
            //height and font Adjustments
            if(Resolutions.nativeSize.Height >= 1000)
            {
                height = (int)(Resolutions.NativeSize.Height * 0.8f);
                Resolutions.MaxFontSize = 23.0f;
            }
            else if (Resolutions.nativeSize.Height >= 700)
            {
                height = (int)(Resolutions.NativeSize.Height * 0.9f);
                Resolutions.MaxFontSize = 21.0f;
            }
            else
            {
                height = (int)(Resolutions.NativeSize.Height * 0.95f);
                Resolutions.MaxFontSize = 19.0f;
            }

            //width Adjustments
            if (Resolutions.nativeSize.Width >= 1800)
            {
                width = (int)(Resolutions.NativeSize.Width * 0.4f);
            }
            else if (Resolutions.nativeSize.Width >= 1200)
            {
                width = (int)(Resolutions.NativeSize.Width * 0.5f);
            }
            else
            {
                height = (int)(Resolutions.NativeSize.Width * 0.55f);
            }

            Resolutions.DefaultFormSize = new Size(width, height);
        }

        public static float MaxFontSize
        {
            set;
            get;
        }
        
        public static Size DefaultFormSize
        {
            set { Resolutions.defaultFormSize = value; }
            get { return Resolutions.defaultFormSize; }
        }

    }
}
