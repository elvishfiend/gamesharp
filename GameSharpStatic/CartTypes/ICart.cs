using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharpStatic.CartTypes
{
    interface ICart
    {
        void ReadRomFile(string RomPath);

        void LoadRamFile(string RamPath);

        void SaveRamFile(string RamPath);

        byte ReadRom(ushort addr);

        byte ReadRam(ushort addr);

        void WriteRom(ushort addr, byte val);

        void WriteRam(ushort addr, byte val);
    }
}
