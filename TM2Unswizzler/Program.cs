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

    unsafe struct Header
    {
        public fixed byte Label[4]; // ASCII label "TIM2"
        ushort us1;
        ushort us2;
        uint ui1;
        uint ui2;
        uint ui3;
        uint ui4;
        public uint ImageSize;
        ushort us3;
        ushort us4;
        byte ub1;
        byte ub2;
        byte ub3;
        byte ub4;
        public ushort Width;
        public ushort Height;
        uint ui5;
        uint ui6;
        uint ui7;
        uint ui8;
        uint ui9;
        uint ui10;
    }

    class Program
    {
        static int GetAddress(int width, int height, int x, int y)
        {
            int block = (y & (~0x0f)) * width + (x & (~0x0f)) * 2;
            int swap = (((y + 2) >> 2) & 0x01) * 4;
            int line = (((y & (~0x03)) >> 1) + (y & 0x01)) & 0x07;
            int column = line * width * 2 + ((x + swap) & 0x07) * 4;
            int offset = ((y >> 1) & 0x01) + ((x >> 2) & 0x02);
            return block + column + offset;
        }

        static void Main(string[] args)
        {
            // This program works by having a file dragged onto it. If this program is launched without a file arg, exit
            if (args.Length == 0) { args = new string[] { "./loading.tm2" }; }
            int headerSize = 0x40;

            using (BinaryReader tm2File = new BinaryReader(File.Open(args[0], FileMode.Open)))
            {
                // Read header of TM2 into struct
                byte[] headerBuf = tm2File.ReadBytes(headerSize);
                GCHandle handle = GCHandle.Alloc(headerBuf, GCHandleType.Pinned);
                Header header = (Header)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(Header));
                handle.Free();

                // palette
                byte[] palette = new byte[256 * 4];

                for (int i = 0; i < 256; ++i)
                {
                    tm2File.BaseStream.Position = headerSize + header.ImageSize + i * 4;
                    byte[] rgba = tm2File.ReadBytes(4);
                    rgba[3] = (byte)(Math.Min((int)((int)rgba[3] << 1), 0xFF));
                    byte temp = rgba[2];
                    rgba[2] = rgba[0];
                    rgba[0] = temp;


                    Array.Copy(rgba, 0, palette, ((i & 0xe7) | ((i & 0x10) >> 1) | ((i & 0x08) << 1)) * 4, 4);
                }

                //image
                byte[] imageBuf = new byte[4 * header.Width * header.Height];
                for (int y = 0; y < header.Height; ++y)
                {
                    for (int x = 0; x < header.Width; ++x)
                    {
                        tm2File.BaseStream.Position = headerSize + GetAddress(header.Width, header.Height, x, y);
                        int c = tm2File.ReadByte();
                        Array.Copy(palette, c * 4, imageBuf, (x * 4) + (header.Width * y) * 4, 4); 
                    }
                }

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
}
