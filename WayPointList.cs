using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLHKGroundStation
{
   public class WayPointList
    {
        public DateTime BulidTime ;//记录当前位置点的记录时间
        public PointLatLng Point;

        public WayPointList(DateTime bulidtime, PointLatLng point)
        {
            BulidTime = bulidtime;
            Point = point;
        }

    }
}
