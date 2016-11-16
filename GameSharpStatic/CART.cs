using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharpStatic
{
    internal static class CART
    {
        private static byte[] cart1 = new byte[16384];

        private static CartTypes.ROM cartProvider;

        private static CartTypes.CartTypes cartType;

        private static byte[] cartHeader = new byte[0x14F];

        public static void LoadCart(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                fileStream.Read(cartHeader, 0, cartHeader.Length);
            }

            cartType = (CartTypes.CartTypes)cartHeader[0x0147];
        }
    }
}
