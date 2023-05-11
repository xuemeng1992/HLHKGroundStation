using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using GMap.NET.WindowsForms;

using GeoUtility;
using GeoUtility.GeoSystem;
using MAVTOOL;
using System.Runtime.InteropServices;
using System.Media;
using System.Diagnostics;
using MAVTOOL.Utilities;
using MAVTOOL.Comms;
using MAVTOOL.Controls;
using System.Collections;
using MAVTOOL.mavlink;
using System.Net;
/**
* 功 能：mavlink设备对象
* 类 名：Device
* 说 明：串口类的通讯已经验证过，完整的类包含TCP/IP协议，这边版本还不包含（2018/5/17）
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

namespace MAVTOOL
{
    public delegate void readmavlink(MAVState MAV, string Ip);//声明一个委托
    public delegate void connectHander();
    public class Device
    {
        //进程之间传送数据
        const int WM_COPYDATA = 0x004A;
        /// <summary>
        ///
        /// </summary>
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
        /// <summary>
        /// enum of firmwares
        /// </summary>
        public enum Firmwares
        {
            ArduPlane,
            ArduCopter2,
            ArduRover,
            ArduSub,
            Ateryx,
            ArduTracker,
            Gymbal,
            PX4
        }


        //*************阿木社区编写(AMOV AUTO)*******************//
        //mavlinkinterface接口实例化，这个成员变量包含了全部的mavlink协议操作
        public MAVLinkInterface comPort
        {
            get
            {
                return _comPort;
            }
            set
            {
                if (_comPort == value)
                    return;

            }
        }
        //实例化MAVLinkInterface类,这个类里面都是mavlink的基础操作，比如发送命令，应答，设置航点等，是mavlink协议的主要实现接口
        MAVLinkInterface _comPort = new MAVLinkInterface();


        //串口读写线程,这个线程是读取飞控的线程
        Thread serialreaderthread;
        /// controls the main serial reader thread
        bool serialThread = false;
        //数据显示线程，从comPort.MAVlist.GetMAVStates的数据缓冲区读取数据显示
        Thread readcomdisplayThread;


        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);



        //*************************创建一个读串口对象*******************************
        UpdateComUiThread readcomdisplay;
        public float Speed { get; set; }

        IPEndPoint Post;
        public string IP = "127.0.0.1";
        public int Port = 21;

        int timetick = 0;
        public bool IsConnect
        {
            get
            {
                if (Environment.TickCount - timetick < 3000)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }



        }

        bool isOccupy = false;
        public int Baud { get; set; }
        public int PannelMode { get; set; }
        public int Index { get; set; }
        public string ComPort = "null";




        public bool IsEnable { get; set; }

        public int Time { get; set; }
        public Device(string ip, int port, int baud)
        {
            IP = ip;
            Port = port;
            Baud = 115200;
            Speed = 0;



        }
        public Device(string comport, int baud)
        {
            ComPort = comport;
            Baud = baud;
            Speed = 0;



        }
        public Device()
        {
            IsEnable = false;
            Time = 12;
            Speed = 0;

        }


        public void Start()
        {

            Speed = 0;
            isStop = true;
            if (isOccupy && comPort.isconnect)
            {
                return;
            }
            connectHander conn = new connectHander(Connect);
            conn.BeginInvoke(null, null);
        }
        public bool isStop = false;
        public void Stop()
        {
            if (isStop)
            {
                comPort.BaseStream.Close();
                serialThread = false;
                isStop = false;

            }
            isOccupy = false;
        }

        //串口或者TCP/IP协议链接函数，负责串口设备或者TCP/IP端口的打开
        private void Connect()
        {
            isOccupy = true;
            Post = new IPEndPoint(IPAddress.Parse(IP), Port);
            _comPort.ComPort = ComPort;
            if (comPort.BaseStream.IsOpen)
            {
                comPort.BaseStream.Close();
                isStop = true;
            }
            else
            {
                if ((comPort.BaseStream.PortName != null) && (comPort.BaseStream.PortName != "None"))
                {

                    try
                    {
                        bool skipconnectcheck = false;
                        comPort.BaseStream = new SerialPort();

                        comPort.MAV.cs.ResetInternals();//初始化接口
                        comPort.BaseStream.PortName = ComPort;
                        comPort.BaseStream.BaudRate = Baud;
                        //     prevent serialreader from doing anything
                        comPort.giveComport = true;
                        if (Settings.Instance.GetBoolean("CHK_resetapmonconnect") == true)
                        {
                            comPort.BaseStream.DtrEnable = false;
                            comPort.BaseStream.RtsEnable = false;
                            comPort.BaseStream.toggleDTR();
                        }
                        comPort.giveComport = false;


                        //     do the connect
                        comPort.Open(false,  skipconnectcheck);

                        if (!comPort.BaseStream.IsOpen)
                        {
                            try
                            {
                                MessageBox.Show("打开失败", "提示");
                                comPort.Close();

                            }
                            catch
                            {
                            }
                            return;
                        }
                        else
                        {
                            ;
                        }

                        // setup main serial reader 新建一个串口数据读线程
                        serialreaderthread = new Thread(SerialReader)
                        {
                            IsBackground = true,
                            Name = "Main display",
                            Priority = ThreadPriority.Normal
                        };
                        serialreaderthread.Start();//启动数据读线程


                        /*
                        这个
                        */
                        readcomdisplay = new UpdateComUiThread(comPort);
                        readcomdisplay.readmavlinkEvent += readcomdisplay_readmavlinkEvent;//绑定一个读取事件

                        //创建一个线程数据填充线程
                        readcomdisplayThread = new Thread(new ThreadStart(readcomdisplay.readmavlinkdelegate));
                        readcomdisplayThread.IsBackground = true;
                        readcomdisplayThread.Start();
                    }
                    catch (Exception ex)
                    {
                        //捕获到异常信息
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("请检出串口配置", "提示");
                }
                if (comPort.BaseStream.IsOpen)
                {


                    //  btn_com_open.Text = "关闭通信";
                }
            }
            isOccupy = false;
        }


        /// <summary>
        /// 着陆
        /// </summary>
        public void SetLand()
        {
            comPort.setMode(new MAVLink.mavlink_set_mode_t
            {
                //使能了用户模式选择
                base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
                //在指导模式下custom_mode = 0标示指导模式
                custom_mode = 0,
                target_system = comPort.MAV.sysid
            });
            System.Threading.Thread.Sleep(100);
            comPort.setMode(new MAVLink.mavlink_set_mode_t
            {
                //使能了用户模式选择
                base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
                //在着陆模式下custom_mode = 9 标示着陆模式
                custom_mode = 9,
                target_system = comPort.MAV.sysid
            });
        }

        public event readmavlink linkEvent;//声明读取事件
        void readcomdisplay_readmavlinkEvent(MAVState MAV, string Ip)
        {
            timetick = Environment.TickCount;
            Speed = MAV.cs.groundspeed;
            PannelMode = MAV.cs.PanelMode;

            if (linkEvent != null)
            {
                linkEvent.Invoke(MAV, Ip);//事件触发更新UI界面
            }
        }


        [DllImport("kernel32.dll")]
        public static extern void OutputDebugString(string str);

        private void SerialReader()//串口读写线程
        {
            if (serialThread == true)
                return;
            serialThread = true;

            while (serialThread)
            {
                if (comPort.BaseStream.IsOpen && comPort.BaseStream.BytesToRead > 0 &&
                          comPort.giveComport == false)
                {
                    try
                    {
                        comPort.readPacket();//去取数据流并且填充到类

                    }
                    catch (Exception ex)
                    {
                        ;
                    }

                }
                else
                {
                    Thread.Sleep(5);
                }
            }
            Thread.Sleep(1); // was 5
        }

    }
}

