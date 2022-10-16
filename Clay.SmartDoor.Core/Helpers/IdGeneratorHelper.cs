using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clay.SmartDoor.Core.Helpers
{
    public static class IdGeneratorHelper
    {
        public static string generateID()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
