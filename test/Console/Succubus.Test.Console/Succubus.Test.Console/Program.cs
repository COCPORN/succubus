using System.Threading;

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
