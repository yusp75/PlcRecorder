namespace MyPlc2
{
    partial class Siemens400
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
            components = new System.ComponentModel.Container();
            groupBox1 = new GroupBox();
            btnSave = new Button();
            lblPlcConnectd = new Label();
            txtSlot = new NumericUpDown();
            btnConnect = new Button();
            txtIP = new MaskedTextBox();
            label2 = new Label();
            label1 = new Label();
            view1 = new DataGridView();
            name = new DataGridViewTextBoxColumn();
            address = new DataGridViewTextBoxColumn();
            type = new DataGridViewComboBoxColumn();
            cycle = new DataGridViewComboBoxColumn();
            comment = new DataGridViewTextBoxColumn();
            active = new DataGridViewCheckBoxColumn();
            menu1 = new ContextMenuStrip(components);
            删除行ToolStripMenuItem = new ToolStripMenuItem();
            label3 = new Label();
            btnVarCancel = new Button();
            btnVarApply = new Button();
            btnEdit = new Button();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)txtSlot).BeginInit();
            ((System.ComponentModel.ISupportInitialize)view1).BeginInit();
            menu1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnSave);
            groupBox1.Controls.Add(lblPlcConnectd);
            groupBox1.Controls.Add(txtSlot);
            groupBox1.Controls.Add(btnConnect);
            groupBox1.Controls.Add(txtIP);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(776, 58);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "西门子300/400";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(607, 21);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 7;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += BtnSavePlc_Click;
            // 
            // lblPlcConnectd
            // 
            lblPlcConnectd.AutoSize = true;
            lblPlcConnectd.Location = new Point(391, 26);
            lblPlcConnectd.Name = "lblPlcConnectd";
            lblPlcConnectd.Size = new Size(44, 17);
            lblPlcConnectd.TabIndex = 6;
            lblPlcConnectd.Text = "未连接";
            // 
            // txtSlot
            // 
            txtSlot.Location = new Point(277, 25);
            txtSlot.Maximum = new decimal(new int[] { 3, 0, 0, 0 });
            txtSlot.Name = "txtSlot";
            txtSlot.Size = new Size(49, 23);
            txtSlot.TabIndex = 5;
            txtSlot.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(516, 21);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(75, 23);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "连接";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += BtnConnect_Click;
            // 
            // txtIP
            // 
            txtIP.Location = new Point(48, 23);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(140, 23);
            txtIP.TabIndex = 3;
            txtIP.Text = "127.0.0.1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(231, 29);
            label2.Name = "label2";
            label2.Size = new Size(30, 17);
            label2.TabIndex = 2;
            label2.Text = "Slot";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 25);
            label1.Name = "label1";
            label1.Size = new Size(19, 17);
            label1.TabIndex = 0;
            label1.Text = "IP";
            // 
            // view1
            // 
            view1.AllowUserToOrderColumns = true;
            view1.BackgroundColor = SystemColors.ButtonFace;
            view1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            view1.Columns.AddRange(new DataGridViewColumn[] { name, address, type, cycle, comment, active });
            view1.ContextMenuStrip = menu1;
            view1.EditMode = DataGridViewEditMode.EditOnEnter;
            view1.Enabled = false;
            view1.GridColor = SystemColors.ActiveCaption;
            view1.Location = new Point(12, 105);
            view1.MultiSelect = false;
            view1.Name = "view1";
            view1.RowTemplate.Height = 25;
            view1.Size = new Size(776, 340);
            view1.TabIndex = 1;
            view1.MouseDown += view1_MouseDown;
            // 
            // name
            // 
            name.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            name.HeaderText = "名称";
            name.Name = "name";
            // 
            // address
            // 
            address.HeaderText = "地址";
            address.Name = "address";
            // 
            // type
            // 
            type.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            type.HeaderText = "类型";
            type.Items.AddRange(new object[] { "bool", "int", "real" });
            type.Name = "type";
            // 
            // cycle
            // 
            cycle.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            cycle.HeaderText = "采样周期";
            cycle.Items.AddRange(new object[] { "10ms", "20ms", "50ms", "100ms", "1s" });
            cycle.Name = "cycle";
            // 
            // comment
            // 
            comment.HeaderText = "注解";
            comment.Name = "comment";
            // 
            // active
            // 
            active.HeaderText = "激活";
            active.Name = "active";
            // 
            // menu1
            // 
            menu1.Items.AddRange(new ToolStripItem[] { 删除行ToolStripMenuItem });
            menu1.Name = "menu1";
            menu1.Size = new Size(181, 48);
            // 
            // 删除行ToolStripMenuItem
            // 
            删除行ToolStripMenuItem.Name = "删除行ToolStripMenuItem";
            删除行ToolStripMenuItem.Size = new Size(180, 22);
            删除行ToolStripMenuItem.Text = "删除行";
            删除行ToolStripMenuItem.Click += DeleteItem_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 85);
            label3.Name = "label3";
            label3.Size = new Size(32, 17);
            label3.TabIndex = 2;
            label3.Text = "变量";
            // 
            // btnVarCancel
            // 
            btnVarCancel.Location = new Point(713, 463);
            btnVarCancel.Name = "btnVarCancel";
            btnVarCancel.Size = new Size(75, 23);
            btnVarCancel.TabIndex = 8;
            btnVarCancel.Text = "取消";
            btnVarCancel.UseVisualStyleBackColor = true;
            btnVarCancel.Click += BtnVarCancel_Click;
            // 
            // btnVarApply
            // 
            btnVarApply.Location = new Point(632, 463);
            btnVarApply.Name = "btnVarApply";
            btnVarApply.Size = new Size(75, 23);
            btnVarApply.TabIndex = 9;
            btnVarApply.Text = "保存";
            btnVarApply.UseVisualStyleBackColor = true;
            btnVarApply.Click += BtnVarApply_Click;
            // 
            // btnEdit
            // 
            btnEdit.Location = new Point(551, 463);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(75, 23);
            btnEdit.TabIndex = 10;
            btnEdit.Text = "编辑";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += BtnEdit_Click;
            // 
            // Siemens400
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 498);
            Controls.Add(btnEdit);
            Controls.Add(btnVarApply);
            Controls.Add(btnVarCancel);
            Controls.Add(label3);
            Controls.Add(view1);
            Controls.Add(groupBox1);
            Name = "Siemens400";
            Text = "PLC记录机-IO变量";
            Load += Siemens400_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)txtSlot).EndInit();
            ((System.ComponentModel.ISupportInitialize)view1).EndInit();
            menu1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private Label label1;
        private TextBox textBox1;
        private Label label2;
        private MaskedTextBox txtIP;
        private Button btnConnect;
        private NumericUpDown txtSlot;
        private Label lblPlcConnectd;
        private Button btnSave;
        private DataGridView view1;
        private Label label3;
        private Button btnVarCancel;
        private Button btnVarApply;
        private Button btnEdit;
        private DataGridViewTextBoxColumn name;
        private DataGridViewTextBoxColumn address;
        private DataGridViewComboBoxColumn type;
        private DataGridViewComboBoxColumn cycle;
        private DataGridViewTextBoxColumn comment;
        private DataGridViewCheckBoxColumn active;
        private ContextMenuStrip menu1;
        private ToolStripMenuItem 删除行ToolStripMenuItem;
    }
}