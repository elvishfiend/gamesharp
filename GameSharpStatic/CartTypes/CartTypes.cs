using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharpStatic.CartTypes
{
    enum CartTypes
    {
        ROM = 0,
        ROM_MBC1 = 1,
        ROM_MBC1_RAM = 2,
        ROM_MBC1_RAM_BATT = 3,

        ROM_MBC2 = 5,
        ROM_MBC2_BATT = 6,

        ROM_RAM = 8,
        ROM_RAM_BATT = 9,

        ROM_MMM01 = 0xB,
        ROM_MMM01_SRAM = 0xC,
        ROM_MMM01_SRAM_BATT = 0xD,

        ROM_MBC3_TIMER_BATT = 0xF,
        ROM_MBC3_TIMER_RAM_BATT = 0x10,
        ROM_MBC3 = 0x11,
        ROM_MBC3_RAM = 0x12,
        ROM_MBC3_RAM_BATT = 0x13,

        ROM_MBC5 = 0x19,
        ROM_MBC5_RAM = 0x1A,
        ROM_MBC5_RAM_BATT = 0x1B,
        ROM_MBC5_RUMBLE = 0x1C,
        ROM_MBC5_RUMBLE_SRAM = 0x1D,
        ROM_MBC5_RUMBLE_SRAM_BATT = 0x1E,
        POCKET_CAMERA = 0x1F,

        BANDAI_TAMA5 = 0xFD,
        HUDSON_HUC_3 = 0xFE,
        HUDSON_HUC_1 = 0xFF



    }
}
