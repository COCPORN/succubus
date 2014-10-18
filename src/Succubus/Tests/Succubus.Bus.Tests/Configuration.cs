using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Bus.Tests
{
    static class Configuration
    {
        static IFactory factory = new InMemoryFactory();

        public static IFactory Factory { get { return factory; } }
    }
}
