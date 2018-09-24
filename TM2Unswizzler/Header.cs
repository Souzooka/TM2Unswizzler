using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TM2Unswizzler
{
    public enum TM2ImageFormat : byte
    {
        Format16bpp = 1,
        Format24bpp = 2,
        Format32bpp = 3,
        Format4bbp = 4,
        Format8bbp = 5
    }

    public enum ImageType : byte
    {
        unk0 = 0
    }

    public enum ClutType : byte
    {
        unk0 = 0
    }

    public unsafe struct TM2Header
    {
        public fixed byte Label[4]; // ASCII label "TIM2"
        public byte Version;
        public byte Format;
        fixed byte padding[8];
        public uint TotalSize;
        public uint ClutSize;
        public uint ImageSize;
        public ushort HeaderSize;
        public ushort ClutColors;
        public TM2ImageFormat TM2ImageFormat;
        public byte MipMapTextures;
        public ClutType ClutType;
        ImageType ImageType;
        public ushort Width;
        public ushort Height;
        public ulong GsTex0;
        public ulong GsText1;
        public uint GsReg;
        public uint GsTexClut;

        public static TM2Header GetHeader(byte[] file)
        {
            // Read header of TM2 into struct
            byte[] headerBuf = new byte[Marshal.SizeOf<TM2Header>()];
            Array.Copy(file, headerBuf, headerBuf.Length);
            GCHandle handle = GCHandle.Alloc(headerBuf, GCHandleType.Pinned);
            TM2Header header = (TM2Header)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(TM2Header));
            handle.Free();
            return header;
        }
    }
}
