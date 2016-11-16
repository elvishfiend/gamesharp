using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharp
{
    public partial class GB
    {
        internal MMU mmu;
        internal CPU cpu;
        internal Cart cart;

        public GB()
        {
            mmu = new MMU(this);
            cpu = new CPU(this);
            cart = new Cart();
        }

    }
}
