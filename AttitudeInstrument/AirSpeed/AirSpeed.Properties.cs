using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AttitudeInstrument
{
    public partial class AirSpeedIndicator
    {
        /// <summary>
        /// 空速
        /// </summary>
        public double AirSpeed
        {
            get
            {
                return _dblAirSpeed;
            }
            set
            {
                _dblAirSpeed = value;
                this.Invalidate();
            }
        }
    }
}
