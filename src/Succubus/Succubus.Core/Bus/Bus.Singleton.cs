using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Core
{
    public partial class Bus
    {
        private static volatile Bus instance;
        private static object syncRoot = new Object();

        public static Bus Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Bus();
                    }
                }
                return instance;
            }
        }
    }
}
