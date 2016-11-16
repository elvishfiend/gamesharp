using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharpStatic
{
    static class Extensions
    {
        public static bool GetBit(this byte val, byte bitIndex)
        {
            if ((val & 1 << bitIndex) == 0)
                return false;
            else
                return true;
        }

        public static byte SetBit(this byte val, byte bitIndex)
        {
            return (byte)(val | 1 << bitIndex);
        }

        public static byte ClearBit(this byte val, byte bitIndex)
        {
            return (byte)(val & ~(1 << bitIndex));
        }
    }
}
