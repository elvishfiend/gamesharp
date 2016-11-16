using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharpStatic.CartTypes
{
    public class ROM : ICart
    {
        internal List<byte> rom;
        internal List<byte> ram;

        internal int ramSize;
        internal int ramPage;
        internal int ramPageCount;
        internal int ramPageSize;

        internal int romSize;
        internal int romPage;
        internal int romPageCount;
        internal int romPageSize;

        internal bool hasBattery = false;
        internal bool hasRam = false;
        internal bool hasClock = false;

        public ROM(List<byte> rom, List<byte> ram)
        {
            this.rom = rom;
            this.ram = ram;

            this.SetRomSize();
            this.SetRamSize();
        }

        public void LoadRamFile(string path)
        {
            this.ram.Clear();
            byte[] _ram = System.IO.File.ReadAllBytes(path);
            this.ram.AddRange(_ram);
        }

        public void SaveRamFile(string path)
        {
            System.IO.File.WriteAllBytes(path, ram.ToArray());
        }

        public void ReadRomFile(string path)
        {
            this.rom.Clear();
            byte[] _rom = System.IO.File.ReadAllBytes(path);
            this.rom.AddRange(_rom);
        }

        public virtual byte ReadRom(ushort addr)
        {
            return CART.rom[addr];
        }

        public virtual void WriteRom(ushort addr, byte val)
        {
            // do nothing. ROM Only
        }

        public virtual byte ReadRam(ushort addr)
        {
            // do nothing. No Ram
            return 0;
        }

        public virtual void WriteRam(ushort addr, byte val)
        {
            // do nothing. No Ram
        }

        private void SetRomSize()
        {
            romPageSize = 16348;

            switch (rom[0x148])
            {
                default:
                case 0:
                    romPageCount = 2;
                    break;
                case 1:
                    romPageCount = 4;
                    break;
                case 2:
                    romPageCount = 8;
                    break;
                case 3:
                    romPageCount = 16;
                    break;
                case 4:
                    romPageCount = 32;
                    break;
                case 5:
                    romPageCount = 64;
                    break;
                case 6:
                    romPageCount = 128;
                    break;
                case 0x52:
                    romPageCount = 72;
                    break;
                case 0x53:
                    romPageCount = 80;
                    break;
                case 0x54:
                    romPageCount = 96;
                    break;
            }

            romSize = romPageCount * romPageSize;
        }

        private void SetRamSize()
        {
            ramPage = 0;
            ramPageSize = 8192;
            hasRam = true;

            switch (rom[0x149])
            {
                default:
                case 0:
                    hasRam = false;
                    ramSize = 0;
                    ramPageSize = 0;
                    ramPageCount = 0;
                    break;
                case 1: // 2 kB on 1x 2kB page
                    ramPageSize = 2048;
                    ramPageCount = 1;
                    break;
                case 2: // 8 kB on 1x 8kB page
                    ramPageCount = 1;
                    break;
                case 3: // 32 kB on 4x 8kB pages
                    ramPageCount = 4;
                    break;
                case 4: // 128 kB on 16x 8kB pages
                    ramPageCount = 16;
                    break;
            }
            ramSize = ramPageCount * ramPageSize;
        }

    }
}
