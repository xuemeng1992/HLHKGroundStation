using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MAVTOOL;
using MAVTOOL.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HLHKGroundStation
{
    public class SetWayPointClass
    {
        public GMapOverlay realRouteOverlay;//动态显示飞行器层

        public GMapMarker vehiVleMarker;//显示载具类型

        public GMapOverlay realVehicleOverlay;//显示实时航线层

        public GMapOverlay markersOverlay_sec;//标记航点层

        public  List<PointLatLngAlt> totalWPlist = new List<PointLatLngAlt>();// 规划航点队列，包含了全部规划航点信息。航点0为家

        public List<WayPointList> wayPointLists = new List<WayPointList>();//当前位置点的坐标集合，每两秒钟在地图上更新一次

        public bool Enable_Draw;

        int num;

        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HLHKPlanner));

        public SetWayPointClass()
        {
            Enable_Draw = true;
            num = 0;
        }

        public void DrawWaypoint(PointLatLng point_end, GMapOverlay markerGmap)
        {
            if (Enable_Draw == true)
            {
                if (wayPointLists.Count == 90)
                {
                    wayPointLists.Clear();//超过90(大概3分钟)个元素就清空，降低计算机资源使用
                    markerGmap.Polygons.Clear();
                }
            
                wayPointLists.Add(new WayPointList(DateTime.Now, point_end));
                if (wayPointLists.Count == 1) return ;
                List<PointLatLng> points;
                GMapPolygon line;
                points = new List<PointLatLng>();
                points.Add(wayPointLists[wayPointLists.Count - 2].Point);
                points.Add(point_end);
                line = new GMapPolygon(points, "");
                line.Stroke = new Pen(Color.RoyalBlue, 4);
                markerGmap.Polygons.Add(line);
            }
            Enable_Draw = false;
        }

            /**
        * 功 能：任意角度选择bitmap图像，参数有旋转角度和输出颜色
        * 
        * 说 明：地图上的载具bitmap具有指向性
        * Ver 变更日期 负责人 变更内容
        * ───────────────────────────────────
        * V0.01 2018/4/23 潇齐 天祈 初版
        *
        * Copyright (c) 2015 Lir Corporation. All rights reserved.
        *┌──────────────────────────────────┐
        *│　阿木实验室完成代码注释　                                          │
        *│　版权所有：阿木实验室 　　　　　　　　　　　　　　                 │
        *└──────────────────────────────────┘
*/
        public static Bitmap KiRotate(Bitmap bmp, float angle, Color bkColor)
        {
            int w = bmp.Width + 2;
            int h = bmp.Height + 2;

            PixelFormat pf;

            if (bkColor == Color.Transparent)
            {
                pf = PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            Bitmap tmp = new Bitmap(w, h, pf);
            Graphics g = Graphics.FromImage(tmp);
            g.Clear(bkColor);
            g.DrawImageUnscaled(bmp, 1, 1);
            g.Dispose();

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, w, h));
            Matrix mtrx = new Matrix();
            mtrx.Rotate(angle);
            RectangleF rct = path.GetBounds(mtrx);

            Bitmap dst = new Bitmap((int)rct.Width, (int)rct.Height, pf);
            g = Graphics.FromImage(dst);
            g.Clear(bkColor);
            g.TranslateTransform(-rct.X, -rct.Y);
            g.RotateTransform(angle);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(tmp, 0, 0);
            g.Dispose();

            tmp.Dispose();

            return dst;
        }

        /**
        * 功 能：addvehiclemarker添加一个载具图标到地图
        * 
        * 说 明：realRouteOverlay动态显示飞行器层添加一个图标
        * Ver 变更日期 负责人 变更内容
        * ───────────────────────────────────
        * V0.01 2018/4/23 潇齐 天祈 初版
        *
        * Copyright (c) 2015 Lir Corporation. All rights reserved.
        *┌──────────────────────────────────┐
        *│　阿木实验室完成代码注释　                                          │
        *│　版权所有：阿木实验室 　　　　　　　　　　　　　　                 │
        *└──────────────────────────────────┘
*/
        public void addvehiclemarker(double lat, double lng, float angle)
        {
            if (vehiVleMarker != null)
            {
                realRouteOverlay.Markers.Remove(vehiVleMarker);//删除上一个地址的图标
            }
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);

                Size size = new Size(32,32);
                Bitmap bitmap = new Bitmap(global::HLHKGroundStation.Properties.Resources.无人机_copy,size);//在picture文件夹下找到要加载的图标
               // global::Amov.Planner.Properties.Resources.
                vehiVleMarker = new GMarkerGoogle(point, KiRotate(bitmap,angle,Color.Transparent));//得到根据yaw角指向的图标
                realRouteOverlay.Markers.Add(vehiVleMarker);//添加该图标到这个图层

            }
            catch (Exception) { }
        }


       


        /// <summary>
        /// 在地图上添加一个航点标志
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <param name="alt"></param>
        public void addpolygonmarker_sec(string tag, double lat, double lng, double alt, Color? color)
        {
            try
            {
                PointLatLng point = new PointLatLng(lat, lng);
                GMapMarkerWP m = new GMapMarkerWP(point, tag);
                m.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                m.ToolTipText = "Alt: " + alt.ToString("0");
                m.Tag = tag;
                markersOverlay_sec.Markers.Add(m);//添加标志
            }
            catch (Exception) { }
        }


        /// <summary>
        /// 在两个规划航点之间画一条直线路径
        /// </summary>
        /// <param name="point_start"></param>
        /// <param name="point_end"></param>
        public void drawLineLinkTwoWP(PointLatLng point_start, PointLatLng point_end, GMapOverlay markerGmap)
        {
            List<PointLatLng> points;
            GMapPolygon line;
            points = new List<PointLatLng>();
            points.Add(point_start);
            points.Add(point_end);
            line = new GMapPolygon(points, "");
            line.Stroke = new Pen(Color.Yellow, 4);
            markerGmap.Polygons.Add(line);
           // markerGmap.Polygons.Count;
        }


        /// <summary>
        /// 画出连接全部航点的航路（直线型）
        /// </summary>
        public void reDrawAllRoute()
        {
            //清除多边形
            markersOverlay_sec.Polygons.Clear();
            //然后重新画航点间直线
            for (int i = 0; i < totalWPlist.Count; i++)
            {
                if (i == totalWPlist.Count - 1)
                    //画最后一个航点到第一个航点的直线
                    drawLineLinkTwoWP(totalWPlist[totalWPlist.Count - 1], totalWPlist[0], markersOverlay_sec);
                else drawLineLinkTwoWP(totalWPlist[i], totalWPlist[i + 1], markersOverlay_sec);
                
            }
        }

        public void reDrawAllWP()
        {
            ////重新将各航点做标记，第一个航点为H，其它航点从1顺序编号
            for (int i = 0; i < totalWPlist.Count; i++)
            {
                if (i == 0) totalWPlist[i].Tag = "H";
                else totalWPlist[i].Tag = i.ToString();
            }
           // 清除地图上的全部航点标志
            markersOverlay_sec.Markers.Clear();
            //重画全部航点
            for (int i = 0; i < totalWPlist.Count; i++)
            {
                addpolygonmarker_sec(totalWPlist[i].Tag, totalWPlist[i].Lat, totalWPlist[i].Lng, totalWPlist[i].Alt, null);
            }
        }

       


    }
}
