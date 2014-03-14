using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Succubus.Core.Interfaces;

namespace Succubus.Core.MessageFrames
{
    public class Diagnostic : IMessageFrame
    {
        public string EmbeddedType { get; set; }

        public object Message { get; set; }
    }
}
