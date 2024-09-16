using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next_Home
{
    internal class Bluetooth_Connection_Exception : Exception
    {
        public Bluetooth_Connection_Exception() { }

        public Bluetooth_Connection_Exception(string message) : base(message) { }

        public Bluetooth_Connection_Exception(string message, Exception innerException) : base(message, innerException) { }
    }
}
