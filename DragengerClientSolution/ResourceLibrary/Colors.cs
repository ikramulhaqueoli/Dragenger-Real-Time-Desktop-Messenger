using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace ResourceLibrary
{
    public static class Colors
    {
        public static Color ErrorTextColor
        {
            get { return Color.FromArgb(211, 57, 57); }
        }

        public static Color DragengerRed
        {
            get { return Color.FromArgb(255, 0, 0); }
        }

        public static Color DragengerOrange
        {
            get { return Color.FromArgb(255,90,0); }
        }

        public static Color DragengerLightOrange
        {
            get { return Color.FromArgb(255, 154, 0); }
        }

        public static Color DragengerDeepYellow
        {
            get { return Color.FromArgb(255, 206, 0); }
        }

        public static Color DragengerYellow
        {
            get { return Color.FromArgb(255, 232, 8); }
        }

        public static Color DragengerTileColor
        {
            get { return Color.FromArgb(221, 221, 221); }
        }
    }
}
