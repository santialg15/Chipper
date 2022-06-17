using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogServer.Exceptions
{
    public class MethodDoesNotExist : Exception
    {
        public MethodDoesNotExist()
            : base("El metodo no existe")
        {
        }
    }
}
