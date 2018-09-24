using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TM2Unswizzler
{
    public static class Palette
    {
        public static byte[][] GetPalette(byte[] file, TM2Header header)
        {
            byte[][] result;
            switch(header.TM2ImageFormat)
            {
                case TM2ImageFormat.Format8bbp:
                    result = getPalette8bbp(file, header);
                    break;
                default:
                    throw new ArgumentException("Unimplemented image format");
            }

            return result;
        }

        private static byte[][] getPalette8bbp(byte[] file, TM2Header header)
        {
            byte[][] palette = new byte[256][];

            
            for (int i = 0; i < 256; ++i)
            {
                byte[] rgba = new byte[4];
                Array.Copy(file, 0x40 + header.ImageSize + i * 4, rgba, 0, 4);

                // Correct alpha channel (0-128) -> (0-255)
                rgba[3] = (byte)(Math.Min((int)((int)rgba[3] << 1), 0xFF));

                // Swap color channels (RGBA -> BGRA)
                byte temp = rgba[2];
                rgba[2] = rgba[0];
                rgba[0] = temp;

                palette[(i & 0xe7) | ((i & 0x10) >> 1) | ((i & 0x08) << 1)] = rgba;
            }

            return palette;
        }
    }
}
