using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSharp
{
    partial class GB
    {
        class Serial : GBInstance
        {
            public void DoSerial()
            {
                if (inTransaction)
                {
                }
                else if ((mmu.GetByte(0xFF02) | (1<<6)) != 0)
                {
                    // determine clock source

                    startTransaction();
                }
                
            }

            public Serial(GB instance) : base(instance)
            {

            }

            private void startTransaction()
            {
                this.serialBuffer = mmu.GetByte(0xFF01);
                //this TODO

            }

            private void endTransaction()
            {
                // TODO
            }

            private bool inTransaction = false;
            private byte serialBuffer = 0;
            private bool useExternalClock = false;
            //private byte 





        }
    }
}
