using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TM2Unswizzler
{

    class Program
    {
        static void Main(string[] args)
        {
            // This program works by having a file dragged onto it. If this program is launched without a file arg, exit
            if (args.Length == 0) { return; }

            byte[] tm2file = File.ReadAllBytes(args[0]);
            byte[] imageBuf = TM2ToBitMap.GetImageBuffer(tm2file);
            TM2Header header = TM2Header.GetHeader(tm2file);


            unsafe
            {
                fixed (byte* ptr = imageBuf)
                {
                    using (Bitmap image = new Bitmap(header.Width, header.Height, header.Width * 4,
                       PixelFormat.Format32bppArgb, new IntPtr(ptr)))
                    {
                        image.Save(@"export.png");
                    }
                }
            }
        }
    }
}
