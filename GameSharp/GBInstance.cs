using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharp
{
    class GBInstance
    {
        public GBInstance(GB instance)
        {
            this.instance = instance;
        }

        public GB instance;

        public GB.MMU mmu
        {
            get { return instance.mmu; }
        }

        public GB.CPU cpu
        {
            get { return instance.cpu; }
        }

        public GB.Cart cart
        {
            get { return instance.cart; }
        }

        


    }
}
