namespace Historical
{
    partial class History
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
            table = new TableLayoutPanel();
            status1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            split1 = new SplitContainer();
            table0 = new TableLayoutPanel();
            groupBox1 = new GroupBox();
            btnQuery = new Button();
            label1 = new Label();
            DtStop = new DateTimePicker();
            DtStart = new DateTimePicker();
            label2 = new Label();
            table1 = new TableLayoutPanel();
            dg1 = new DataGridView();
            col_address = new DataGridViewTextBoxColumn();
            col_X = new DataGridViewTextBoxColumn();
            col_Y = new DataGridViewTextBoxColumn();
            table.SuspendLayout();
            status1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)split1).BeginInit();
            split1.Panel2.SuspendLayout();
            split1.SuspendLayout();
            table0.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dg1).BeginInit();
            SuspendLayout();
            // 
            // table
            // 
            table.ColumnCount = 1;
            table.ColumnStyles.Add(new ColumnStyle());
            table.Controls.Add(status1, 0, 2);
            table.Controls.Add(split1, 0, 0);
            table.Controls.Add(dg1, 0, 1);
            table.Dock = DockStyle.Fill;
            table.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            table.Location = new Point(0, 0);
            table.MinimumSize = new Size(652, 120);
            table.Name = "table";
            table.RowCount = 3;
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 82.9473648F));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 17.0526314F));
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            table.Size = new Size(800, 496);
            table.TabIndex = 10;
            // 
            // status1
            // 
            status1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            status1.Location = new Point(0, 475);
            status1.Name = "status1";
            status1.Size = new Size(806, 21);
            status1.TabIndex = 13;
            status1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(0, 16);
            // 
            // split1
            // 
            split1.Dock = DockStyle.Fill;
            split1.Location = new Point(3, 3);
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
            split1.Panel2.Controls.Add(table0);
            split1.Size = new Size(800, 388);
            split1.SplitterDistance = 144;
            split1.TabIndex = 5;
            // 
            // table0
            // 
            table0.AutoSize = true;
            table0.ColumnCount = 1;
            table0.ColumnStyles.Add(new ColumnStyle());
            table0.Controls.Add(groupBox1, 0, 0);
            table0.Controls.Add(table1, 0, 1);
            table0.Dock = DockStyle.Fill;
            table0.Location = new Point(0, 0);
            table0.MinimumSize = new Size(652, 120);
            table0.Name = "table0";
            table0.RowCount = 2;
            table0.RowStyles.Add(new RowStyle(SizeType.Percent, 11.333333F));
            table0.RowStyles.Add(new RowStyle(SizeType.Percent, 88.6666641F));
            table0.Size = new Size(652, 388);
            table0.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnQuery);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(DtStop);
            groupBox1.Controls.Add(DtStart);
            groupBox1.Controls.Add(label2);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(3, 3);
            groupBox1.MinimumSize = new Size(646, 42);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(646, 42);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            // 
            // btnQuery
            // 
            btnQuery.Location = new Point(535, 13);
            btnQuery.Name = "btnQuery";
            btnQuery.Size = new Size(75, 23);
            btnQuery.TabIndex = 7;
            btnQuery.Text = "查询";
            btnQuery.UseVisualStyleBackColor = true;
            btnQuery.Click += btnQuery_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 19);
            label1.Name = "label1";
            label1.Size = new Size(32, 17);
            label1.TabIndex = 4;
            label1.Text = "开始";
            // 
            // DtStop
            // 
            DtStop.CustomFormat = "dd/MM/yyyy HH:mm";
            DtStop.Format = DateTimePickerFormat.Custom;
            DtStop.Location = new Point(281, 12);
            DtStop.Name = "DtStop";
            DtStop.Size = new Size(159, 23);
            DtStop.TabIndex = 6;
            // 
            // DtStart
            // 
            DtStart.CustomFormat = "dd/MM/yyyy HH:mm";
            DtStart.Format = DateTimePickerFormat.Custom;
            DtStart.Location = new Point(55, 13);
            DtStart.Name = "DtStart";
            DtStart.Size = new Size(159, 23);
            DtStart.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(243, 18);
            label2.Name = "label2";
            label2.Size = new Size(32, 17);
            label2.TabIndex = 5;
            label2.Text = "结束";
            // 
            // table1
            // 
            table1.AutoScroll = true;
            table1.ColumnCount = 1;
            table1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            table1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            table1.Dock = DockStyle.Fill;
            table1.Location = new Point(3, 46);
            table1.MinimumSize = new Size(640, 0);
            table1.Name = "table1";
            table1.RowCount = 1;
            table1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            table1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            table1.Size = new Size(646, 339);
            table1.TabIndex = 13;
            // 
            // dg1
            // 
            dg1.AllowUserToDeleteRows = false;
            dg1.AllowUserToOrderColumns = true;
            dg1.BackgroundColor = SystemColors.Control;
            dg1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dg1.Columns.AddRange(new DataGridViewColumn[] { col_address, col_X, col_Y });
            dg1.Dock = DockStyle.Bottom;
            dg1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dg1.Location = new Point(3, 401);
            dg1.Name = "dg1";
            dg1.RowTemplate.Height = 25;
            dg1.ScrollBars = ScrollBars.Vertical;
            dg1.Size = new Size(800, 71);
            dg1.TabIndex = 6;
            // 
            // col_address
            // 
            col_address.HeaderText = "地址";
            col_address.Name = "col_address";
            col_address.ReadOnly = true;
            // 
            // col_X
            // 
            col_X.HeaderText = "X值";
            col_X.MinimumWidth = 100;
            col_X.Name = "col_X";
            col_X.ReadOnly = true;
            col_X.Width = 200;
            // 
            // col_Y
            // 
            col_Y.HeaderText = "Y值";
            col_Y.MinimumWidth = 20;
            col_Y.Name = "col_Y";
            col_Y.ReadOnly = true;
            // 
            // History
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 496);
            Controls.Add(table);
            Name = "History";
            Text = "历史-PLC记录机";
            Load += History_Load;
            table.ResumeLayout(false);
            table.PerformLayout();
            status1.ResumeLayout(false);
            status1.PerformLayout();
            split1.Panel2.ResumeLayout(false);
            split1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)split1).EndInit();
            split1.ResumeLayout(false);
            table0.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dg1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private StatusStrip statusStrip1;
        private TableLayoutPanel table;
        private SplitContainer split1;
        private TableLayoutPanel table0;
        private GroupBox groupBox1;
        private Button btnQuery;
        private Label label1;
        private DateTimePicker DtStop;
        private DateTimePicker DtStart;
        private Label label2;
        private TableLayoutPanel table1;
        private StatusStrip status1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private DataGridView dg1;
        private DataGridViewTextBoxColumn col_address;
        private DataGridViewTextBoxColumn col_X;
        private DataGridViewTextBoxColumn col_Y;
    }
}