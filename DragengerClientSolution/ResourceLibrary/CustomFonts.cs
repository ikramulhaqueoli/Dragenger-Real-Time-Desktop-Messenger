using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace ResourceLibrary
{
    public class CustomFonts
    {
        private static Font biggest, bigger, big, regular;
        private static Font smallest, smaller, small;
        private static Font biggestBold, biggerBold, bigBold, regularBold;
        private static Font nanoBold, smallestBold, smallerBold, smallBold;
        private static string formalFontName, stylishFontName;
        static CustomFonts()
        {
            CustomFonts.SetSuitableFontSize();
            CustomFonts.formalFontName = "Arial";
            CustomFonts.stylishFontName = "Segoe Script";
            biggest = new Font(formalFontName, CustomFonts.BiggestSize);
            bigger = new Font(formalFontName, CustomFonts.BiggerSize);
            big = new Font(formalFontName, CustomFonts.BigSize);
            regular = new Font(formalFontName, CustomFonts.RegularSize);
            small = new Font(formalFontName, CustomFonts.SmallSize);
            smaller = new Font(formalFontName, CustomFonts.SmallerSize);
            smallest = new Font(formalFontName, CustomFonts.SmallestSize);

            biggestBold = new Font(formalFontName, CustomFonts.BiggestSize, FontStyle.Bold);
            biggerBold = new Font(formalFontName, CustomFonts.BiggerSize, FontStyle.Bold);
            bigBold = new Font(formalFontName, CustomFonts.BigSize, FontStyle.Bold);
            regularBold = new Font(formalFontName, CustomFonts.RegularSize, FontStyle.Bold);
            smallBold = new Font(formalFontName, CustomFonts.SmallSize, FontStyle.Bold);
            smallerBold = new Font(formalFontName, CustomFonts.SmallerSize, FontStyle.Bold);
            smallestBold = new Font(formalFontName, CustomFonts.SmallestSize, FontStyle.Bold);
            nanoBold = new Font(formalFontName, CustomFonts.NanoSize, FontStyle.Bold);
        }
        public static Font New(float size, char style)
        {
            if (style == 'r' || style == 'R') return new Font(formalFontName, size, FontStyle.Regular);
            if (style == 'b' || style == 'B') return new Font(formalFontName, size, FontStyle.Bold);
            if (style == 'i' || style == 'I') return new Font(formalFontName, size, FontStyle.Italic);
            if (style == 's' || style == 'S') return new Font(formalFontName, size, FontStyle.Strikeout);
            if (style == 'u' || style == 'U') return new Font(formalFontName, size, FontStyle.Underline);
            return null;
        }
        public static Font New(string fontName, float size, char style)
        {
            if (style == 'r' || style == 'R') return new Font(fontName, size, FontStyle.Regular);
            if (style == 'b' || style == 'B') return new Font(fontName, size, FontStyle.Bold);
            if (style == 'i' || style == 'I') return new Font(fontName, size, FontStyle.Italic);
            if (style == 's' || style == 'S') return new Font(fontName, size, FontStyle.Strikeout);
            if (style == 'u' || style == 'U') return new Font(fontName, size, FontStyle.Underline);
            return null;
        }
        
        public static float BiggestSize
        {
            set;
            get;
        }
        public static float BiggerSize
        {
            set;
            get;
        }
        public static float BigSize
        {
            set;
            get;
        }
        public static float RegularSize
        {
            set;
            get;
        }
        public static float SmallSize
        {
            set;
            get;
        }
        public static float SmallerSize
        {
            set;
            get;
        }
        public static float SmallestSize
        {
            set;
            get;
        }
        public static float NanoSize
        {
            set;
            get;
        }
        private static void SetSuitableFontSize()
        {
            CustomFonts.BiggestSize = Resolutions.MaxFontSize;
            CustomFonts.BiggerSize = Resolutions.MaxFontSize - 3.0f;
            CustomFonts.BigSize = Resolutions.MaxFontSize - 5.0f;
            CustomFonts.RegularSize = Resolutions.MaxFontSize - 6.0f;
            CustomFonts.SmallSize = Resolutions.MaxFontSize - 8.0f;
            CustomFonts.SmallerSize = Resolutions.MaxFontSize - 11.0f;
            CustomFonts.SmallestSize = Resolutions.MaxFontSize - 13.5f;
            CustomFonts.NanoSize = Resolutions.MaxFontSize - 15.0f;
        }
        public static string StylishFontName
        { get { return CustomFonts.stylishFontName; } }
        public static Font Biggest
        { get { return CustomFonts.biggest; } }
        public static Font Bigger
        { get { return CustomFonts.bigger; } }
        public static Font Big
        { get { return CustomFonts.big; } }
        public static Font Regular
        { get { return CustomFonts.regular; } }
        public static Font Small
        { get { return CustomFonts.small; } }
        public static Font Smaller
        { get { return CustomFonts.smaller; } }
        public static Font Smallest
        { get { return CustomFonts.smallest; } }
        public static Font NanoBold
        { get { return CustomFonts.nanoBold; } }

        public static Font BiggestBold
        { get { return CustomFonts.biggestBold; } }
        public static Font BiggerBold
        { get { return CustomFonts.biggerBold; } }
        public static Font BigBold
        { get { return CustomFonts.bigBold; } }
        public static Font RegularBold
        { get { return CustomFonts.regularBold; } }
        public static Font SmallBold
        { get { return CustomFonts.smallBold; } }
        public static Font SmallerBold
        { get { return CustomFonts.smallerBold; } }
        public static Font SmallestBold
        { get { return CustomFonts.smallestBold; } }
    }
}
