using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharpStatic
{
    class Byte
    {

        byte val;
        public Byte(byte b)
        {
            val = b;
        }

        public static implicit operator Byte(byte b)
        {
            return new Byte(b);
        }

        public static implicit operator byte(Byte b)
        {
            return b.val;
        }

        public static byte operator &(Byte left, byte right)
        {
            return (byte)(left.val & right);
        }

        public static byte operator ^(Byte left, byte right)
        {
            return (byte)(left.val ^ right);
        }

        public static byte operator |(Byte left, byte right)
        {
            return (byte)(left.val | right);
        }
    }
}
