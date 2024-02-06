using ScottPlot;
using Serilog;
using Sharp7;
using System.Diagnostics;
using ScottPlot.WinForms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyPlc2
{
    public partial class Main : Form
    {
        private bool updateVar = false;
        private S7Client client;

        private Queue<Vc_d> queue_10ms = new();
        private Queue<Vc_d> queue_20ms = new();
        private Queue<Vc_d> queue_50ms = new();
        private Queue<Vc_d> queue_100ms = new();
        private Queue<Vc_d> queue_1s = new();

        private MyWorker worker_10;
        private MyWorker worker_20;
        private MyWorker worker_50;
        private MyWorker worker_100;
        private MyWorker worker_1s;

        private static Mutex mutex = new Mutex(false);

        private List<Record> records = new();
        private Dictionary<string, Vc_d> vcs = new();

        private int CountOfClientLost = 0;

        private MTreeView mTreeView = new();

        private readonly string config_path = AppDomain.CurrentDomain.BaseDirectory + "\\config\\";

        //窗体
        private siemens400 io;

        public Main()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Main_FormClosing);

            //菜单
            mTreeView.CheckBoxes = false;
            mTreeView.Dock = DockStyle.Fill;
            this.split1.Panel1.Controls.Add(mTreeView);

            //委托 双击
            mTreeView.dblClickDelegate += DblClicHandler;

            //右键菜单


        }

        private void VcIntoQueue()
        {
            //清空队列
            queue_10ms.Clear();
            queue_20ms.Clear();
            queue_50ms.Clear();
            queue_100ms.Clear();
            queue_1s.Clear();

            //Vc_d入队列
            records = Vc_d.Parse();
            foreach (Record r in records)
            {
                if (r == null) continue;
                Vc_d vc = new(client, r);
                vc.itemDropped += ItemDroppedHandler;


                //记录到列表
                vcs.Add(r.Address, vc);

                if (r.Cycle == "10ms") queue_10ms.Enqueue(vc);
                if (r.Cycle == "20ms") queue_20ms.Enqueue(vc);
                if (r.Cycle == "50ms") queue_50ms.Enqueue(vc);
                if (r.Cycle == "100ms") queue_100ms.Enqueue(vc);
                if (r.Cycle == "1s") queue_1s.Enqueue(vc);

            }

        }
        //响应：树形菜单双击 1
        private void DblClicHandler(string name, string address)
        {
            //Debug.WriteLine(name + " " + address);
            try
            {
                var vc = vcs[address];
                FormsPlot d = vc.MFormsPlot;
                //适配宽度
                d.Anchor = AnchorStyles.Left;
                d.Dock = DockStyle.Fill;
                //最小高度
                d.MinimumSize = new Size(0, 300);

                table1.Controls.Add(d);

                vc.delePlot += DeletePlotHandler;

                vc.AddStreamer();
            }
            catch (KeyNotFoundException ex)
            {
                Debug.WriteLine("DblClicHandler：" + ex.Message);
            }

        }

        //处理：删除图形
        private void DeletePlotHandler(string address)
        {
            var vc = vcs[address];
            vc.ClearPlot(vc.MFormsPlot);
            table1.Controls.Remove(vc.MFormsPlot);
        }

        //处理：item拖放 2
        private void ItemDroppedHandler(string address, string parent)
        {
            try
            {
                Vc_d vc1 = vcs[address];
                Vc_d vc_p = vcs[parent];

                vc_p.AddStreamer(address);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        //action：启动
        private void ActionStartClick(object sender, EventArgs e)
        {
            ChangeBgColor(true, false);

            try
            {
                io.TryConnect();
                this.client = io.GetClient();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return;
            }

            worker_10 = new(queue_10ms, client, 10, mutex);
            worker_20 = new(queue_20ms, client, 20, mutex);
            worker_50 = new(queue_50ms, client, 50, mutex);
            worker_100 = new(queue_100ms, client, 100, mutex);
            worker_1s = new(queue_1s, client, 1000, mutex);

            //订阅
            foreach (var vc in vcs)
            {
                Vc_d vc_d = vc.Value;
                worker_10.ReadEvent += vc_d.HandleReadEvent;
                worker_20.ReadEvent += vc_d.HandleReadEvent;
                worker_50.ReadEvent += vc_d.HandleReadEvent;
                worker_100.ReadEvent += vc_d.HandleReadEvent;
                worker_1s.ReadEvent += vc_d.HandleReadEvent;
            }

            //线程：读
            ThreadPool.QueueUserWorkItem(worker_10.Run, worker_10);
            ThreadPool.QueueUserWorkItem(worker_20.Run, worker_20);
            ThreadPool.QueueUserWorkItem(worker_50.Run, worker_50);
            ThreadPool.QueueUserWorkItem(worker_100.Run, worker_100);
            ThreadPool.QueueUserWorkItem(worker_1s.Run, worker_1s);

            //线程：PLC连接
            ThreadPool.QueueUserWorkItem(CheckPlcConnect);

        }

        private void ActionIoClick(object sender, EventArgs e)
        {
            //
        }

        private void BtnIO_Click(object sender, EventArgs e)
        {
            // 打开PLC　IO管理页
            siemens400 siemens = new siemens400();
            siemens.sendMsg += new siemens400.SendMsg(ReceiveMsg);
            siemens.Show();

        }

        //消息：plc变量改变
        private void ReceiveMsg(bool isApplied, string s)
        {
            this.updateVar = isApplied;
            Debug.WriteLine("变量：" + s);
            //读变量到tree菜单
            mTreeView.ReadVars(s);

            //更新记录变量
            //停止所有线程
            //VcIntoQueue();

        }

        private void Main_Load(object sender, EventArgs e)
        {
            //载入
            io = new();
            VcIntoQueue();
            //ChangeScreenSize();
            //读窗口位置
            try
            {
                using StreamReader reader = new(this.config_path + "\\window.json");
                string s = reader.ReadToEnd();
                if (s != null && s.Length > 0)
                {
                    JObject obj = JsonConvert.DeserializeObject<JObject>(s);

                    Left = (int)obj["left"];
                    Top = (int)obj["top"];
                    Width = (int)obj["width"];
                    Height = (int)obj["height"];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        //事件：关闭
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
            //MyClose();
        }


        // 按钮改变背景色
        private void ChangeBgColor(bool a, bool b)
        {
            if (a)
                action_start.BackColor = System.Drawing.Color.Green;
            else
                action_start.BackColor = SystemColors.Control;
        }

        //action:停止
        private void ActionStopClick(object sender, EventArgs e)
        {
            ChangeBgColor(false, true);
        }

        //历史数据分析
        private void ActionHistoryClick(object sender, EventArgs e)
        {
            //调用外部程序：历史曲线

        }

        //定时检查s7 client连接
        public void CheckPlcConnect(Object threadContext)
        {
            while (true)
            {
                if (!client.Connected)
                {
                    io.TryConnect();
                    this.client = io.GetClient();
                    if (this.client.Connected) { CountOfClientLost = 0; }
                    else
                    {
                        CountOfClientLost++;
                        if (CountOfClientLost % 10 == 0)
                        {
                            Debug.WriteLine("PLC连接丢失：" + CountOfClientLost.ToString());
                        }
                    }
                }
                Thread.Sleep(5000);
            }
        }


        //程序窗口尺寸
        private void ChangeScreenSize()
        {
            int boundWidth = Screen.PrimaryScreen.Bounds.Width;
            int boundHeight = Screen.PrimaryScreen.Bounds.Height;
            int x = boundWidth - this.Width;
            int y = boundHeight - this.Height;
            this.Location = new Point(x / 2, y / 2);
        }

        //action：退出
        private void ActionExitClick(object sender, EventArgs e)
        {
            //关闭窗体
            Close();
        }

        private void MyClose()
        {
            var result = MessageBox.Show("确定退出？", "警告", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                //保存窗口位置
                try
                {
                    using (StreamWriter writer = new(config_path + "window.json"))
                    {
                        string left = Left.ToString();
                        string top = Top.ToString();
                        string width = Width.ToString();
                        string height = Height.ToString();

                        var obj = new
                        {
                            left,
                            top,
                            width,
                            height,
                        };
                        writer.WriteLine(JsonConvert.SerializeObject(obj));
                    }
                }

                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                //停止线程

                //关闭
                //Close();
                Dispose();
                Application.Exit();
            }
            else
            {
                //最小化
                WindowState = FormWindowState.Minimized;
            }
        }

        //
    }
}