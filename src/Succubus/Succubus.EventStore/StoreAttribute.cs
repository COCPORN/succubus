using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Stores.EventStore
{
    /// <summary>
    /// Indicate that this message should be put in the EventStore
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class StoreAttribute : Attribute
    {
        public string Stream { get; set; }

        public StoreAttribute() : this("Succubus")
        {
           
        }

        public StoreAttribute(string stream)
        {
            Stream = stream;
        }

    }
}
