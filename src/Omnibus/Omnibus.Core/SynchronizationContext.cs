using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnibus
{
    class SynchronizationContext
    {
        public bool Static { get; set; }

        public List<SynchronizationFrame> Frames { get; set; }

        public SynchronizationContext()
        {
            Static = false;
            Frames = new List<SynchronizationFrame>();
        }
    }
}
