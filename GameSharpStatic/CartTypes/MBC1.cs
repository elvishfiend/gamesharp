using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharpStatic.CartTypes
{
    internal class MBC1 : ICart
    {
        private byte romPage;
        private byte romPageSize;

        private byte ramPage;
        private ushort ramPageSize = 8192;



        private byte[] rom;
        private byte[] ram;

        private bool ramEnabled = false;

        private bool mode = false; // 0 = 16Mbit ROM/8kByte Ram ; 1 = 4Mbit ROM / 32kByte Ram

        public void WriteByte(ushort addr, byte val)
        {
            if (addr > 0xBFFF || (addr > 0x7FFF && addr < 0xC000)) // invalid range
                    throw new IndexOutOfRangeException();

            if (addr >= 0x6000 && addr <= 0x7FFF) // mode select change
            {
                if ((val | 0x1) == 0)
                    mode = false;
                else
                    mode = true;

                return;
            }

            if (addr >= 0x2000 && addr <= 0x3FFF) // bank select
            {
                romPage = (byte)(val & 0x1F); // XXXBBBBB
                return;
            }

            if (mode && (addr >= 0x4000 && addr <= 0x5FFF)) // 4/32 mode RamBank at A000-C000
            {
                ramPage = (byte)(val & 0x03); // XXXXXXBB
                return;
            }

            if (mode && (addr >= 0x0000 && addr <= 0x1FFF)) // 4/32 mode Ram Enable
            {
                if ((val & 0x0F) == 0x0A) // XXXX1010 enables, anything else disables
                    ramEnabled = true;
                else
                    ramEnabled = false;
            }

            if (!mode && (addr >= 0x4000 && addr <= 0x3FFF)) // 16/8 mode ROM - bits 6/5 of ROM address
            {
                // need to test this
                romPage = (byte)(romPage + ((val & 0x03) << 5));
            }
            


        }

        public byte ReadByte(ushort addr)
        {
            if (addr > 0xBFFF || (addr > 0x7FFF && addr < 0xC000)) // invalid range
                throw new IndexOutOfRangeException();

            if (addr <= 0x3FFF)
                return rom[addr];

            else if (addr >= 0x4000 && addr <= 0x7FFF)
                return rom[addr + romPage * 8196];

            else if (addr >= 0xA000 && addr <= 0xBFFF)
                return ram[(addr & 0x1FFF) + ramPage * ramPageSize];

            else
                throw new IndexOutOfRangeException();
        }

        public byte ReadRom(ushort addr)
        {
            if (addr >= 0x8000)
                throw new IndexOutOfRangeException();
            else
            {
                if (addr <= 0x1FFF)
                    return rom[addr];

                if (romPage == 0 || romPage == 0x20 || romPage == 0x40 || romPage == 0x60)
                {
                    return rom[addr + (romPage + 1)*romPageSize];
                }
                else
                {
                    return rom[addr + romPage * romPageSize];
                }
            }    


        }

        public byte ReadRam(ushort addr)
        {

        }
    }
}
