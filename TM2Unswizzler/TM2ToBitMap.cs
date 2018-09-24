using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TM2Unswizzler
{
    public static class TM2ToBitMap
    {
        private static int GetAddress8BppSwizzle(int width, int height, int x, int y)
        {
            int block = (y & (~0x0f)) * width + (x & (~0x0f)) * 2;
            int swap = (((y + 2) >> 2) & 0x01) * 4;
            int line = (((y & (~0x03)) >> 1) + (y & 0x01)) & 0x07;
            int column = line * width * 2 + ((x + swap) & 0x07) * 4;
            int offset = ((y >> 1) & 0x01) + ((x >> 2) & 0x02);
            return block + column + offset;
        }

        public static byte[] GetImageBuffer(byte[] tm2file)
        {
            TM2Header header = TM2Header.GetHeader(tm2file);
            byte[][] palette = Palette.GetPalette(tm2file, header);

            // TODO -- different images based on image type
            byte[] imageBuf = new byte[4 * header.Width * header.Height];
            for (int y = 0; y < header.Height; ++y)
            {
                for (int x = 0; x < header.Width; ++x)
                {
                    byte c = tm2file[0x40 + GetAddress8BppSwizzle(header.Width, header.Height, x, y)];
                    Array.Copy(palette[c], 0, imageBuf, (x * 4) + (header.Width * y) * 4, 4);
                }
            }

            return imageBuf;
        }
    }
}
