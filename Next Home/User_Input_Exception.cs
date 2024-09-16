using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next_Home
{
    internal class User_Input_Exception : Exception
    {
        public User_Input_Exception() { }
        public User_Input_Exception(string message) : base(message) { }
        public User_Input_Exception(String message, Exception innerException) : base(message, innerException) { }
    }
}
