using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharpStatic
{
    public static class Registers
    {
        public static byte P1 {
            get { return MMU.GetByte(0xFF00); }
            set { MMU.WriteByte(0xFF00, value); }
        }

        public static byte DIV
        {
            get { return MMU.GetByte(0xFF04); }
            set { MMU.WriteByte(0xFF04, 0); }
        }
        
        public static byte TIMA
        {
            get { return MMU.GetByte(0xFF05); }
            set { MMU.WriteByte(0xFF05, value); }
        }

        public static byte TMA
        {
            get { return MMU.GetByte(0xFF06); }
            set { MMU.WriteByte(0xFF06, value); }
        }
        public static byte TAC
        {
            get { return MMU.GetByte(0xFF07); }
            set { MMU.WriteByte(0xFF07, value); }
        }
        public static byte IF
        {
            get { return MMU.GetByte(0xFF0F); }
            set { MMU.WriteByte(0xFF0F, value); }
        }

        // TODO: Add all the sound registers (NR10 => NR52), WavePatternRam
        public static byte LCDC
        {
            get { return MMU.GetByte(0xFF40); }
            set { MMU.WriteByte(0xFF40, value); }
        }

        public static byte STAT
        {
            get { return MMU.GetByte(0xFF41); }
            set { MMU.WriteByte(0xFF41, value); }
        }
        public static byte SCY
        {
            get { return MMU.GetByte(0xFF42); }
            set { MMU.WriteByte(0xFF42, value); }
        }

        public static byte SCX
        {
            get { return MMU.GetByte(0xFF43); }
            set { MMU.WriteByte(0xFF43, value); }
        }
        public static byte LY
        {
            get { return MMU.GetByte(0xFF44); }
            set { MMU.WriteByte(0xFF44, value); }
        }
        public static byte LYC
        {
            get { return MMU.GetByte(0xFF45); }
            set { MMU.WriteByte(0xFF45, value); }
        }

        public static byte DMA
        {
            get { return MMU.GetByte(0xFF46); }
            set { MMU.WriteByte(0xFF46, value); }
        }

        public static byte BGP
        {
            get { return MMU.GetByte(0xFF47); }
            set { MMU.WriteByte(0xFF47, value); }
        }

        public static byte OBP0
        {
            get { return MMU.GetByte(0xFF48); }
            set { MMU.WriteByte(0xFF48, value); }
        }

        public static byte OBP1
        {
            get { return MMU.GetByte(0xFF49); }
            set { MMU.WriteByte(0xFF49, value); }
        }
        public static byte WY
        {
            get { return MMU.GetByte(0xFF4A); }
            set { MMU.WriteByte(0xFF4A, value); }
        }

        public static byte WX
        {
            get { return MMU.GetByte(0xFF4B); }
            set { MMU.WriteByte(0xFF4B, value); }
        }

        public static byte IE
        {
            get { return MMU.GetByte(0xFFFF); }
            set { MMU.WriteByte(0xFFFF, value); }
        }

    }
}
