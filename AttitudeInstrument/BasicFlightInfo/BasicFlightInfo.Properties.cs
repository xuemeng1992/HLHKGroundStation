using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AttitudeInstrument.BasicFlightInfo
{
    partial class BasicFlightInfo
	{
        /// <summary>
        /// 高度
        /// </summary>
        public double Altitude
        {
            get
            {
                return altitudeMeter.Altitude;
            }
            set
            {
                altitudeMeter.Altitude = value;
            }
        }

        /// <summary>
        /// 空速
        /// </summary>
        public double AirSpeed
        {
            get
            {
                return airSpeedIndicator.AirSpeed;
            }
            set
            {
                airSpeedIndicator.AirSpeed = value;
            }
        }

        /// <summary>
        /// 俯仰角
        /// </summary>
        public double Pitch
        {
            get
            {
                return pitchAndBank.Pitch;
            }
            set
            {
                pitchAndBank.Pitch = value;
            }
        }

        /// <summary>
        /// 侧倾角
        /// </summary>
        public double Bank
        {
            get
            {
                return pitchAndBank.Bank;
            }
            set
            {
                pitchAndBank.Bank = value;
            }
        }
	}
}
