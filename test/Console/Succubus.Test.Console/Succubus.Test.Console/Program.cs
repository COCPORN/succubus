using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SuccubusTest.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Test t = new Test();
            t.Run();
            new AutoResetEvent(false).WaitOne();
        }
    }
}
