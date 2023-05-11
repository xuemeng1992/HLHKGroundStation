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
    public partial class PitchAndBank : UserControl
    {

        public PitchAndBank()
        {
            InitializeComponent();
        }

        private Bitmap Rotate(Bitmap source, float angle, Point center)
        {
            Bitmap bmp = (Bitmap)source.Clone();
            Graphics g = Graphics.FromImage(bmp);
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform((float)this.Bank);
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(source, 0, 0);
            g.Dispose();
            g = null;
            return bmp;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (e.ClipRectangle.Width == 0 || e.ClipRectangle.Height == 0)
                return;


            System.Drawing.Size visibleSize = new System.Drawing.Size((int)e.Graphics.VisibleClipBounds.Width, (int)e.Graphics.VisibleClipBounds.Height);
            int factor = 2;
            using (Bitmap bmp = new Bitmap((int)e.Graphics.VisibleClipBounds.Width * factor, (int)e.Graphics.VisibleClipBounds.Height * factor))
            {// 如果不乘以2，则在旋转时会出现问题
                Graphics g = Graphics.FromImage(bmp);
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                // 姿态仪的中心位置，即天地分割线的中点。
                // 绘制俯仰角标尺时，每2.5°为15像素，因此俯仰角每变动1°，标尺应移动6像素，
                // 故中心线也移动6像素
                Point center = new Point((int)bmp.Width / 2, (int)(bmp.Height / 2 - this.Pitch * 6));
                // 可视的中心位置，即 LCD 的中心，该点在旋转刻度时会用到
                Point visibleCenter = new Point((int)bmp.Width / 2, (int)(bmp.Height / 2));

                g.Clear(Color.Red);
                g.FillRectangle(Brushes.SaddleBrown, new Rectangle(0, 0, bmp.Width, bmp.Height));


                //double center.Y = bmp.Height / 4.0 + UpAndDown * 4.0;

                g.FillRectangle(Brushes.DodgerBlue, 0, 0, bmp.Width, (float)(center.Y));

                Pen whitePen = new Pen(Brushes.White, 2.0f);
                // 绘制天地分割线
                g.DrawLine(whitePen, 0, (float)(center.Y), bmp.Width, (float)(center.Y));

                //g.FillEllipse(Brushes.Red, visibleCenter.X - 10, visibleCenter.Y - 10, 20, 20);

                // 绘制俯仰角刻度线
                for (int i = 1; i <= 36; i++)
                {
                    if (i % 4 == 0)
                    {
                        if (center.Y + 15.0 * i > visibleCenter.Y - bmp.Height / factor/2 + 50)
                        {
                            // 下面一半
                            g.DrawLine(whitePen, (float)(bmp.Width / 2.0 - 40.0), (float)(center.Y + 15.0 * i), (float)(bmp.Width / 2.0 + 40.0), (float)(center.Y + 15.0 * i));
                            g.DrawString((i * 10 / 4).ToString(), new Font("Time NewRoman", 12, FontStyle.Bold), Brushes.White, (float)(bmp.Width / 2.0 + 50.0), (float)(center.Y - 10 + 15.0 * i));
                        }

                        // 上面一半
                        if (center.Y - 15.0 * i > visibleCenter.Y - bmp.Height / factor/2 + 50)
                        {
                            g.DrawLine(whitePen, (float)(bmp.Width / 2.0 - 40.0), (float)(center.Y - 15.0 * i), (float)(bmp.Width / 2.0 + 40.0), (float)(center.Y - 15.0 * i));
                            g.DrawString((i * 10 / 4).ToString(), new Font("Time NewRoman", 12, FontStyle.Bold), Brushes.White, (float)(bmp.Width / 2.0 + 50.0), (float)(center.Y - 10 - 15.0 * i));
                        }

                    }
                    else if (i % 2 == 0)
                    {
                        if (center.Y + 15.0 * i > visibleCenter.Y - bmp.Height / factor/2 + 50)
                        {
                            g.DrawLine(whitePen, (float)(bmp.Width / 2.0 - 20.0), (float)(center.Y + 15.0 * i), (float)(bmp.Width / 2.0 + 20.0), (float)(center.Y + 15.0 * i));
                        }
                        if (center.Y - 15.0 * i > visibleCenter.Y - bmp.Height / factor/2 + 50)
                            g.DrawLine(whitePen, (float)(bmp.Width / 2.0 - 20.0), (float)(center.Y - 15.0 * i), (float)(bmp.Width / 2.0 + 20.0), (float)(center.Y - 15.0 * i));
                    }
                    else
                    {
                        if (center.Y + 15.0 * i > visibleCenter.Y - bmp.Height / factor/2 + 50)
                        {
                            g.DrawLine(whitePen, (float)(bmp.Width / 2.0 - 10.0), (float)(center.Y + 15.0 * i), (float)(bmp.Width / 2.0 + 10.0), (float)(center.Y + 15.0 * i));
                        }
                        if (center.Y - 15.0 * i > visibleCenter.Y - bmp.Height / factor/2 + 50)
                            g.DrawLine(whitePen, (float)(bmp.Width / 2.0 - 10.0), (float)(center.Y - 15.0 * i), (float)(bmp.Width / 2.0 + 10.0), (float)(center.Y - 15.0 * i));
                    }
                }

                // 绘制侧倾指示器，就是那个三角
                PointF[] anglePointer = new PointF[4];
                anglePointer[0] = new PointF(bmp.Width / 2.0f, visibleCenter.Y - bmp.Height / factor/2 + 20);
                anglePointer[1] = new PointF((float)(bmp.Width / 2 - bmp.Width * 0.02 * Math.Cos(Math.PI / 3)), (float)(visibleCenter.Y - bmp.Height / factor/2 + 30 + bmp.Width * 0.02 * Math.Sin(Math.PI / 3)));
                anglePointer[2] = new PointF((float)(bmp.Width / 2 + bmp.Width * 0.02 * Math.Cos(Math.PI / 3)), (float)(visibleCenter.Y - bmp.Height / factor/2 + 30 + bmp.Width * 0.02 * Math.Sin(Math.PI / 3)));
                anglePointer[3] = new PointF(bmp.Width / 2.0f, visibleCenter.Y - bmp.Height / factor/2 + 20);
                //g.DrawLines(whitePen, anglePointer);
                //g.DrawLine(Pens.Red, visibleCenter, anglePointer[0]);
                
                g.FillPolygon(Brushes.Yellow, anglePointer);
                g.ResetTransform();
                g.Save();


                // 旋转俯仰角刻度线
                Bitmap bmp2 = Rotate(bmp, (int)this.Bank, visibleCenter);
                //g.Clear(Color.SaddleBrown);
                g.DrawImage(bmp2, 0, 0);
                bmp2.Dispose();
                bmp2 = null;

                // 绘制侧倾角度器刻度
                double r1 = visibleCenter.Y - anglePointer[0].Y+30, r2 = r1-20 ;

                //g.DrawEllipse(Pens.Red, (float)(visibleCenter.X - r1), (float)(visibleCenter.Y-r1), (float)r1*2.0f,(float) r1*2.0f);

                double x1, y1, x2, y2;
                for (int i = -180; i <= 0; i += 10)
                {
                    if (i % 30 == 0)
                    {
                        x1 = r1 * Math.Cos(Math.PI * i / 180.0) + visibleCenter.X;
                        y1 = r1 * Math.Sin(Math.PI * i / 180.0) + visibleCenter.Y;
                        x2 = r2 * Math.Cos(Math.PI * i / 180.0) + visibleCenter.X;
                        y2 = r2 * Math.Sin(Math.PI * i / 180.0) + visibleCenter.Y;
                    }
                    else
                    {
                        x1 = (r1 - 10) * Math.Cos(Math.PI * i / 180.0) + visibleCenter.X;
                        y1 = (r1 - 10) * Math.Sin(Math.PI * i / 180.0) + visibleCenter.Y;
                        x2 = r2 * Math.Cos(Math.PI * i / 180.0) + visibleCenter.X;
                        y2 = r2 * Math.Sin(Math.PI * i / 180.0) + visibleCenter.Y;
                    }
                    g.DrawLine(whitePen, (float)x1, (float)y1, (float)x2, (float)y2);
                }

                //g.DrawLine(Pens.Red, 0, visibleCenter.Y, bmp.Width, visibleCenter.Y);
                //g.DrawLine(Pens.Red, visibleCenter.X, 0, visibleCenter.X, bmp.Height);

                // 绘制姿态仪中间的两个拐角
                Point[] centerIndicator=new Point[5];
                centerIndicator[0] = new Point(visibleCenter.X - 4, visibleCenter.Y );
                centerIndicator[1] = new Point(visibleCenter.X + 4, visibleCenter.Y );
                centerIndicator[2] = new Point(visibleCenter.X + 4, visibleCenter.Y+8 );
                centerIndicator[3] = new Point(visibleCenter.X - 4, visibleCenter.Y+8 );
                centerIndicator[4] = new Point(visibleCenter.X - 4, visibleCenter.Y );
                //g.FillPolygon(Brushes.Black, centerIndicator);
                g.DrawLines(whitePen, centerIndicator);

                Point[] leftIndicator = new Point[7];
                leftIndicator[0] = new Point(visibleCenter.X - 120, visibleCenter.Y);
                leftIndicator[1] = new Point(visibleCenter.X - 40, visibleCenter.Y);
                leftIndicator[2] = new Point(visibleCenter.X - 40, visibleCenter.Y+20);
                leftIndicator[3] = new Point(visibleCenter.X - 47, visibleCenter.Y+20);
                leftIndicator[4] = new Point(visibleCenter.X - 47, visibleCenter.Y+7);
                leftIndicator[5] = new Point(visibleCenter.X - 120, visibleCenter.Y+7);
                leftIndicator[6] = new Point(visibleCenter.X - 120, visibleCenter.Y);
                g.FillPolygon(Brushes.Black, leftIndicator);
                g.DrawLines(whitePen, leftIndicator);

                Point[] rightIndicator = new Point[7];
                rightIndicator[0] = new Point(visibleCenter.X + 120, visibleCenter.Y);
                rightIndicator[1] = new Point(visibleCenter.X + 40, visibleCenter.Y);
                rightIndicator[2] = new Point(visibleCenter.X + 40, visibleCenter.Y + 20);
                rightIndicator[3] = new Point(visibleCenter.X + 47, visibleCenter.Y + 20);
                rightIndicator[4] = new Point(visibleCenter.X + 47, visibleCenter.Y + 7);
                rightIndicator[5] = new Point(visibleCenter.X + 120, visibleCenter.Y + 7);
                rightIndicator[6] = new Point(visibleCenter.X + 120, visibleCenter.Y);
                g.FillPolygon(Brushes.Black, rightIndicator);
                g.DrawLines(whitePen, rightIndicator);

                e.Graphics.DrawImage(bmp, 0 - bmp.Width / factor / 2, 0 - bmp.Height / factor / 2);
                //e.Graphics.DrawImage(bmp, 0 ,0);
                g.Dispose();
            }


            // 绘制圆角控件
            Rectangle rect = new Rectangle(0, 0, base.Width, base.Height);
            int radius = 50;
            System.Drawing.Drawing2D.GraphicsPath roundRect = new System.Drawing.Drawing2D.GraphicsPath();
            //顶端 
            roundRect.AddLine(rect.Left + radius - 1, rect.Top - 1, rect.Right - radius, rect.Top - 1);
            //右上角 
            roundRect.AddArc(rect.Right - radius, rect.Top - 1, radius, radius, 270, 90);
            //右边 
            roundRect.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom - radius);
            //右下角
            roundRect.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            //底边 
            roundRect.AddLine(rect.Right - radius, rect.Bottom, rect.Left + radius, rect.Bottom);
            //左下角 
            roundRect.AddArc(rect.Left - 1, rect.Bottom - radius, radius, radius, 90, 90);
            //左边 
            roundRect.AddLine(rect.Left - 1, rect.Top + radius, rect.Left - 1, rect.Bottom - radius);
            //左上角 
            roundRect.AddArc(rect.Left - 1, rect.Top - 1, radius, radius, 180, 90);
            this.Region = new Region(roundRect);

            base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
    }
}
