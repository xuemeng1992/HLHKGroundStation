namespace AttitudeInstrument.BasicFlightInfo
{
    partial class BasicFlightInfo
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pitchAndBank = new AttitudeInstrument.PitchAndBank();
            this.airSpeedIndicator = new AttitudeInstrument.AirSpeedIndicator();
            this.altitudeMeter = new AttitudeInstrument.AltitudeMeter();
            this.SuspendLayout();
            // 
            // pitchAndBank
            // 
            this.pitchAndBank.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pitchAndBank.Bank = 0D;
            this.pitchAndBank.Location = new System.Drawing.Point(84, 21);
            this.pitchAndBank.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.pitchAndBank.Name = "pitchAndBank";
            this.pitchAndBank.Pitch = 0D;
            this.pitchAndBank.Size = new System.Drawing.Size(246, 253);
            this.pitchAndBank.TabIndex = 1;
            // 
            // airSpeedIndicator
            // 
            this.airSpeedIndicator.AirSpeed = 0D;
            this.airSpeedIndicator.Dock = System.Windows.Forms.DockStyle.Left;
            this.airSpeedIndicator.Location = new System.Drawing.Point(0, 0);
            this.airSpeedIndicator.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.airSpeedIndicator.Name = "airSpeedIndicator";
            this.airSpeedIndicator.Size = new System.Drawing.Size(72, 295);
            this.airSpeedIndicator.TabIndex = 0;
            // 
            // altitudeMeter
            // 
            this.altitudeMeter.Altitude = 0D;
            this.altitudeMeter.Dock = System.Windows.Forms.DockStyle.Right;
            this.altitudeMeter.Location = new System.Drawing.Point(342, 0);
            this.altitudeMeter.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.altitudeMeter.Name = "altitudeMeter";
            this.altitudeMeter.Size = new System.Drawing.Size(84, 295);
            this.altitudeMeter.TabIndex = 2;
            // 
            // BasicFlightInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(49)))));
            this.Controls.Add(this.pitchAndBank);
            this.Controls.Add(this.airSpeedIndicator);
            this.Controls.Add(this.altitudeMeter);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "BasicFlightInfo";
            this.Size = new System.Drawing.Size(426, 295);
            this.ResumeLayout(false);

        }

        #endregion

        private AirSpeedIndicator airSpeedIndicator;
        private PitchAndBank pitchAndBank;
        private AltitudeMeter altitudeMeter;
    }
}
