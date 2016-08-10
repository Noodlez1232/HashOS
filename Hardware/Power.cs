using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HashOS;

namespace HashOS.Drivers
{
    public class Power
    {
        public static void ShutDown()
        {
            HAL.Power.ShutDown();
        }
        public static void Reboot()
        {
            HAL.Power.Reboot();
        }
        
    }
}
