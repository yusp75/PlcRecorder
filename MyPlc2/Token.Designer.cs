namespace MyPlc2
{
    partial class Token
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Token));
            label1 = new Label();
            CurrentToken = new TextBox();
            NewToken = new TextBox();
            BtnSave = new Button();
            BtnCancel = new Button();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(64, 48);
            label1.Name = "label1";
            label1.Size = new Size(239, 17);
            label1.TabIndex = 0;
            label1.Text = "InfuluxDB数据库口令环，Bucket名为plc。";
            // 
            // CurrentToken
            // 
            CurrentToken.Location = new Point(64, 105);
            CurrentToken.Multiline = true;
            CurrentToken.Name = "CurrentToken";
            CurrentToken.ReadOnly = true;
            CurrentToken.Size = new Size(649, 96);
            CurrentToken.TabIndex = 1;
            // 
            // NewToken
            // 
            NewToken.Location = new Point(64, 226);
            NewToken.Multiline = true;
            NewToken.Name = "NewToken";
            NewToken.Size = new Size(649, 96);
            NewToken.TabIndex = 2;
            // 
            // BtnSave
            // 
            BtnSave.Location = new Point(509, 378);
            BtnSave.Name = "BtnSave";
            BtnSave.Size = new Size(75, 23);
            BtnSave.TabIndex = 3;
            BtnSave.Text = "保存";
            BtnSave.UseVisualStyleBackColor = true;
            BtnSave.Click += BtnSave_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(638, 378);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(75, 23);
            BtnCancel.TabIndex = 4;
            BtnCancel.Text = "取消";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += BtnCancel_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(725, 21);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(63, 64);
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // Token
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(pictureBox1);
            Controls.Add(BtnCancel);
            Controls.Add(BtnSave);
            Controls.Add(NewToken);
            Controls.Add(CurrentToken);
            Controls.Add(label1);
            Name = "Token";
            Text = "Token";
            Load += Token_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox CurrentToken;
        private TextBox NewToken;
        private Button BtnSave;
        private Button BtnCancel;
        private PictureBox pictureBox1;
    }
}