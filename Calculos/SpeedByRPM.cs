using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculos
{
    public class SpeedByRPM
    {
        public double Speed { get; set; }
        public int RPM { get; set; }

        public SpeedByRPM(double Speed, int RPM)
        {
            this.Speed = Speed;
            this.RPM = RPM;
        }
    }
}
