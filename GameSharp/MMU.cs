using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharp
{
    public partial class GB
    {
        internal class MMU : GBInstance
        {
            public MMU(GB Instance) : base(Instance)
            {
                this.instance = Instance;
            }

            public bool InBios = true;

            private byte[] bios = new Bios().bios;
            private byte[] cart = new byte[0x8000];
            private byte[] sram = new byte[0x2000];
            private byte[] io = new byte[0x100];
            private byte[] vram = new byte[0x2000];
            private byte[] oam = new byte[0x100];
            private byte[] wram = new byte[0x2000];
            private byte[] hram = new byte[0x80];

            private byte[] zram;
            private byte[] eram;

            public byte GetByte(ushort Addr)
            {
                switch (Addr & 0xF000)
                {
                    // ROM bank 0
                    case 0x0000:
                        if (this.InBios)
                        {
                            if (Addr < 0x0100)
                            {
                                return this.bios[Addr];
                            }
                            break;
                        }
                        else
                        {
                            return this.cart[Addr];
                        }

                    case 0x1000:
                    case 0x2000:
                    case 0x3000:
                        return this.cart[Addr];

                    // ROM bank 1
                    case 0x4000:
                    case 0x5000:
                    case 0x6000:
                    case 0x7000:
                        return this.cart[Addr];

                    // VRAM
                    case 0x8000:
                    case 0x9000:
                        return this.vram[Addr & 0x1FFF];

                    // External RAM
                    case 0xA000:
                    case 0xB000:
                        return this.eram[Addr & 0x1FFF];

                    // Work RAM and echo
                    case 0xC000:
                    case 0xD000:
                    case 0xE000:
                        return this.wram[Addr & 0x1FFF];

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
                                return this.wram[Addr & 0x1FFF];

                            // OAM
                            case 0xE00:
                                return ((Addr & 0xFF) < 0xA0) ? this.oam[Addr & 0xFF] : (byte)0;

                            // Zeropage RAM, I/O
                            case 0xF00:
                                if (Addr > 0xFF7F)
                                {
                                    return this.zram[Addr & 0x7F];
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
            public byte GetByte(int Addr) { return GetByte((ushort)Addr); }

            public ushort GetShort(ushort Addr)
            {
                return (ushort)((GetByte(Addr) << 8) + GetByte((ushort)(Addr + (ushort)1)));
            }
            public ushort GetShort(int Addr) { return GetShort((ushort)Addr); }

            public void WriteByte(ushort Addr, byte Value)
            {
                switch (Addr & 0xF000)
                {
                    // ROM bank 0
                    case 0x0000:
                        if (this.InBios && Addr < 0x0100) return;
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
                        this.vram[Addr & 0x1FFF] = Value;
                        //this.updatetile(addr & 0x1FFF, val); // TODO
                        break;

                    // External RAM
                    case 0xA000:
                    case 0xB000:
                        this.eram[Addr & 0x1FFF] = Value;
                        break;

                    // Work RAM and echo
                    case 0xC000:
                    case 0xD000:
                    case 0xE000:
                        this.wram[Addr & 0x1FFF] = Value;
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
                                this.wram[Addr & 0x1FFF] = Value;
                                break;

                            // OAM
                            case 0xE00:
                                if ((Addr & 0xFF) < 0xA0) this.oam[Addr & 0xFF] = Value;
                                //this.updateoam(addr, val); // TODO
                                break;

                            // Zeropage RAM, I/O
                            case 0xF00:
                                if (Addr > 0xFF7F) { this.zram[Addr & 0x7F] = Value; }
                                else switch (Addr & 0xF0)
                                    {
                                    }
                                break;
                        }
                        break;
                }
            }
            public void WriteByte(int Addr, byte Value) { WriteByte((ushort)Addr, Value); }

            public void WriteShort(ushort Addr, ushort Value)
            {
                WriteByte(Addr, (byte)(Value >> 8 & 0xFF));
                WriteByte((byte)(Addr + 1), (byte)(Value & 0xFF));
            }
            public void WriteShort(int Addr, ushort Value) { WriteShort((ushort)Addr, Value); }

        }
    }
}
