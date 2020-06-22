using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceLibrary
{
    public static class GraphicsStudio
    {
        public static Image Overlap(Image source1, Image source2)
        {
            var target = new Bitmap(source1.Width, source1.Height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(target);
            graphics.CompositingMode = CompositingMode.SourceOver; // this is the default, but just to be clear

            graphics.DrawImage(source1, 0, 0);
            graphics.DrawImage(source2, 0, 0);

            return target;
        }

        public static Bitmap ResizeImageByHeight(Image image, int targetHeight)
        {
            double imgwidth = image.Width;
            double imgheight = image.Height;
            Bitmap resized = new Bitmap(image, new Size((int)(((double)targetHeight / imgheight) * imgwidth), (int)targetHeight));
            image.Dispose();
            return resized;
        }

        public static Image ClipToCircle(Image sourceImage)
        {
            int x = sourceImage.Width / 2;
            int y = sourceImage.Height / 2;
            int r = Math.Min(x, y);

            Bitmap targetImage = null;
            targetImage = new Bitmap(2 * r, 2 * r);
            using (Graphics g = Graphics.FromImage(targetImage))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TranslateTransform(targetImage.Width / 2, targetImage.Height / 2);
                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse(0 - r, 0 - r, 2 * r, 2 * r);
                Region rg = new Region(gp);
                g.SetClip(rg, CombineMode.Replace);
                Bitmap bmp = new Bitmap(sourceImage);
                g.DrawImage(bmp, new Rectangle(-r, -r, 2 * r, 2 * r), new Rectangle(x - r, y - r, 2 * r, 2 * r), GraphicsUnit.Pixel);

            }
            return targetImage;
        }
    }
}
