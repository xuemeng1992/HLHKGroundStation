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
    public partial class AltitudeMeter
    {

        /// <summary>
        /// 高度
        /// </summary>
        public double Altitude
        {
            get
            {
                return _dblAltitude;
            }
            set
            {
                _dblAltitude = value;
                this.Refresh();
            }
        }
    }
}



