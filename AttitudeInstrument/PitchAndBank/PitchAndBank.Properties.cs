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
    public partial class PitchAndBank
    {
        /// <summary>
        /// 俯仰角
        /// </summary>
        public double Pitch
        {
            get
            {
                return _dblUpAndDown;
            }
            set
            {
                _dblUpAndDown = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// 侧倾角
        /// </summary>
        public double Bank
        {
            get
            {
                return _dblRoll;
            }
            set
            {
                _dblRoll = value;
                this.Refresh();
            }
        }


    }
}