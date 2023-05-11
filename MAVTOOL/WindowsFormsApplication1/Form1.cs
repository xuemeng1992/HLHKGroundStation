using ANOGround;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Device dv = new Device("192.168.4.1", 23, 57600);
            dv.readmavlinkEvent += dv_readmavlinkEvent;
            dv.Start();
        }

        void dv_readmavlinkEvent(ANOGround.mavlink.MAVState MAV)
        {
            this.label1.Text = ("MAV-pitch:" + MAV.cs.pitch + " roll:" + MAV.cs.roll);
        }
    }
}
