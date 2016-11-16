using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharpStatic
{
    internal static class MMU
    {
        public static bool InBios = true;

        private static Bios bios = new Bios();
        private static byte[] cart = new byte[0x8000];
        private static byte[] sram = new byte[0x2000];
        private static byte[] io = new byte[0x100];
        private static byte[] vram = new byte[0x2000];
        private static byte[] oam = new byte[0x100];
        private static byte[] wram = new byte[0x2000];
        private static byte[] hram = new byte[0x80];

        private static byte[] zram;
        private static byte[] eram;

        public static byte GetByte(ushort Addr)
        {
            switch (Addr & 0xF000)
            {
                // ROM bank 0
                case 0x0000:
                    if (InBios)
                    {
                        if (Addr < 0x0100)
                        {
                            return bios[Addr];
                        }
                        break;
                    }
                    else
                    {
                        return cart[Addr];
                    }

                case 0x1000:
                case 0x2000:
                case 0x3000:
                    return cart[Addr];

                // ROM bank 1
                case 0x4000:
                case 0x5000:
                case 0x6000:
                case 0x7000:
                    return cart[Addr];

                // VRAM
                case 0x8000:
                case 0x9000:
                    return vram[Addr & 0x1FFF];

                // External RAM
                case 0xA000:
                case 0xB000:
                    return eram[Addr & 0x1FFF];

                // Work RAM and echo
                case 0xC000:
                case 0xD000:
                case 0xE000:
                    return wram[Addr & 0x1FFF];

                // Everything else
                case 0xF000:
                    switch (Addr & 0x0F00)
                    {
                        // Echo RAM
                        case 0x000:
                        case 0x100:
                        case 0x200:
                        case 0x300:
                        case 0x400:
                        case 0x500:
                        case 0x600:
                        case 0x700:
                        case 0x800:
                        case 0x900:
                        case 0xA00:
                        case 0xB00:
                        case 0xC00:
                        case 0xD00:
                            return wram[Addr & 0x1FFF];

                        // OAM
                        case 0xE00:
                            return ((Addr & 0xFF) < 0xA0) ? oam[Addr & 0xFF] : (byte)0;

                        // Zeropage RAM, I/O
                        case 0xF00:
                            if (Addr > 0xFF7F)
                            {
                                return zram[Addr & 0x7F];
                            }
                            else
                                switch (Addr & 0xF0)
                                {
                                }
                            break;
                    }
                    break;
            }

            return 0x00;
        }
        public static byte GetByte(int Addr) { return GetByte((ushort)Addr); }

        public static ushort GetShort(ushort Addr)
        {
            return (ushort)((GetByte(Addr) << 8) + GetByte((ushort)(Addr + (ushort)1)));
        }
        public static ushort GetShort(int Addr) { return GetShort((ushort)Addr); }

        public static void WriteByte(ushort Addr, byte Value)
        {
            switch (Addr & 0xF000)
            {
                // ROM bank 0
                case 0x0000:
                    if (InBios && Addr < 0x0100) return;
                    break;
                // fall through
                case 0x1000:
                case 0x2000:
                case 0x3000:
                    break;

                // ROM bank 1
                case 0x4000:
                case 0x5000:
                case 0x6000:
                case 0x7000:
                    break;

                // VRAM
                case 0x8000:
                case 0x9000:
                    vram[Addr & 0x1FFF] = Value;
                    //updatetile(addr & 0x1FFF, val); // TODO
                    break;

                // External RAM
                case 0xA000:
                case 0xB000:
                    eram[Addr & 0x1FFF] = Value;
                    break;

                // Work RAM and echo
                case 0xC000:
                case 0xD000:
                case 0xE000:
                    wram[Addr & 0x1FFF] = Value;
                    break;

                // Everything else
                case 0xF000:
                    switch (Addr & 0x0F00)
                    {
                        // Echo RAM
                        case 0x000:
                        case 0x100:
                        case 0x200:
                        case 0x300:
                        case 0x400:
                        case 0x500:
                        case 0x600:
                        case 0x700:
                        case 0x800:
                        case 0x900:
                        case 0xA00:
                        case 0xB00:
                        case 0xC00:
                        case 0xD00:
                            wram[Addr & 0x1FFF] = Value;
                            break;

                        // OAM
                        case 0xE00:
                            if ((Addr & 0xFF) < 0xA0) oam[Addr & 0xFF] = Value;
                            //updateoam(addr, val); // TODO
                            break;

                        // Zeropage RAM, I/O
                        case 0xF00:
                            if (Addr > 0xFF7F) { zram[Addr & 0x7F] = Value; }
                            else switch (Addr & 0xF0)
                                {
                                }
                            break;
                    }
                    break;
            }
        }
        public static void WriteByte(int Addr, byte Value) { WriteByte((ushort)Addr, Value); }

        public static void WriteShort(ushort Addr, ushort Value)
        {
            WriteByte(Addr, (byte)(Value >> 8 & 0xFF));
            WriteByte((byte)(Addr + 1), (byte)(Value & 0xFF));
        }
        public static void WriteShort(int Addr, ushort Value) { WriteShort((ushort)Addr, Value); }

    }
}
