using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sharp7;

namespace MyPlc2
{
    public partial class siemens400 : Form
    {
        private bool isConnected = false;
        private readonly S7Client client = new S7Client();
        private bool inEdit = false;
        private readonly string config_path = System.AppDomain.CurrentDomain.BaseDirectory + "\\config\\";
        public delegate void SendMsg(bool isApplied);
        public SendMsg sendMsg;

        private MyLog log = new(null);

        public siemens400()
        {
            InitializeComponent();

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //连接
            this.Connect();

        }

        public void Connect()
        {
            String ip = this.txtIP.Text.Replace(" ", "");
            int rack = 0;
            int slot = (int)this.txtSlot.Value;
            //this.client.SetConnectionType(0x02);
            this.client.ConnTimeout = 5;

            if (!this.isConnected)
            {
                int code = this.client.ConnectTo(ip, rack, slot);
                if (code > 0)
                {
                    //MessageBox.Show("连接失败：" + this.client.ErrorText(code));
                    log.Error("连接失败：" + this.client.ErrorText(code));
                }

                this.isConnected = this.client.Connected;
                if (this.isConnected)
                {
                    this.UpdateLblStatus("已连接", Color.FromArgb(0, 255, 0));
                }
            }

        }

        private void siemens400_Activated(object sender, EventArgs e)
        {
            //窗体激活

            //生成配置文件目录
            if (!Directory.Exists(config_path))
            {
                Directory.CreateDirectory(this.config_path);
            }

            //读取PLC连接
            if (File.Exists(this.config_path + "plc.json"))
            {
                using StreamReader reader = new(this.config_path + "\\plc.json");
                string s = reader.ReadLine();

                //PlcConnect connect = new();
                //var obj = JsonConvert.DeserializeObject(s);
                var obj = JObject.Parse(s);
                txtIP.Text = obj.GetValue("ip").ToString();
                txtSlot.Text = obj.GetValue("slot").ToString();

            }

            //读取变量到数据表格
            if (File.Exists(this.config_path + "vars.json"))
            {
                using StreamReader reader = new(this.config_path + "\\vars.json");
                string s = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(s))
                {
                    List<PlcVar> vars = JsonConvert.DeserializeObject<List<PlcVar>>(s);
                    if (vars is not null && vars.Count > 0)
                    {
                        for (int i = 0; i < vars.Count; i++)
                        {
                            this.view1.Rows.Add();
                            this.view1.Rows[i].Cells["name"].Value = vars[i].name;
                            this.view1.Rows[i].Cells["address"].Value = vars[i].address;
                            this.view1.Rows[i].Cells["type"].Value = vars[i].type;
                            this.view1.Rows[i].Cells["cycle"].Value = vars[i].cycle;
                            this.view1.Rows[i].Cells["comment"].Value = vars[i].comment;
                            this.view1.Rows[i].Cells["active"].Value = vars[i].active;
                        }
                    }
                }

            } //存在变量文件
        }

        private void UpdateLblStatus(String msg, Color color)
        {
            //更新PLC连接状态
            this.lblPlcConnectd.Text = msg;
            this.lblPlcConnectd.BackColor = color;
        }

        public bool GetConnected()
        {
            return client.Connected;
        }

        public S7Client GetClient()
        {
            return this.client;
        }


        private void btnVarCancel_Click(object sender, EventArgs e)
        {
            //关闭变量IO
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            //编辑变量列表模式
            if (this.inEdit)
            {
                this.inEdit = false;
                this.view1.Enabled = false;
                this.btnEdit.BackColor = SystemColors.Control;
            }
            else
            {
                this.inEdit = true;
                this.view1.EditMode = DataGridViewEditMode.EditOnEnter;
                this.view1.Enabled = true;
                this.btnEdit.BackColor = Color.FromArgb(0, 255, 0);
            }
        }

        private void btnVarApply_Click(object sender, EventArgs e)
        {
            //按钮：应用
            //检查数值合理
            //保存变量到json
            List<PlcVar> list = new();
            foreach (DataGridViewRow row in this.view1.Rows)
            {
                if (row.Cells["name"].Value is null ||
                    row.Cells["address"].Value is null ||
                    row.Cells["cycle"].Value is null ||
                    row.Cells["type"].Value is null
                    )
                {
                    continue;
                }

                string comment = "";
                if (row.Cells["comment"].Value is not null)
                    comment = row.Cells["comment"].Value.ToString();

                try
                {
                    PlcVar plcVar = new(
                        row.Cells["name"].Value.ToString(),
                        row.Cells["address"].Value.ToString(),
                        row.Cells["type"].Value.ToString(),
                        row.Cells["cycle"].Value.ToString(),
                        comment,
                        (bool)row.Cells["active"].Value
                        );
                    list.Add(plcVar);
                }
                catch (NullReferenceException ex)
                {
                    Debug.WriteLine("变量填写错：" + ex.ToString());
                    continue;
                }
            }

            try
            {
                using StreamWriter writer = new(config_path + "vars.json");
                String s = JsonConvert.SerializeObject(list);
                writer.Write(s);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            //按钮按下，发送true
            this.sendMsg(true);

            //button
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //保存plc参数
            try
            {
                using StreamWriter writer = new(config_path + "plc.json");
                string ip = txtIP.Text.Replace(" ", "");
                int slot = Convert.ToInt16(txtSlot.Text.Trim());

                var obj = new
                {
                    ip = ip,
                    slot = slot
                };
                writer.WriteLine(JsonConvert.SerializeObject(obj));


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }
    }


    //
}
