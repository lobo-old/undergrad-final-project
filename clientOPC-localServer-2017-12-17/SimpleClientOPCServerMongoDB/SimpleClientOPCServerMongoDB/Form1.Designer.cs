namespace SimpleClientOPCServerMongoDB
{
    partial class FormConnection
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
            this.btnStartThreadPoliing = new System.Windows.Forms.Button();
            this.btnStartThreadResponses = new System.Windows.Forms.Button();
            this.btnStartThreadOPC = new System.Windows.Forms.Button();
            this.btnStartThreadPeriodic = new System.Windows.Forms.Button();
            this.btnStopThreadPeriodic = new System.Windows.Forms.Button();
            this.btnStopThreadOPC = new System.Windows.Forms.Button();
            this.btnStopThreadResponses = new System.Windows.Forms.Button();
            this.btnStopThreadPoliing = new System.Windows.Forms.Button();
            this.btnCreateConn = new System.Windows.Forms.Button();
            this.btnDisconnectServer = new System.Windows.Forms.Button();
            this.listBoxStaatusLog = new System.Windows.Forms.ListBox();
            this.treeViewDevices = new System.Windows.Forms.TreeView();
            this.cbSendMongo = new System.Windows.Forms.ComboBox();
            this.btnAddToMongo = new System.Windows.Forms.Button();
            this.btnRemoveFromMongo = new System.Windows.Forms.Button();
            this.btnSetAvailableD = new System.Windows.Forms.Button();
            this.btnEditDevices = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbSampleTlb = new System.Windows.Forms.Label();
            this.numericUpDownSampleT = new System.Windows.Forms.NumericUpDown();
            this.lbStatusLog = new System.Windows.Forms.Label();
            this.lbDevTree = new System.Windows.Forms.Label();
            this.lbDevList = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBoxRequestsLog = new System.Windows.Forms.ListBox();
            this.lbRequestsLog = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.NqueueReqLength = new System.Windows.Forms.Label();
            this.lbRequestQueueN = new System.Windows.Forms.Label();
            this.NwriteReq = new System.Windows.Forms.Label();
            this.lbNwriteReq = new System.Windows.Forms.Label();
            this.NqueueRespoLength = new System.Windows.Forms.Label();
            this.lbResponseQueueN = new System.Windows.Forms.Label();
            this.NreadReq = new System.Windows.Forms.Label();
            this.lbNreadReq = new System.Windows.Forms.Label();
            this.checkBoxStatusLog = new System.Windows.Forms.CheckBox();
            this.checkBoxRequestsLog = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnBrowseServers = new System.Windows.Forms.Button();
            this.cbBrowseServers = new System.Windows.Forms.ComboBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSampleT)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartThreadPoliing
            // 
            this.btnStartThreadPoliing.Enabled = false;
            this.btnStartThreadPoliing.Location = new System.Drawing.Point(16, 78);
            this.btnStartThreadPoliing.Name = "btnStartThreadPoliing";
            this.btnStartThreadPoliing.Size = new System.Drawing.Size(133, 23);
            this.btnStartThreadPoliing.TabIndex = 0;
            this.btnStartThreadPoliing.Text = "Start Thread Poliing";
            this.btnStartThreadPoliing.UseVisualStyleBackColor = true;
            this.btnStartThreadPoliing.Click += new System.EventHandler(this.btnStartThreadPoliing_Click);
            // 
            // btnStartThreadResponses
            // 
            this.btnStartThreadResponses.Enabled = false;
            this.btnStartThreadResponses.Location = new System.Drawing.Point(16, 20);
            this.btnStartThreadResponses.Name = "btnStartThreadResponses";
            this.btnStartThreadResponses.Size = new System.Drawing.Size(133, 23);
            this.btnStartThreadResponses.TabIndex = 1;
            this.btnStartThreadResponses.Text = "Start Thread Responses";
            this.btnStartThreadResponses.UseVisualStyleBackColor = true;
            this.btnStartThreadResponses.Click += new System.EventHandler(this.btnStartThreadResponses_Click);
            // 
            // btnStartThreadOPC
            // 
            this.btnStartThreadOPC.Enabled = false;
            this.btnStartThreadOPC.Location = new System.Drawing.Point(16, 49);
            this.btnStartThreadOPC.Name = "btnStartThreadOPC";
            this.btnStartThreadOPC.Size = new System.Drawing.Size(133, 23);
            this.btnStartThreadOPC.TabIndex = 2;
            this.btnStartThreadOPC.Text = "Start Thread OPC";
            this.btnStartThreadOPC.UseVisualStyleBackColor = true;
            this.btnStartThreadOPC.Click += new System.EventHandler(this.btnStartThreadOPC_Click);
            // 
            // btnStartThreadPeriodic
            // 
            this.btnStartThreadPeriodic.Enabled = false;
            this.btnStartThreadPeriodic.Location = new System.Drawing.Point(16, 133);
            this.btnStartThreadPeriodic.Name = "btnStartThreadPeriodic";
            this.btnStartThreadPeriodic.Size = new System.Drawing.Size(134, 23);
            this.btnStartThreadPeriodic.TabIndex = 3;
            this.btnStartThreadPeriodic.Text = "Start Thread Periodic";
            this.btnStartThreadPeriodic.UseVisualStyleBackColor = true;
            this.btnStartThreadPeriodic.Click += new System.EventHandler(this.btnStartThreadPeriodic_Click);
            // 
            // btnStopThreadPeriodic
            // 
            this.btnStopThreadPeriodic.Enabled = false;
            this.btnStopThreadPeriodic.Location = new System.Drawing.Point(250, 133);
            this.btnStopThreadPeriodic.Name = "btnStopThreadPeriodic";
            this.btnStopThreadPeriodic.Size = new System.Drawing.Size(133, 23);
            this.btnStopThreadPeriodic.TabIndex = 7;
            this.btnStopThreadPeriodic.Text = "Stop Thread Periodic";
            this.btnStopThreadPeriodic.UseVisualStyleBackColor = true;
            this.btnStopThreadPeriodic.Click += new System.EventHandler(this.btnStopThreadPeriodic_Click);
            // 
            // btnStopThreadOPC
            // 
            this.btnStopThreadOPC.Enabled = false;
            this.btnStopThreadOPC.Location = new System.Drawing.Point(250, 49);
            this.btnStopThreadOPC.Name = "btnStopThreadOPC";
            this.btnStopThreadOPC.Size = new System.Drawing.Size(133, 23);
            this.btnStopThreadOPC.TabIndex = 6;
            this.btnStopThreadOPC.Text = "Stop Thread OPC";
            this.btnStopThreadOPC.UseVisualStyleBackColor = true;
            this.btnStopThreadOPC.Click += new System.EventHandler(this.btnStopThreadOPC_Click);
            // 
            // btnStopThreadResponses
            // 
            this.btnStopThreadResponses.Enabled = false;
            this.btnStopThreadResponses.Location = new System.Drawing.Point(250, 20);
            this.btnStopThreadResponses.Name = "btnStopThreadResponses";
            this.btnStopThreadResponses.Size = new System.Drawing.Size(133, 23);
            this.btnStopThreadResponses.TabIndex = 5;
            this.btnStopThreadResponses.Text = "Stop Thread Responses";
            this.btnStopThreadResponses.UseVisualStyleBackColor = true;
            this.btnStopThreadResponses.Click += new System.EventHandler(this.btnStopThreadResponses_Click);
            // 
            // btnStopThreadPoliing
            // 
            this.btnStopThreadPoliing.Enabled = false;
            this.btnStopThreadPoliing.Location = new System.Drawing.Point(250, 78);
            this.btnStopThreadPoliing.Name = "btnStopThreadPoliing";
            this.btnStopThreadPoliing.Size = new System.Drawing.Size(133, 23);
            this.btnStopThreadPoliing.TabIndex = 4;
            this.btnStopThreadPoliing.Text = "Stop Thread Poliing";
            this.btnStopThreadPoliing.UseVisualStyleBackColor = true;
            this.btnStopThreadPoliing.Click += new System.EventHandler(this.btnStopThreadPoliing_Click);
            // 
            // btnCreateConn
            // 
            this.btnCreateConn.Location = new System.Drawing.Point(16, 48);
            this.btnCreateConn.Name = "btnCreateConn";
            this.btnCreateConn.Size = new System.Drawing.Size(150, 38);
            this.btnCreateConn.TabIndex = 10;
            this.btnCreateConn.Text = "Connect";
            this.btnCreateConn.UseVisualStyleBackColor = true;
            this.btnCreateConn.Click += new System.EventHandler(this.btnCreateConn_Click);
            // 
            // btnDisconnectServer
            // 
            this.btnDisconnectServer.Location = new System.Drawing.Point(172, 46);
            this.btnDisconnectServer.Name = "btnDisconnectServer";
            this.btnDisconnectServer.Size = new System.Drawing.Size(211, 38);
            this.btnDisconnectServer.TabIndex = 11;
            this.btnDisconnectServer.Text = "Disconnect and Close";
            this.btnDisconnectServer.UseVisualStyleBackColor = true;
            this.btnDisconnectServer.Click += new System.EventHandler(this.btnDisconnectServer_Click);
            // 
            // listBoxStaatusLog
            // 
            this.listBoxStaatusLog.FormattingEnabled = true;
            this.listBoxStaatusLog.HorizontalScrollbar = true;
            this.listBoxStaatusLog.Location = new System.Drawing.Point(25, 38);
            this.listBoxStaatusLog.Name = "listBoxStaatusLog";
            this.listBoxStaatusLog.Size = new System.Drawing.Size(488, 290);
            this.listBoxStaatusLog.TabIndex = 12;
            // 
            // treeViewDevices
            // 
            this.treeViewDevices.Location = new System.Drawing.Point(25, 41);
            this.treeViewDevices.Name = "treeViewDevices";
            this.treeViewDevices.Size = new System.Drawing.Size(177, 241);
            this.treeViewDevices.TabIndex = 13;
            this.treeViewDevices.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDevices_AfterSelect);
            // 
            // cbSendMongo
            // 
            this.cbSendMongo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cbSendMongo.FormattingEnabled = true;
            this.cbSendMongo.Location = new System.Drawing.Point(352, 41);
            this.cbSendMongo.Name = "cbSendMongo";
            this.cbSendMongo.Size = new System.Drawing.Size(155, 241);
            this.cbSendMongo.TabIndex = 14;
            // 
            // btnAddToMongo
            // 
            this.btnAddToMongo.Enabled = false;
            this.btnAddToMongo.Location = new System.Drawing.Point(208, 74);
            this.btnAddToMongo.Name = "btnAddToMongo";
            this.btnAddToMongo.Size = new System.Drawing.Size(138, 23);
            this.btnAddToMongo.TabIndex = 15;
            this.btnAddToMongo.Text = "Add to Mongo Send >>";
            this.btnAddToMongo.UseVisualStyleBackColor = true;
            this.btnAddToMongo.Click += new System.EventHandler(this.btnAddToMongo_Click);
            // 
            // btnRemoveFromMongo
            // 
            this.btnRemoveFromMongo.Enabled = false;
            this.btnRemoveFromMongo.Location = new System.Drawing.Point(208, 108);
            this.btnRemoveFromMongo.Name = "btnRemoveFromMongo";
            this.btnRemoveFromMongo.Size = new System.Drawing.Size(138, 23);
            this.btnRemoveFromMongo.TabIndex = 16;
            this.btnRemoveFromMongo.Text = "Remove From List";
            this.btnRemoveFromMongo.UseVisualStyleBackColor = true;
            this.btnRemoveFromMongo.Click += new System.EventHandler(this.btnRemoveFromMongo_Click);
            // 
            // btnSetAvailableD
            // 
            this.btnSetAvailableD.Enabled = false;
            this.btnSetAvailableD.Location = new System.Drawing.Point(208, 160);
            this.btnSetAvailableD.Name = "btnSetAvailableD";
            this.btnSetAvailableD.Size = new System.Drawing.Size(138, 23);
            this.btnSetAvailableD.TabIndex = 17;
            this.btnSetAvailableD.Text = "Set Available Devices";
            this.btnSetAvailableD.UseVisualStyleBackColor = true;
            this.btnSetAvailableD.Click += new System.EventHandler(this.btnSetAvailableD_Click);
            // 
            // btnEditDevices
            // 
            this.btnEditDevices.Enabled = false;
            this.btnEditDevices.Location = new System.Drawing.Point(208, 189);
            this.btnEditDevices.Name = "btnEditDevices";
            this.btnEditDevices.Size = new System.Drawing.Size(138, 23);
            this.btnEditDevices.TabIndex = 18;
            this.btnEditDevices.Text = "Edit Available Devices";
            this.btnEditDevices.UseVisualStyleBackColor = true;
            this.btnEditDevices.Click += new System.EventHandler(this.btnEditDevices_Click_1);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.lbSampleTlb);
            this.groupBox1.Controls.Add(this.numericUpDownSampleT);
            this.groupBox1.Controls.Add(this.btnStopThreadOPC);
            this.groupBox1.Controls.Add(this.btnStartThreadPoliing);
            this.groupBox1.Controls.Add(this.btnStopThreadPeriodic);
            this.groupBox1.Controls.Add(this.btnStartThreadPeriodic);
            this.groupBox1.Controls.Add(this.btnStartThreadResponses);
            this.groupBox1.Controls.Add(this.btnStartThreadOPC);
            this.groupBox1.Controls.Add(this.btnStopThreadPoliing);
            this.groupBox1.Controls.Add(this.btnStopThreadResponses);
            this.groupBox1.Location = new System.Drawing.Point(553, 109);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(404, 215);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Threads Manager";
            // 
            // lbSampleTlb
            // 
            this.lbSampleTlb.AutoSize = true;
            this.lbSampleTlb.Location = new System.Drawing.Point(157, 172);
            this.lbSampleTlb.Name = "lbSampleTlb";
            this.lbSampleTlb.Size = new System.Drawing.Size(95, 13);
            this.lbSampleTlb.TabIndex = 9;
            this.lbSampleTlb.Text = "Sampling Time(ms)";
            // 
            // numericUpDownSampleT
            // 
            this.numericUpDownSampleT.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownSampleT.Location = new System.Drawing.Point(172, 189);
            this.numericUpDownSampleT.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDownSampleT.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownSampleT.Name = "numericUpDownSampleT";
            this.numericUpDownSampleT.Size = new System.Drawing.Size(62, 20);
            this.numericUpDownSampleT.TabIndex = 10;
            this.numericUpDownSampleT.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownSampleT.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // lbStatusLog
            // 
            this.lbStatusLog.AutoSize = true;
            this.lbStatusLog.Location = new System.Drawing.Point(22, 21);
            this.lbStatusLog.Name = "lbStatusLog";
            this.lbStatusLog.Size = new System.Drawing.Size(58, 13);
            this.lbStatusLog.TabIndex = 20;
            this.lbStatusLog.Text = "Status Log";
            // 
            // lbDevTree
            // 
            this.lbDevTree.AutoSize = true;
            this.lbDevTree.Location = new System.Drawing.Point(60, 21);
            this.lbDevTree.Name = "lbDevTree";
            this.lbDevTree.Size = new System.Drawing.Size(94, 13);
            this.lbDevTree.TabIndex = 21;
            this.lbDevTree.Text = "Devices TreeView";
            // 
            // lbDevList
            // 
            this.lbDevList.AutoSize = true;
            this.lbDevList.Location = new System.Drawing.Point(398, 21);
            this.lbDevList.Name = "lbDevList";
            this.lbDevList.Size = new System.Drawing.Size(65, 13);
            this.lbDevList.TabIndex = 22;
            this.lbDevList.Text = "Devices List";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.treeViewDevices);
            this.groupBox2.Controls.Add(this.lbDevList);
            this.groupBox2.Controls.Add(this.cbSendMongo);
            this.groupBox2.Controls.Add(this.lbDevTree);
            this.groupBox2.Controls.Add(this.btnAddToMongo);
            this.groupBox2.Controls.Add(this.btnRemoveFromMongo);
            this.groupBox2.Controls.Add(this.btnSetAvailableD);
            this.groupBox2.Controls.Add(this.btnEditDevices);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(513, 294);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Devices Manager";
            // 
            // listBoxRequestsLog
            // 
            this.listBoxRequestsLog.FormattingEnabled = true;
            this.listBoxRequestsLog.HorizontalScrollbar = true;
            this.listBoxRequestsLog.Location = new System.Drawing.Point(541, 38);
            this.listBoxRequestsLog.Name = "listBoxRequestsLog";
            this.listBoxRequestsLog.Size = new System.Drawing.Size(386, 199);
            this.listBoxRequestsLog.TabIndex = 24;
            // 
            // lbRequestsLog
            // 
            this.lbRequestsLog.AutoSize = true;
            this.lbRequestsLog.Location = new System.Drawing.Point(540, 21);
            this.lbRequestsLog.Name = "lbRequestsLog";
            this.lbRequestsLog.Size = new System.Drawing.Size(73, 13);
            this.lbRequestsLog.TabIndex = 25;
            this.lbRequestsLog.Text = "Requests Log";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.NqueueReqLength);
            this.groupBox3.Controls.Add(this.lbRequestQueueN);
            this.groupBox3.Controls.Add(this.NwriteReq);
            this.groupBox3.Controls.Add(this.lbNwriteReq);
            this.groupBox3.Controls.Add(this.NqueueRespoLength);
            this.groupBox3.Controls.Add(this.lbResponseQueueN);
            this.groupBox3.Controls.Add(this.NreadReq);
            this.groupBox3.Controls.Add(this.lbNreadReq);
            this.groupBox3.Location = new System.Drawing.Point(541, 243);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(386, 85);
            this.groupBox3.TabIndex = 26;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Requests Counters and Queues Length";
            // 
            // NqueueReqLength
            // 
            this.NqueueReqLength.AutoSize = true;
            this.NqueueReqLength.Location = new System.Drawing.Point(298, 57);
            this.NqueueReqLength.Name = "NqueueReqLength";
            this.NqueueReqLength.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.NqueueReqLength.Size = new System.Drawing.Size(13, 13);
            this.NqueueReqLength.TabIndex = 7;
            this.NqueueReqLength.Text = "0";
            // 
            // lbRequestQueueN
            // 
            this.lbRequestQueueN.AutoSize = true;
            this.lbRequestQueueN.Location = new System.Drawing.Point(167, 57);
            this.lbRequestQueueN.Name = "lbRequestQueueN";
            this.lbRequestQueueN.Size = new System.Drawing.Size(117, 13);
            this.lbRequestQueueN.TabIndex = 6;
            this.lbRequestQueueN.Text = "Request Queue length:";
            // 
            // NwriteReq
            // 
            this.NwriteReq.AutoSize = true;
            this.NwriteReq.Location = new System.Drawing.Point(109, 57);
            this.NwriteReq.Name = "NwriteReq";
            this.NwriteReq.Size = new System.Drawing.Size(13, 13);
            this.NwriteReq.TabIndex = 5;
            this.NwriteReq.Text = "0";
            // 
            // lbNwriteReq
            // 
            this.lbNwriteReq.AutoSize = true;
            this.lbNwriteReq.Location = new System.Drawing.Point(8, 57);
            this.lbNwriteReq.Name = "lbNwriteReq";
            this.lbNwriteReq.Size = new System.Drawing.Size(83, 13);
            this.lbNwriteReq.TabIndex = 4;
            this.lbNwriteReq.Text = "Write Requests:";
            // 
            // NqueueRespoLength
            // 
            this.NqueueRespoLength.AutoSize = true;
            this.NqueueRespoLength.Location = new System.Drawing.Point(298, 30);
            this.NqueueRespoLength.Name = "NqueueRespoLength";
            this.NqueueRespoLength.Size = new System.Drawing.Size(13, 13);
            this.NqueueRespoLength.TabIndex = 3;
            this.NqueueRespoLength.Text = "0";
            // 
            // lbResponseQueueN
            // 
            this.lbResponseQueueN.AutoSize = true;
            this.lbResponseQueueN.Location = new System.Drawing.Point(167, 30);
            this.lbResponseQueueN.Name = "lbResponseQueueN";
            this.lbResponseQueueN.Size = new System.Drawing.Size(125, 13);
            this.lbResponseQueueN.TabIndex = 2;
            this.lbResponseQueueN.Text = "Response Queue length:";
            // 
            // NreadReq
            // 
            this.NreadReq.AutoSize = true;
            this.NreadReq.Location = new System.Drawing.Point(109, 30);
            this.NreadReq.Name = "NreadReq";
            this.NreadReq.Size = new System.Drawing.Size(13, 13);
            this.NreadReq.TabIndex = 1;
            this.NreadReq.Text = "0";
            // 
            // lbNreadReq
            // 
            this.lbNreadReq.AutoSize = true;
            this.lbNreadReq.Location = new System.Drawing.Point(8, 30);
            this.lbNreadReq.Name = "lbNreadReq";
            this.lbNreadReq.Size = new System.Drawing.Size(84, 13);
            this.lbNreadReq.TabIndex = 0;
            this.lbNreadReq.Text = "Read Requests:";
            // 
            // checkBoxStatusLog
            // 
            this.checkBoxStatusLog.AutoSize = true;
            this.checkBoxStatusLog.Checked = true;
            this.checkBoxStatusLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStatusLog.Location = new System.Drawing.Point(86, 20);
            this.checkBoxStatusLog.Name = "checkBoxStatusLog";
            this.checkBoxStatusLog.Size = new System.Drawing.Size(77, 17);
            this.checkBoxStatusLog.TabIndex = 27;
            this.checkBoxStatusLog.Text = "Auto Scroll";
            this.checkBoxStatusLog.UseVisualStyleBackColor = true;
            // 
            // checkBoxRequestsLog
            // 
            this.checkBoxRequestsLog.AutoSize = true;
            this.checkBoxRequestsLog.Checked = true;
            this.checkBoxRequestsLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRequestsLog.Location = new System.Drawing.Point(620, 20);
            this.checkBoxRequestsLog.Name = "checkBoxRequestsLog";
            this.checkBoxRequestsLog.Size = new System.Drawing.Size(77, 17);
            this.checkBoxRequestsLog.TabIndex = 28;
            this.checkBoxRequestsLog.Text = "Auto Scroll";
            this.checkBoxRequestsLog.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnBrowseServers);
            this.groupBox5.Controls.Add(this.cbBrowseServers);
            this.groupBox5.Controls.Add(this.btnDisconnectServer);
            this.groupBox5.Controls.Add(this.btnCreateConn);
            this.groupBox5.Location = new System.Drawing.Point(553, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(404, 91);
            this.groupBox5.TabIndex = 30;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Server Connection";
            // 
            // btnBrowseServers
            // 
            this.btnBrowseServers.Location = new System.Drawing.Point(16, 19);
            this.btnBrowseServers.Name = "btnBrowseServers";
            this.btnBrowseServers.Size = new System.Drawing.Size(150, 23);
            this.btnBrowseServers.TabIndex = 13;
            this.btnBrowseServers.Text = "Browse Servers >>";
            this.btnBrowseServers.UseVisualStyleBackColor = true;
            this.btnBrowseServers.Click += new System.EventHandler(this.btnBrowseServers_Click);
            // 
            // cbBrowseServers
            // 
            this.cbBrowseServers.FormattingEnabled = true;
            this.cbBrowseServers.Location = new System.Drawing.Point(172, 19);
            this.cbBrowseServers.Name = "cbBrowseServers";
            this.cbBrowseServers.Size = new System.Drawing.Size(214, 21);
            this.cbBrowseServers.TabIndex = 12;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.listBoxStaatusLog);
            this.groupBox6.Controls.Add(this.lbStatusLog);
            this.groupBox6.Controls.Add(this.groupBox3);
            this.groupBox6.Controls.Add(this.checkBoxStatusLog);
            this.groupBox6.Controls.Add(this.checkBoxRequestsLog);
            this.groupBox6.Controls.Add(this.listBoxRequestsLog);
            this.groupBox6.Controls.Add(this.lbRequestsLog);
            this.groupBox6.Location = new System.Drawing.Point(22, 330);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(935, 348);
            this.groupBox6.TabIndex = 31;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Logs";
            // 
            // FormConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 702);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "FormConnection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server OPC and device manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSampleT)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        

        #endregion

        private System.Windows.Forms.Button btnStartThreadPoliing;
        private System.Windows.Forms.Button btnStartThreadResponses;
        private System.Windows.Forms.Button btnStartThreadOPC;
        private System.Windows.Forms.Button btnStartThreadPeriodic;
        private System.Windows.Forms.Button btnStopThreadPeriodic;
        private System.Windows.Forms.Button btnStopThreadOPC;
        private System.Windows.Forms.Button btnStopThreadResponses;
        private System.Windows.Forms.Button btnStopThreadPoliing;
        private System.Windows.Forms.Button btnCreateConn;
        private System.Windows.Forms.Button btnDisconnectServer;
        private System.Windows.Forms.ListBox listBoxStaatusLog;
        private System.Windows.Forms.TreeView treeViewDevices;
        private System.Windows.Forms.ComboBox cbSendMongo;
        private System.Windows.Forms.Button btnAddToMongo;
        private System.Windows.Forms.Button btnRemoveFromMongo;
        private System.Windows.Forms.Button btnSetAvailableD;
        private System.Windows.Forms.Button btnEditDevices;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbStatusLog;
        private System.Windows.Forms.Label lbDevTree;
        private System.Windows.Forms.Label lbDevList;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBoxRequestsLog;
        private System.Windows.Forms.Label lbRequestsLog;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label NqueueReqLength;
        private System.Windows.Forms.Label lbRequestQueueN;
        private System.Windows.Forms.Label NwriteReq;
        private System.Windows.Forms.Label lbNwriteReq;
        private System.Windows.Forms.Label NqueueRespoLength;
        private System.Windows.Forms.Label lbResponseQueueN;
        private System.Windows.Forms.Label NreadReq;
        private System.Windows.Forms.Label lbNreadReq;
        private System.Windows.Forms.CheckBox checkBoxStatusLog;
        private System.Windows.Forms.CheckBox checkBoxRequestsLog;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox cbBrowseServers;
        private System.Windows.Forms.Button btnBrowseServers;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label lbSampleTlb;
        private System.Windows.Forms.NumericUpDown numericUpDownSampleT;
    }
}

