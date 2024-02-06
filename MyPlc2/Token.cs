using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPlc2
{
    public partial class Token : Form
    {
        private readonly string config_path = AppDomain.CurrentDomain.BaseDirectory + "\\config\\";
        public Token()
        {
            InitializeComponent();
        }

        private void Token_Load(object sender, EventArgs e)
        {
            //读入令牌
            using (StreamReader reader = new(this.config_path + "\\token"))
            {
                string s = reader.ReadToEnd();
                CurrentToken.Text = s;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if(NewToken.Text.Trim().Length > 10)
            {
                using (StreamWriter writer = new StreamWriter(this.config_path + "\\token")){
                    string s = NewToken.Text.Trim();
                    writer.WriteLine(s);
                    CurrentToken.Text = s;
                }
            }
        }
    }
}
