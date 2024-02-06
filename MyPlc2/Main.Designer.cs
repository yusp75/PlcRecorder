namespace MyPlc2
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            menuStrip1 = new MenuStrip();
            menu_manage = new ToolStripMenuItem();
            menu_io = new ToolStripMenuItem();
            menu_curve = new ToolStripMenuItem();
            menu_curve_history = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            action_start = new ToolStripButton();
            acttion_stop = new ToolStripButton();
            btnIO = new ToolStripButton();
            action_history = new ToolStripButton();
            action_exit = new ToolStripButton();
            split_0 = new SplitContainer();
            split1 = new SplitContainer();
            table1 = new TableLayoutPanel();
            status1 = new StatusStrip();
            logView = new DataGridView();
            id = new DataGridViewTextBoxColumn();
            message = new DataGridViewTextBoxColumn();
            type = new DataGridViewTextBoxColumn();
            time = new DataGridViewTextBoxColumn();
            InfluxDbToken = new ToolStripButton();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)split_0).BeginInit();
            split_0.Panel1.SuspendLayout();
            split_0.Panel2.SuspendLayout();
            split_0.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)split1).BeginInit();
            split1.Panel2.SuspendLayout();
            split1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logView).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { menu_manage, menu_curve });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 25);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // menu_manage
            // 
            menu_manage.DropDownItems.AddRange(new ToolStripItem[] { menu_io });
            menu_manage.Name = "menu_manage";
            menu_manage.Size = new Size(44, 21);
            menu_manage.Text = "管理";
            // 
            // menu_io
            // 
            menu_io.Name = "menu_io";
            menu_io.Size = new Size(114, 22);
            menu_io.Text = "IO变量";
            menu_io.Click += ActionIoClick;
            // 
            // menu_curve
            // 
            menu_curve.DropDownItems.AddRange(new ToolStripItem[] { menu_curve_history });
            menu_curve.Name = "menu_curve";
            menu_curve.Size = new Size(44, 21);
            menu_curve.Text = "曲线";
            // 
            // menu_curve_history
            // 
            menu_curve_history.Name = "menu_curve_history";
            menu_curve_history.Size = new Size(124, 22);
            menu_curve_history.Text = "历史曲线";
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { action_start, acttion_stop, btnIO, action_history, InfluxDbToken, action_exit });
            toolStrip1.Location = new Point(0, 25);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // action_start
            // 
            action_start.BackColor = Color.Transparent;
            action_start.DisplayStyle = ToolStripItemDisplayStyle.Image;
            action_start.Image = (Image)resources.GetObject("action_start.Image");
            action_start.ImageTransparentColor = Color.Magenta;
            action_start.Name = "action_start";
            action_start.Size = new Size(23, 22);
            action_start.Text = "开始";
            action_start.Click += ActionStartClick;
            // 
            // acttion_stop
            // 
            acttion_stop.BackColor = Color.Transparent;
            acttion_stop.DisplayStyle = ToolStripItemDisplayStyle.Image;
            acttion_stop.Image = (Image)resources.GetObject("acttion_stop.Image");
            acttion_stop.ImageTransparentColor = Color.Magenta;
            acttion_stop.Name = "acttion_stop";
            acttion_stop.Size = new Size(23, 22);
            acttion_stop.Text = "停止";
            acttion_stop.Click += ActionStopClick;
            // 
            // btnIO
            // 
            btnIO.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btnIO.Image = (Image)resources.GetObject("btnIO.Image");
            btnIO.ImageTransparentColor = Color.Magenta;
            btnIO.Name = "btnIO";
            btnIO.Size = new Size(23, 22);
            btnIO.Text = "变量设置";
            btnIO.Click += BtnIO_Click;
            // 
            // action_history
            // 
            action_history.DisplayStyle = ToolStripItemDisplayStyle.Image;
            action_history.Image = (Image)resources.GetObject("action_history.Image");
            action_history.ImageTransparentColor = Color.Magenta;
            action_history.Name = "action_history";
            action_history.Size = new Size(23, 22);
            action_history.Text = "历史曲线";
            action_history.Click += ActionHistoryClick;
            // 
            // action_exit
            // 
            action_exit.DisplayStyle = ToolStripItemDisplayStyle.Image;
            action_exit.Image = (Image)resources.GetObject("action_exit.Image");
            action_exit.ImageTransparentColor = Color.Magenta;
            action_exit.Name = "action_exit";
            action_exit.Size = new Size(23, 22);
            action_exit.Text = "退出";
            action_exit.Click += ActionExitClick;
            // 
            // split_0
            // 
            split_0.Dock = DockStyle.Fill;
            split_0.Location = new Point(0, 50);
            split_0.Name = "split_0";
            split_0.Orientation = Orientation.Horizontal;
            // 
            // split_0.Panel1
            // 
            split_0.Panel1.Controls.Add(split1);
            // 
            // split_0.Panel2
            // 
            split_0.Panel2.Controls.Add(status1);
            split_0.Panel2.Controls.Add(logView);
            split_0.Size = new Size(800, 448);
            split_0.SplitterDistance = 346;
            split_0.TabIndex = 2;
            // 
            // split1
            // 
            split1.Dock = DockStyle.Fill;
            split1.Location = new Point(0, 0);
            split1.Name = "split1";
            // 
            // split1.Panel1
            // 
            split1.Panel1.AutoScroll = true;
            split1.Panel1.Margin = new Padding(3);
            // 
            // split1.Panel2
            // 
            split1.Panel2.AutoScroll = true;
            split1.Panel2.Controls.Add(table1);
            split1.Size = new Size(800, 346);
            split1.SplitterDistance = 144;
            split1.TabIndex = 4;
            // 
            // table1
            // 
            table1.AutoScroll = true;
            table1.AutoScrollMinSize = new Size(0, 300);
            table1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            table1.ColumnCount = 1;
            table1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            table1.Dock = DockStyle.Fill;
            table1.Location = new Point(0, 0);
            table1.Margin = new Padding(1);
            table1.MinimumSize = new Size(632, 300);
            table1.Name = "table1";
            table1.RowCount = 1;
            table1.RowStyles.Add(new RowStyle());
            table1.RowStyles.Add(new RowStyle());
            table1.Size = new Size(652, 346);
            table1.TabIndex = 0;
            // 
            // status1
            // 
            status1.Location = new Point(0, 76);
            status1.Name = "status1";
            status1.Size = new Size(800, 22);
            status1.TabIndex = 1;
            status1.Text = "statusStrip1";
            // 
            // logView
            // 
            logView.BackgroundColor = SystemColors.ControlLight;
            logView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            logView.Columns.AddRange(new DataGridViewColumn[] { id, message, type, time });
            logView.Dock = DockStyle.Fill;
            logView.Location = new Point(0, 0);
            logView.Name = "logView";
            logView.RowTemplate.Height = 25;
            logView.Size = new Size(800, 98);
            logView.TabIndex = 0;
            // 
            // id
            // 
            id.HeaderText = "ID";
            id.Name = "id";
            id.ReadOnly = true;
            // 
            // message
            // 
            message.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            message.HeaderText = "消息";
            message.Name = "message";
            message.ReadOnly = true;
            // 
            // type
            // 
            type.HeaderText = "类型";
            type.Name = "type";
            type.ReadOnly = true;
            // 
            // time
            // 
            time.HeaderText = "时间";
            time.Name = "time";
            time.ReadOnly = true;
            // 
            // InfluxDbToken
            // 
            InfluxDbToken.DisplayStyle = ToolStripItemDisplayStyle.Image;
            InfluxDbToken.Image = (Image)resources.GetObject("InfluxDbToken.Image");
            InfluxDbToken.ImageTransparentColor = Color.Magenta;
            InfluxDbToken.Name = "InfluxDbToken";
            InfluxDbToken.Size = new Size(23, 22);
            InfluxDbToken.Text = "Token";
            InfluxDbToken.Click += InfluxDbToken_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 498);
            Controls.Add(split_0);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Main";
            Text = "PLC记录机";
            Load += Main_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            split_0.Panel1.ResumeLayout(false);
            split_0.Panel2.ResumeLayout(false);
            split_0.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)split_0).EndInit();
            split_0.ResumeLayout(false);
            split1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)split1).EndInit();
            split1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)logView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem menu_manage;
        private ToolStripMenuItem menu_io;
        private ToolStripMenuItem menu_curve;
        private ToolStripMenuItem menu_curve_history;
        private ToolStrip toolStrip1;
        private ToolStripButton action_start;
        private ToolStripButton acttion_stop;
        private ToolStripButton btnIO;
        private ToolStripButton action_history;
        private SplitContainer split_0;
        private SplitContainer split1;
        private TableLayoutPanel table1;
        private DataGridView logView;
        private DataGridViewTextBoxColumn id;
        private DataGridViewTextBoxColumn message;
        private DataGridViewTextBoxColumn type;
        private DataGridViewTextBoxColumn time;
        private StatusStrip status1;
        private ToolStripButton action_exit;
        private ToolStripButton InfluxDbToken;
    }
}