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
    public partial class AirSpeedIndicator : UserControl
    {
        public AirSpeedIndicator()
        {
            InitializeComponent();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (e.ClipRectangle.Width == 0 || e.ClipRectangle.Height == 0)
                return;

            using (Bitmap bmp = new Bitmap((int)e.Graphics.VisibleClipBounds.Width, (int)e.Graphics.VisibleClipBounds.Height))
            {
                Graphics g = Graphics.FromImage(bmp);
                Pen whitePen = new Pen(Brushes.White, 2.0f);

                g.Clear(Color.FromArgb(57, 49, 66));



                // 姿态仪的中心位置，即天地分割线的中点。
                // 绘制俯仰角标尺时，每2.5°为15像素，因此俯仰角每变动1°，标尺应移动6像素，
                // 故中心线也移动6像素
                Point center = new Point((int)bmp.Width / 2, (int)(bmp.Height / 2 + _dblAirSpeed * 3));

                // 可视的中心位置，即 LCD 的中心，该点在移动刻度时会用到
                Point visibleCenter = new Point((int)bmp.Width / 2, (int)(bmp.Height / 2));

                Font indicatorFont = new Font("Time NewRoman", 9);
                double stringWidth = 0;
                // 绘制空速表刻度线
                for (int i = 0; i <= 500; i+=10)
                {
                    g.DrawLine(whitePen, (float)(bmp.Width - 10.0), (float)(bmp.Height/2 - 3.0 * i + _dblAirSpeed * 3), (float)(bmp.Width), (float)(bmp.Height/2 - 3.0 * i + _dblAirSpeed * 3));
                    stringWidth = g.MeasureString(i.ToString(), indicatorFont).Width;
                    if (i % 20 == 0)
                        g.DrawString(i.ToString(), indicatorFont, Brushes.White, (float)(bmp.Width - 10.0 - stringWidth), (float)(bmp.Height / 2 - 3.0 * i - 8 + _dblAirSpeed * 3));
                }

                Point[] indicator = new Point[8];
                indicator[0] = new Point(bmp.Width - 45, visibleCenter.Y - 15);
                indicator[1] = new Point(bmp.Width - 15, visibleCenter.Y - 15);
                indicator[2] = new Point(bmp.Width - 15, visibleCenter.Y - 5);
                indicator[3] = new Point(bmp.Width - 10, visibleCenter.Y);
                indicator[4] = new Point(bmp.Width - 15, visibleCenter.Y + 5);
                indicator[5] = new Point(bmp.Width - 15, visibleCenter.Y + 15);
                indicator[6] = new Point(bmp.Width - 45, visibleCenter.Y + 15);
                indicator[7] = new Point(bmp.Width - 45, visibleCenter.Y - 15);
                g.FillPolygon(new SolidBrush(Color.FromArgb(16, 16, 49)), indicator);
                g.DrawLines(whitePen, indicator);
                indicatorFont = new Font("Time NewRoman", 11);
                //stringWidth = g.MeasureString(((int)this._dblAirSpeed).ToString("D3"), indicatorFont).Width;
                //double stringHeight = g.MeasureString(this._dblAirSpeed.ToString("D3"), indicatorFont).Height;
                g.DrawString(((int)this._dblAirSpeed).ToString("D3"), indicatorFont, Brushes.White, bmp.Width - 44, visibleCenter.Y - 10);
                e.Graphics.DrawImage(bmp, 0, 0);
            }
            base.OnPaint(e);
        }

        private void AirSpeedIndicator_Load(object sender, EventArgs e)
        {

        }
    }
}
