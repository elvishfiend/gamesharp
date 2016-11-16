using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharpStatic.CartTypes
{
    internal abstract class CartBase
    {
        internal byte[] fileHeader = new byte[0x150]; // 336 bytes in the header
        internal byte[] rom;
        internal byte[] ram;

        internal int romPage;
        internal int romPageCount = 2;
        internal int romPageSize = 16348;
        internal int romSize;

        internal int ramPage;
        internal int ramPageCount = 0;
        internal int ramPageSize = 0;
        internal int ramSize;

        FileStream fileStream;

        public void LoadRom(string FilePath)
        {
            fileStream = new FileStream(FilePath, FileMode.Open);

            fileStream.Read(fileHeader, 0, fileHeader.Length);

            SetRomSize();
            this.rom = new byte[romSize];
            fileStream.Read(rom, 0, romSize);

            SetRamSize();
            this.ram = new byte[ramSize];
        }

        public void ReadSavFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Create))
                fs.Read(ram, 0, ram.Length);
        }

        public void WriteSavFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Create))
                fs.Write(ram, 0, ram.Length);
        }

        private void SetRomSize()
        {
            romPageSize = 16348;

            switch (fileHeader[0x148])
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

                switch (fileHeader[0x149])
                {
                    default:
                    case 0:
                        ramSize = 0;
                        ramPageSize = 0;
                        ramPageCount = 0;
                        break;
                    case 1:
                        ramSize = 2048; // 2 kB on 1x 2kB page
                        ramPageSize = 2048;
                        ramPageCount = 1;
                        break;
                    case 2:
                        ramSize = 8192; // 8 kB on 1x 8kB page
                        ramPageSize = 8192;
                        ramPageCount = 1;
                        break;
                    case 3:
                        ramSize = 32768; // 32 kB on 4x 8kB pages
                        ramPageSize = 8192;
                        ramPageCount = 4;
                        break;
                    case 4:
                        ramSize = 131072; // 128 kB on 16x 8kB pages
                        ramPageSize = 8192;
                        ramPageCount = 16;
                        break;
                }

                if (ramSize != 0)
                    ram = new byte[ramSize];
                else
                    ram = null;
            }

        public abstract byte ReadRam(ushort addr);

        public abstract byte ReadRom(ushort addr);

        public abstract void WriteRam(ushort addr, byte val);

        public abstract void WriteRom(ushort addr, byte val);
    }
}
