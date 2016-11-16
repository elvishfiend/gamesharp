using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharp
{
    partial class GB
    {
        internal class Cart
        {
            Cartridge cartridge;

            byte[] romHeader = new byte[0x14F];

            public void LoadRom(string path)
            {
                using (var romStream = System.IO.File.OpenRead(path))
                {
                    if (romHeader == null)
                    {
                        romHeader = new byte[0x14F];
                    }

                    romStream.Read(romHeader, 0, 0x14F);
                }

            }

            
            private bool cartLoaded = false;

            private byte romType;
            private byte romBankSize; // size of rom banks in kB
            private byte romBankNum; // number of rom banks in cart
            private byte romMode = 0;

            private byte ramSize; // size of ram bank in kB
            private byte ramBanks; // number of rambanks


            private byte[] file;

            public void LoadCart(string Path)
            {
                this.file = System.IO.File.ReadAllBytes(Path);
                this.GetCartType();
                this.GetRomSize();
                this.GetRamSize();

                cartLoaded = true;
            }

            private void loadBank(ushort bank)
            {
                if (!cartLoaded)
                    return;

                if (bank == 0)
                    return;

                var bankSize = 0x4000;

                var bankStart = bank * bankSize;
                var bankEnd = ((bank + 1) * bankSize > file.Length) ? file.Length : (bank + 1) * bankSize;

                // copy over to ROM2
                for (int i = 0; i < bankSize; i++)
                {
                    if (bankStart + i > file.Length)
                        Rom2[i] = 0;
                    else
                        Rom2[i] = file[bankStart + i];
                }
            }

            byte[] Rom1 = new byte[0x4000];
            byte[] Rom2 = new byte[0x4000];

            private void GetCartType()
            {
                var cartType = Rom1[0x0147];
                cartridge = carts.First(x => x.cartType == cartType);
            }

            private void GetRomSize()
            {
                romBankSize = 16;

                switch (file[0x148])
                {
                    case 0: romBankNum = 2; break;
                    case 1: romBankNum = 4; break;
                    case 2: romBankNum = 8; break;
                    case 3: romBankNum = 16; break;
                    case 4: romBankNum = 32; break;
                    case 5: romBankNum = 64; break;
                    case 6: romBankNum = 128; break;
                    case 0x52: romBankNum = 72; break;
                    case 0x53: romBankNum = 80; break;
                    case 0x54: romBankNum = 96; break;
                    default: throw new Exception("Unsupported ROM type");
                }
            }

            private void GetRamSize()
            {
                switch (file[0x149])
                {
                    case 0: ramBanks = 0; break;
                    case 1: ramBanks = 1; ramSize = 2; break;
                    case 2: ramBanks = 1; ramSize = 8; break;
                    case 3: ramBanks = 4; ramSize = 8; break;
                    case 4: ramBanks = 16; ramSize = 16; break;
                }
            }

            // Pokemon Rom Type: ROM + MBC3 + RAM + Battery
            public void WriteByte(ushort addr, byte value)
            {
            }

            enum MemControlType
            {
                NONE = 0,
                MBC1 = 1,
                MBC2 = 5,
                MMM01 = 0xB,
                MBC3 = 0x12,
                MBC5 = 0x19
            }

            class Cartridge
            {
                public byte cartType;
                public MemControlType memController = MemControlType.NONE;
                public bool hasRam = false;
                public bool hasBattery = false;
                public bool hasRumble = false;
                public bool hasSram = false;
            }

            List<Cartridge> carts = new List<Cartridge>()
            {
                new Cartridge() { cartType = 0 },
                new Cartridge() { cartType = 1, memController = MemControlType.MBC1 },
                new Cartridge() { cartType = 2, memController = MemControlType.MBC1, hasRam = true },
                new Cartridge() { cartType = 3, memController = MemControlType.MBC1, hasRam = true, hasBattery = true },
                new Cartridge() { cartType = 5, memController = MemControlType.MBC2 },
                new Cartridge() { cartType = 6, memController = MemControlType.MBC2, hasBattery = true },
                new Cartridge() { cartType = 8, hasRam = true },
                new Cartridge() { cartType = 9, hasRam = true, hasBattery = true },
                new Cartridge() {cartType = 0xB, memController = MemControlType.MMM01 },
                new Cartridge() {cartType = 0xC, memController = MemControlType.MMM01, hasSram = true },
                new Cartridge() {cartType = 0xD, memController = MemControlType.MMM01, hasSram = true, hasBattery = true },
                new Cartridge() { cartType = 0x12, memController = MemControlType.MBC3, hasRam = true },
                new Cartridge() { cartType = 0x13, memController = MemControlType.MBC3, hasRam = true, hasBattery = true },
                new Cartridge() { cartType = 0x19, memController = MemControlType.MBC5 },
                new Cartridge() { cartType = 0x1A, memController = MemControlType.MBC5, hasRam = true },
                new Cartridge() { cartType = 0x1B, memController = MemControlType.MBC5, hasRam = true, hasBattery = true },
                new Cartridge() { cartType = 0x1C, memController = MemControlType.MBC5, hasRumble = true },
                new Cartridge() { cartType = 0x1D, memController = MemControlType.MBC5, hasRumble = true, hasSram = true },
                new Cartridge() { cartType = 0x1E, memController = MemControlType.MBC5, hasRumble = true, hasSram = true, hasBattery = true }
            };
        }
    }
}
