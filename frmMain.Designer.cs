namespace AcuLink_Bridge_Reader_CSharp
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.lblWuStationID = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.pbarProgressBar1 = new System.Windows.Forms.ProgressBar();
            this.BackgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.Label11 = new System.Windows.Forms.Label();
            this.txtSignal = new System.Windows.Forms.TextBox();
            this.lblBattery = new System.Windows.Forms.Label();
            this.txtBattery = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtLastUpdated = new System.Windows.Forms.TextBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSignalFails = new System.Windows.Forms.TextBox();
            this.lblWbStationID = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblPwsStationID = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblAweatherStationID = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblOwmId = new System.Windows.Forms.Label();
            this.lblCwopId = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerCWOP = new System.Windows.Forms.Timer(this.components);
            this.timerAW = new System.Windows.Forms.Timer(this.components);
            this.txtErrorCount = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtSensor = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtSensorId = new System.Windows.Forms.TextBox();
            this.timerGetNetworkDevices = new System.Windows.Forms.Timer(this.components);
            this.txtAcuriteTime = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.MenuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(112, 45);
            this.AboutToolStripMenuItem.Text = "About";
            this.AboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click_1);
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            this.ToolStripMenuItem1.Size = new System.Drawing.Size(137, 45);
            this.ToolStripMenuItem1.Text = "Settings";
            this.ToolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem1,
            this.AboutToolStripMenuItem});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Padding = new System.Windows.Forms.Padding(16, 4, 0, 4);
            this.MenuStrip1.Size = new System.Drawing.Size(2686, 53);
            this.MenuStrip1.TabIndex = 78;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // lblWuStationID
            // 
            this.lblWuStationID.AutoSize = true;
            this.lblWuStationID.Location = new System.Drawing.Point(336, 95);
            this.lblWuStationID.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblWuStationID.Name = "lblWuStationID";
            this.lblWuStationID.Size = new System.Drawing.Size(0, 32);
            this.lblWuStationID.TabIndex = 77;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(26, 95);
            this.Label4.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(310, 32);
            this.Label4.TabIndex = 76;
            this.Label4.Text = "Weather Underground: ";
            // 
            // pbarProgressBar1
            // 
            this.pbarProgressBar1.Location = new System.Drawing.Point(1534, 963);
            this.pbarProgressBar1.Margin = new System.Windows.Forms.Padding(8);
            this.pbarProgressBar1.Name = "pbarProgressBar1";
            this.pbarProgressBar1.Size = new System.Drawing.Size(843, 48);
            this.pbarProgressBar1.Step = 1;
            this.pbarProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbarProgressBar1.TabIndex = 75;
            this.pbarProgressBar1.Value = 5;
            this.pbarProgressBar1.Visible = false;
            // 
            // BackgroundWorker1
            // 
            this.BackgroundWorker1.WorkerSupportsCancellation = true;
            this.BackgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork_1);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(2539, 963);
            this.btnStop.Margin = new System.Windows.Forms.Padding(8);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(130, 54);
            this.btnStop.TabIndex = 74;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click_1);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(2393, 963);
            this.btnStart.Margin = new System.Windows.Forms.Padding(8);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(130, 54);
            this.btnStart.TabIndex = 73;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Location = new System.Drawing.Point(991, 1021);
            this.Label11.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(96, 32);
            this.Label11.TabIndex = 72;
            this.Label11.Text = "Signal";
            // 
            // txtSignal
            // 
            this.txtSignal.Location = new System.Drawing.Point(993, 965);
            this.txtSignal.Margin = new System.Windows.Forms.Padding(8);
            this.txtSignal.Name = "txtSignal";
            this.txtSignal.ReadOnly = true;
            this.txtSignal.Size = new System.Drawing.Size(82, 38);
            this.txtSignal.TabIndex = 71;
            // 
            // lblBattery
            // 
            this.lblBattery.AutoSize = true;
            this.lblBattery.Location = new System.Drawing.Point(1095, 1021);
            this.lblBattery.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblBattery.Name = "lblBattery";
            this.lblBattery.Size = new System.Drawing.Size(105, 32);
            this.lblBattery.TabIndex = 70;
            this.lblBattery.Text = "Battery";
            // 
            // txtBattery
            // 
            this.txtBattery.Location = new System.Drawing.Point(1095, 965);
            this.txtBattery.Margin = new System.Windows.Forms.Padding(8);
            this.txtBattery.Name = "txtBattery";
            this.txtBattery.ReadOnly = true;
            this.txtBattery.Size = new System.Drawing.Size(148, 38);
            this.txtBattery.TabIndex = 69;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(15, 1021);
            this.Label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(184, 32);
            this.Label1.TabIndex = 68;
            this.Label1.Text = "Last Updated";
            // 
            // txtLastUpdated
            // 
            this.txtLastUpdated.Location = new System.Drawing.Point(15, 965);
            this.txtLastUpdated.Margin = new System.Windows.Forms.Padding(8);
            this.txtLastUpdated.Name = "txtLastUpdated";
            this.txtLastUpdated.ReadOnly = true;
            this.txtLastUpdated.Size = new System.Drawing.Size(335, 38);
            this.txtLastUpdated.TabIndex = 67;
            // 
            // txtOutput
            // 
            this.txtOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.45F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(0, 163);
            this.txtOutput.Margin = new System.Windows.Forms.Padding(8);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(2682, 758);
            this.txtOutput.TabIndex = 66;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1256, 1021);
            this.label2.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 32);
            this.label2.TabIndex = 80;
            this.label2.Text = "Signal Fails";
            // 
            // txtSignalFails
            // 
            this.txtSignalFails.Location = new System.Drawing.Point(1264, 965);
            this.txtSignalFails.Margin = new System.Windows.Forms.Padding(8);
            this.txtSignalFails.Name = "txtSignalFails";
            this.txtSignalFails.ReadOnly = true;
            this.txtSignalFails.Size = new System.Drawing.Size(132, 38);
            this.txtSignalFails.TabIndex = 79;
            // 
            // lblWbStationID
            // 
            this.lblWbStationID.AutoSize = true;
            this.lblWbStationID.Location = new System.Drawing.Point(786, 95);
            this.lblWbStationID.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblWbStationID.Name = "lblWbStationID";
            this.lblWbStationID.Size = new System.Drawing.Size(0, 32);
            this.lblWbStationID.TabIndex = 82;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(606, 95);
            this.label5.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(181, 32);
            this.label5.TabIndex = 81;
            this.label5.Text = "WeatherBug:";
            // 
            // lblPwsStationID
            // 
            this.lblPwsStationID.AutoSize = true;
            this.lblPwsStationID.Location = new System.Drawing.Point(1022, 95);
            this.lblPwsStationID.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblPwsStationID.Name = "lblPwsStationID";
            this.lblPwsStationID.Size = new System.Drawing.Size(0, 32);
            this.lblPwsStationID.TabIndex = 84;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(936, 95);
            this.label7.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 32);
            this.label7.TabIndex = 83;
            this.label7.Text = "PWS: ";
            // 
            // lblAweatherStationID
            // 
            this.lblAweatherStationID.AutoSize = true;
            this.lblAweatherStationID.Location = new System.Drawing.Point(1538, 95);
            this.lblAweatherStationID.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblAweatherStationID.Name = "lblAweatherStationID";
            this.lblAweatherStationID.Size = new System.Drawing.Size(0, 32);
            this.lblAweatherStationID.TabIndex = 86;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1290, 95);
            this.label6.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(256, 32);
            this.label6.TabIndex = 85;
            this.label6.Text = "Anything Weather: ";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 21600000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1758, 95);
            this.label3.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(255, 32);
            this.label3.TabIndex = 87;
            this.label3.Text = "OpenWeatherMap:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(2360, 95);
            this.label8.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 32);
            this.label8.TabIndex = 88;
            this.label8.Text = "CWOP:";
            // 
            // lblOwmId
            // 
            this.lblOwmId.AutoSize = true;
            this.lblOwmId.Location = new System.Drawing.Point(2012, 95);
            this.lblOwmId.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblOwmId.Name = "lblOwmId";
            this.lblOwmId.Size = new System.Drawing.Size(0, 32);
            this.lblOwmId.TabIndex = 89;
            // 
            // lblCwopId
            // 
            this.lblCwopId.AutoSize = true;
            this.lblCwopId.Location = new System.Drawing.Point(2464, 95);
            this.lblCwopId.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblCwopId.Name = "lblCwopId";
            this.lblCwopId.Size = new System.Drawing.Size(0, 32);
            this.lblCwopId.TabIndex = 90;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "The weather app will continue to run in the background";
            this.notifyIcon1.BalloonTipTitle = "Kevin\'s Acu-Link Bridge Reader";
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Kevin\'s Acu-Link Bridge Reader";
            this.notifyIcon1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(180, 50);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(179, 46);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // timerCWOP
            // 
            this.timerCWOP.Enabled = true;
            this.timerCWOP.Interval = 300000;
            this.timerCWOP.Tick += new System.EventHandler(this.timerCWOP_Tick);
            // 
            // timerAW
            // 
            this.timerAW.Enabled = true;
            this.timerAW.Interval = 300000;
            this.timerAW.Tick += new System.EventHandler(this.timerAW_Tik);
            // 
            // txtErrorCount
            // 
            this.txtErrorCount.Location = new System.Drawing.Point(1416, 965);
            this.txtErrorCount.Margin = new System.Windows.Forms.Padding(8);
            this.txtErrorCount.Name = "txtErrorCount";
            this.txtErrorCount.ReadOnly = true;
            this.txtErrorCount.Size = new System.Drawing.Size(102, 38);
            this.txtErrorCount.TabIndex = 91;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1416, 1021);
            this.label9.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(91, 32);
            this.label9.TabIndex = 92;
            this.label9.Text = "Errors";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(703, 1021);
            this.label10.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(105, 32);
            this.label10.TabIndex = 94;
            this.label10.Text = "Sensor";
            // 
            // txtSensor
            // 
            this.txtSensor.Location = new System.Drawing.Point(705, 965);
            this.txtSensor.Margin = new System.Windows.Forms.Padding(8);
            this.txtSensor.Name = "txtSensor";
            this.txtSensor.ReadOnly = true;
            this.txtSensor.Size = new System.Drawing.Size(118, 38);
            this.txtSensor.TabIndex = 93;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(841, 1021);
            this.label12.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(139, 32);
            this.label12.TabIndex = 96;
            this.label12.Text = "Sensor ID";
            // 
            // txtSensorId
            // 
            this.txtSensorId.Location = new System.Drawing.Point(843, 965);
            this.txtSensorId.Margin = new System.Windows.Forms.Padding(8);
            this.txtSensorId.Name = "txtSensorId";
            this.txtSensorId.ReadOnly = true;
            this.txtSensorId.Size = new System.Drawing.Size(130, 38);
            this.txtSensorId.TabIndex = 95;
            // 
            // timerGetNetworkDevices
            // 
            this.timerGetNetworkDevices.Enabled = true;
            this.timerGetNetworkDevices.Interval = 15000;
            this.timerGetNetworkDevices.Tick += new System.EventHandler(this.timerGetNetworkDevices_Tick);
            // 
            // txtAcuriteTime
            // 
            this.txtAcuriteTime.Location = new System.Drawing.Point(366, 965);
            this.txtAcuriteTime.Margin = new System.Windows.Forms.Padding(8);
            this.txtAcuriteTime.Name = "txtAcuriteTime";
            this.txtAcuriteTime.ReadOnly = true;
            this.txtAcuriteTime.Size = new System.Drawing.Size(323, 38);
            this.txtAcuriteTime.TabIndex = 97;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(380, 1021);
            this.label13.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(185, 32);
            this.label13.TabIndex = 98;
            this.label13.Text = "AcuRite Time";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(2686, 1081);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtAcuriteTime);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtSensorId);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtSensor);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtErrorCount);
            this.Controls.Add(this.lblCwopId);
            this.Controls.Add(this.lblOwmId);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblAweatherStationID);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblPwsStationID);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblWbStationID);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSignalFails);
            this.Controls.Add(this.MenuStrip1);
            this.Controls.Add(this.lblWuStationID);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.pbarProgressBar1);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.Label11);
            this.Controls.Add(this.txtSignal);
            this.Controls.Add(this.lblBattery);
            this.Controls.Add(this.txtBattery);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.txtLastUpdated);
            this.Controls.Add(this.txtOutput);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(8);
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(2628, 1076);
            this.Name = "frmMain";
            this.Text = "Kevin\'s My AcuRite smartHUB Reader";
            this.Load += new System.EventHandler(this.frmMain_Load_1);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem1;
        internal System.Windows.Forms.MenuStrip MenuStrip1;
        internal System.Windows.Forms.Label lblWuStationID;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.ProgressBar pbarProgressBar1;
        internal System.ComponentModel.BackgroundWorker BackgroundWorker1;
        internal System.Windows.Forms.Button btnStop;
        internal System.Windows.Forms.Button btnStart;
        internal System.Windows.Forms.Label Label11;
        internal System.Windows.Forms.TextBox txtSignal;
        internal System.Windows.Forms.Label lblBattery;
        internal System.Windows.Forms.TextBox txtBattery;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox txtLastUpdated;
        internal System.Windows.Forms.TextBox txtOutput;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.TextBox txtSignalFails;
        internal System.Windows.Forms.Label lblWbStationID;
        internal System.Windows.Forms.Label label5;
        internal System.Windows.Forms.Label lblPwsStationID;
        internal System.Windows.Forms.Label label7;
        internal System.Windows.Forms.Label lblAweatherStationID;
        internal System.Windows.Forms.Label label6;
        private System.Windows.Forms.Timer timer1;
        internal System.Windows.Forms.Label label3;
        internal System.Windows.Forms.Label label8;
        internal System.Windows.Forms.Label lblOwmId;
        internal System.Windows.Forms.Label lblCwopId;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Timer timerCWOP;
        private System.Windows.Forms.Timer timerAW;
        internal System.Windows.Forms.TextBox txtErrorCount;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.Label label10;
        internal System.Windows.Forms.TextBox txtSensor;
        internal System.Windows.Forms.Label label12;
        internal System.Windows.Forms.TextBox txtSensorId;
        private System.Windows.Forms.Timer timerGetNetworkDevices;
        internal System.Windows.Forms.TextBox txtAcuriteTime;
        internal System.Windows.Forms.Label label13;
    }
}