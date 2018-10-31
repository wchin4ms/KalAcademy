using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp
{
    class NSFException : Exception
    {
        public NSFException(string message) : base(message)
        {
        }
    }
}
