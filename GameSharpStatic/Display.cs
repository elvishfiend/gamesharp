using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace GameSharpStatic
{
    static class Display
    {

        private static Canvas canvas = new Canvas();

        private static byte[] sprite = new byte[20];

        private static byte[] background = new byte[1024]; // 32 x 32 tiles

        private static bool LcdControlOperation // tile map display select
        {
            get { return Registers.LCDC.GetBit(7); }
        }
        private static bool WindowTileMapDisplaySelect
        {
            get { return Registers.LCDC.GetBit(6); }
        }
        private static bool WindowDisplay
        {
            get { return Registers.LCDC.GetBit(5); }
        }
        private static bool BgWindowTileDataSelect
        {
            get { return Registers.LCDC.GetBit(4); }
        }
        private static bool BgTileMapDisplaySelect
        {
            get { return Registers.LCDC.GetBit(3); }
        }
        private static bool ObjSize
        {
            get { return Registers.LCDC.GetBit(2); }
        }
        private static bool ObjDisplay
        {
            get { return Registers.LCDC.GetBit(1); }
        }
        private static bool BgWindowDisplay
        {
            get { return Registers.LCDC.GetBit(0); }
        }


        private static int clock;
        private static int Clock
        {
            get { return clock; }
            set
            {
                if (value > 70223)
                    clock = 0;
                else
                    clock = value;
            }
        }

        public static void tick()
        {
            // set STAT registers
        }

        private static void UpdateDisplay()
        {

        }
    }
}
