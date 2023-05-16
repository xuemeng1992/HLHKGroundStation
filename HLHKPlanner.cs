using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MAVTOOL;
using MAVTOOL.mavlink;
using MAVTOOL.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HLHKGroundStation
{
    public partial class HLHKPlanner : Form
    {
        private Device dv1;
        private GBL_FLAG gbl_flag;
        private SetWayPointClass swpc = new SetWayPointClass();//航点图层操作显示类
        private DateTime timemousedown;
        private int timemillSecond = 200;
        private int mousedistance = 30;
        private int mousepress_x = 0, mousepress_y = 0;
        private bool isMouseDrag = false;//false:鼠标单击 true:鼠标拖拽
        private bool isMouseDown = false;
        private bool isMarkerEnter = false;
        private bool isMarkerDrag = false;

        public HLHKPlanner()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            new TabPadding(tabControl1);
            gbl_flag = new GBL_FLAG();
            gbl_flag.flight = false;//地面站打开，系统默认为飞控不处于飞行状态
            gbl_flag.bRoutePlan = false;//地面站打开，系统默认飞控不处于航线规划状态
            tableLayoutPanel11.Visible = false;
            Commands.Visible = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            dv1 = new Device();
            dv1.linkEvent += Dv1_linkEvent;
        }
        private void Dv1_linkEvent(MAVState MAV, string Ip)
        {
            pitchAndBank1.Bank = MAV.cs.roll;
            pitchAndBank1.Pitch = MAV.cs.pitch;
            airSpeedIndicator1.AirSpeed = (int)MAV.cs.groundspeed;
            altitudeMeter1.Altitude = MAV.cs.alt;

            label_height.Text = MAV.cs.alt.ToString("0.##");//高度
            label_speed.Text = MAV.cs.groundspeed.ToString("0.##");//地速
            label_distance.Text = MAV.cs.wp_dist.ToString("0.##");//航点距离
            label_yaw.Text = MAV.cs.yaw.ToString("0.##");//偏航
            label_verticalspeed.Text = MAV.cs.verticalspeed.ToString("0.##");//升降速度
            label_disttohome.Text = MAV.cs.DistToHome.ToString("0.##");//距离home点
            if (dv1.comPort.isconnect)
            {
                button_connect.BackgroundImage = global::HLHKGroundStation.Properties.Resources.链接;
            }
            LinkqualityGcsLab.Text = MAV.cs.linkqualitygcs.ToString() + "%";//显示数传连接质量
            Hdop_label.Text = MAV.cs.gpshdop.ToString();//显示GPS水平定位因子
            //gMap.Position = new PointLatLng(MAV.cs.lat, MAV.cs.lng);
            //swpc.addvehiclemarker(MAV.cs.lat, MAV.cs.lng, MAV.cs.yaw);

        }
        private void gMap_Load(object sender, EventArgs e)
        {
            gMap.CacheLocation = System.Windows.Forms.Application.StartupPath + "\\GMapCache\\";
            gMap.MapProvider = GMapProviders.AMap;
            GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            gMap.ShowCenter = false;//显示中心的红色十字
            gMap.DragButton = System.Windows.Forms.MouseButtons.Left;  //左键拖动地图            
            //设置地图分辨率信息
            gMap.MaxZoom = 20;
            gMap.MinZoom = 1;
            gMap.Zoom = 15;
            gMap.Position = new PointLatLng(34.34726882, 108.94646555);
            //创建一个飞行动画层，用于动态显示飞行器的飞行状态
            Bitmap bitmap = new Bitmap(50, 50);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            swpc.realVehicleOverlay = new GMapOverlay("realvehicle");
            gMap.Overlays.Add(swpc.realVehicleOverlay);

            //创建实时航路层，用于显示实时航线
            swpc.realRouteOverlay = new GMapOverlay("realroute");
            gMap.Overlays.Add(swpc.realRouteOverlay);

            //创建一个marker层，用于标记航点
            swpc.markersOverlay_sec = new GMapOverlay("markers_sec");
            gMap.Overlays.Add(swpc.markersOverlay_sec);

            gMap.MouseClick += gMapControl1_MouseClick;
            gMap.MouseDown += gMapControl1_MouseDown;

            gMap.OnMarkerEnter += gMapControl1_OnMarkerEnter;
            gMap.OnMarkerLeave += gMapControl1_OnMarkerLeave;
            gMap.MouseMove += gMapControl1_MouseMove;
            gMap.MouseUp += gMapControl1_MouseUp;
        }
        void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMarkerEnter && isMouseDown)
            {
                isMarkerDrag = true;
            }
            if (!isMouseDown)
            {
                isMarkerDrag = false;
            }
        }
        void gMapControl1_OnMarkerLeave(GMapMarker item)
        {
            isMarkerEnter = false;
        }
        void gMapControl1_OnMarkerEnter(GMapMarker item)
        {
            isMarkerEnter = true;
        }
        void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (timemousedown.AddMilliseconds(timemillSecond) > DateTime.Now)
            {
                isMouseDrag = false;
            }
            else
            {
                isMouseDrag = true;
            }

            int x_dis = (mousepress_x > e.X) ? mousepress_x - e.X : e.X - mousepress_x;
            int y_dis = (mousepress_y > e.Y) ? mousepress_y - e.Y : e.Y - mousepress_y;
            if (mousedistance < System.Math.Sqrt(x_dis * x_dis + y_dis * x_dis))
            {
                isMouseDrag = true;
            }
            if (!isMouseDrag && gbl_flag.bRoutePlan)
            {
                if (swpc.totalWPlist.Count == 0)
                {
                    swpc.totalWPlist.Add(new PointLatLngAlt(gMap.FromLocalToLatLng(e.X, e.Y).Lat, gMap.FromLocalToLatLng(e.X, e.Y).Lng, 0, "H"));
                    swpc.addpolygonmarker_sec("H", gMap.FromLocalToLatLng(e.X, e.Y).Lat, gMap.FromLocalToLatLng(e.X, e.Y).Lng, 0, null);
                    swpc.reDrawAllRoute();
                    return;
                }
                //将本点追加入航点链表
                //// 这些值经纬度参数和指令参数是写入正确的，还有默认海拔高度是20M，其他参数是0
                Commands.Rows.Add();//添加Commands行  

                //DataGridViewComboBoxCell cell = Commands.Rows[swpc.totalWPlist.Count - 1].Cells[0] as DataGridViewComboBoxCell;
                //cell.Value = (int)(MAVLink.MAV_CMD.WAYPOINT);
                Commands.Rows[swpc.totalWPlist.Count - 1].Cells[1].Value = 0;
                Commands.Rows[swpc.totalWPlist.Count - 1].Cells[2].Value = 0;
                Commands.Rows[swpc.totalWPlist.Count - 1].Cells[3].Value = 0;
                Commands.Rows[swpc.totalWPlist.Count - 1].Cells[4].Value = 0;
                Commands.Rows[swpc.totalWPlist.Count - 1].Cells[5].Value = gMap.FromLocalToLatLng(e.X, e.Y).Lat;
                Commands.Rows[swpc.totalWPlist.Count - 1].Cells[6].Value = gMap.FromLocalToLatLng(e.X, e.Y).Lng;
                Commands.Rows[swpc.totalWPlist.Count - 1].Cells[7].Value = 20;//默认高度20M

                swpc.totalWPlist.Add(new PointLatLngAlt(gMap.FromLocalToLatLng(e.X, e.Y).Lat, gMap.FromLocalToLatLng(e.X, e.Y).Lng, 20, swpc.totalWPlist.Count.ToString()));///这里一定不能使用默认航速default_WP_spd
                swpc.addpolygonmarker_sec((swpc.totalWPlist.Count - 1).ToString(), gMap.FromLocalToLatLng(e.X, e.Y).Lat, gMap.FromLocalToLatLng(e.X, e.Y).Lng, 20, null);
                swpc.reDrawAllRoute();
            }
        }
        void gMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            timemousedown = DateTime.Now;
            mousepress_x = e.X;
            mousepress_y = e.Y;

            isMouseDown = true;
        }
        void gMapControl1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }
        private void CMB_comport_Click(object sender, EventArgs e)
        {
            CMB_comport.DataSource = SerialPort.GetPortNames();
        }
        private void button_connect_Click(object sender, EventArgs e)
        {
            dv1.ComPort = CMB_comport.Text;//从Combox控件里面获取端口名称
            dv1.Baud = int.Parse(CMB_baudrate.Text);//Combox控件里面获取波特率
            if (dv1.comPort.isconnect == false)//判断地面站是否已经连接上飞控
            { 
                dv1.Start(); 
            }
            if (dv1.comPort.isconnect == true)//已经连接上，在按一次就断开连接
            {
                dv1.comPort.isconnect = false;
                dv1.Stop();
                button_connect.BackgroundImage = global::HLHKGroundStation.Properties.Resources.链接22;//更换图标
            }
        }
        private void Airplane_button_Click(object sender, EventArgs e)
        {
            if (gbl_flag.bRoutePlan) 
            {
                tableLayoutPanel11.Visible = false;
                Commands.Visible = false;
                gbl_flag.bRoutePlan = false;
            }
        }
        private void Waypoint_button_Click(object sender, EventArgs e)
        {
            if (!gbl_flag.bRoutePlan)
            {
                tableLayoutPanel11.Visible = true;
                Commands.Visible = true;
                gbl_flag.bRoutePlan = true;
            }
        }
    }
}
