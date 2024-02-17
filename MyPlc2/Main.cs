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
        private S7Client? Client;

        private Queue<Vc_d> queue_10ms = new();
        private Queue<Vc_d> queue_20ms = new();
        private Queue<Vc_d> queue_50ms = new();
        private Queue<Vc_d> queue_100ms = new();
        private Queue<Vc_d> queue_1s = new();

        private MyWorker? worker_10;
        private MyWorker? worker_20;
        private MyWorker? worker_50;
        private MyWorker? worker_100;
        private MyWorker? worker_1s;

        private static Mutex mutex = new Mutex(false);

        private List<Record> records = new();
        private Dictionary<string, Vc_d> vcs = new();

        private int CountOfClientLost = 0;
        private bool ConnectState = false;

        private MTreeView mTreeView = new();

        private readonly string config_path = AppDomain.CurrentDomain.BaseDirectory + "\\config\\";

        public event EventHandler<UpdatePlcClientEventArgs>? UpdatePlcClientEvent;
        private delegate void UpdateStripLabelDelegate(ref ToolStripLabel label, System.Drawing.Color color);
        private UpdateStripLabelDelegate updateStripLabelDelegate = UpdateStripLabel;

        //����
        private Siemens400 Io;

        public Main()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Main_FormClosing);

            //�˵�
            mTreeView.CheckBoxes = false;
            mTreeView.Dock = DockStyle.Fill;
            this.split1.Panel1.Controls.Add(mTreeView);

            //ί�� ˫��
            mTreeView.dblClickDelegate += DblClicHandler;

        }

        private void CreateVcQueue()
        {
            //��ն���
            ClearQueue();

            //Vc_d�����
            records = Vc_d.Parse();
            foreach (Record r in records)
            {
                if (r == null) continue;
                Vc_d vc = new(Client, r);
                vc.itemDropped += ItemDroppedHandler;

                //��¼���б�
                vcs.Add(r.Address, vc);

                if (r.Cycle == "10ms") queue_10ms.Enqueue(vc);
                if (r.Cycle == "20ms") queue_20ms.Enqueue(vc);
                if (r.Cycle == "50ms") queue_50ms.Enqueue(vc);
                if (r.Cycle == "100ms") queue_100ms.Enqueue(vc);
                if (r.Cycle == "1s") queue_1s.Enqueue(vc);

            }
            //��������
            //worker_10.ReadEvent -= 
            foreach (var vc in vcs)
            {
                Vc_d vc_d = vc.Value;

                worker_10.ReadEvent += vc_d.HandleReadEvent;
                worker_20.ReadEvent += vc_d.HandleReadEvent;
                worker_50.ReadEvent += vc_d.HandleReadEvent;
                worker_100.ReadEvent += vc_d.HandleReadEvent;
                worker_1s.ReadEvent += vc_d.HandleReadEvent;

                UpdatePlcClientEvent += vc_d.HandleUpdatePlcClientEvent;
                UpdatePlcClientEvent += vc_d.HandleUpdatePlcClientEvent;
                UpdatePlcClientEvent += vc_d.HandleUpdatePlcClientEvent;
                UpdatePlcClientEvent += vc_d.HandleUpdatePlcClientEvent;
                UpdatePlcClientEvent += vc_d.HandleUpdatePlcClientEvent;
            }

        }

        //��������ն���
        private void ClearQueue(bool stop = false)
        {
            queue_10ms.Clear();
            queue_20ms.Clear();
            queue_50ms.Clear();
            queue_100ms.Clear();
            queue_1s.Clear();

            vcs.Clear();

            if (stop)
            {
                worker_10.Running = false;
                worker_20.Running = false;
                worker_50.Running = false;
                worker_100.Running = false;
                worker_1s.Running = false;
            }
            else
            {
                worker_10.Running = true;
                worker_20.Running = true;
                worker_50.Running = true;
                worker_100.Running = true;
                worker_1s.Running = true;
            }

        }

        //��Ӧ�����β˵�˫�� 1
        private void DblClicHandler(string name, string address)
        {
            //Debug.WriteLine(name + " " + address);
            try
            {
                var vc = vcs[address];
                FormsPlot d = vc.MFormsPlot;
                //������
                d.Anchor = AnchorStyles.Left;
                d.Dock = DockStyle.Fill;
                //��С�߶�
                d.MinimumSize = new Size(0, 300);

                table1.Controls.Add(d);

                vc.delePlot += DeletePlotHandler;
                vc.AddStreamer();
            }
            catch (KeyNotFoundException ex)
            {
                Debug.WriteLine("DblClicHandler��" + ex.Message);
            }

        }

        //����ɾ��ͼ��
        private void DeletePlotHandler(string address)
        {
            var vc = vcs[address];
            vc.ClearPlot(vc.MFormsPlot);
            table1.Controls.Remove(vc.MFormsPlot);
        }

        //����item�Ϸ� 2
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

        private void ActionIoClick(object sender, EventArgs e)
        {
            //
        }

        private void BtnIO_Click(object sender, EventArgs e)
        {
            // ��PLC��IO����ҳ
            Siemens400 siemens = new Siemens400();
            siemens.sendMsg += new Siemens400.SendMsg(ReceiveMsg);
            siemens.Show();

        }

        //��Ϣ��plc�����ı�
        private void ReceiveMsg(bool isApplied, string s)
        {
            this.updateVar = isApplied;
            Debug.WriteLine("������" + s);
            //��������tree�˵�
            mTreeView.ReadVars(s);

            //���¼�¼����
            //ֹͣ�����߳�
            //VcIntoQueue();

        }

        private void Main_Load(object sender, EventArgs e)
        {
            //���壺����

            //��ʼ������IO����
            Io = new();

            //ChangeScreenSize();
            //������λ��
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

            //�����߳�
            worker_10 = new(queue_10ms, 10, mutex);
            worker_20 = new(queue_20ms, 20, mutex);
            worker_50 = new(queue_50ms, 50, mutex);
            worker_100 = new(queue_100ms, 100, mutex);
            worker_1s = new(queue_1s, 1000, mutex);

            //�̳߳�
            ThreadPool.QueueUserWorkItem(worker_10.Run, worker_10);
            ThreadPool.QueueUserWorkItem(worker_20.Run, worker_20);
            ThreadPool.QueueUserWorkItem(worker_50.Run, worker_50);
            ThreadPool.QueueUserWorkItem(worker_100.Run, worker_100);
            ThreadPool.QueueUserWorkItem(worker_1s.Run, worker_1s);

            //PLC����
            ThreadPool.QueueUserWorkItem(CheckPlcConnect);
        }

        //�¼����ر�
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
            //MyClose();
        }


        // �ı䰴ť����ɫ
        private void BtnStartBgColor(bool a, bool b)
        {
            if (a)
                action_start.BackColor = System.Drawing.Color.Green;
            else
                action_start.BackColor = SystemColors.Control;
        }

        //action������
        private void ActionStartClick(object sender, EventArgs e)
        {
            //�ı䰴ť����ɫ
            BtnStartBgColor(true, false);
            //���ɶ�����
            CreateVcQueue();
        }

        //action:ֹͣ
        private void ActionStopClick(object sender, EventArgs e)
        {
            //��ť����ɫ
            BtnStartBgColor(false, true);
            //��ն�����
            ClearQueue(true);
        }

        //��ʷ���ݷ���
        private void ActionHistoryClick(object sender, EventArgs e)
        {
            //�����ⲿ������ʷ����

        }

        //��ʱ���s7 client����
        public void CheckPlcConnect(object threadContext)
        {
            while (true)
            {
                if (!ConnectState && Io.GetConnected())
                {
                    ConnectState = true;
                    Client = Io.GetClient();
                    OnRaiseUpdatePlcClientEvent(Client);
                }

                if (!Io.GetConnected())
                {
                    ConnectState = false;

                    Io.TryConnect();
                }

                if (Io.GetConnected())
                {
                    //����
                    CountOfClientLost = 0;
                    //PlcStrip.BackColor = System.Drawing.Color.FromArgb(0, 0, 255);
                    //updateStripLabelDelegate(ref PlcStrip, System.Drawing.Color.FromArgb(0, 0, 255));

                }
                else
                {
                    CountOfClientLost++;
                    //PlcStrip.BackColor = System.Drawing.Color.FromArgb(192, 0, 0);
                    if (CountOfClientLost % 10 == 0)
                    {
                        Debug.WriteLine("PLC���Ӷ�ʧ��" + CountOfClientLost.ToString());
                    }
                }


                Thread.Sleep(5000);
            }
        }

        //���򴰿ڳߴ�
        private void ChangeScreenSize()
        {
            int boundWidth = Screen.PrimaryScreen.Bounds.Width;
            int boundHeight = Screen.PrimaryScreen.Bounds.Height;
            int x = boundWidth - this.Width;
            int y = boundHeight - this.Height;
            this.Location = new Point(x / 2, y / 2);
        }

        //action���˳�
        private void ActionExitClick(object sender, EventArgs e)
        {
            //�رմ���
            Close();
        }

        private void MyClose()
        {
            var result = MessageBox.Show("ȷ���˳���", "����", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                //���洰��λ��
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

                //ֹͣ�߳�

                //�ر�
                //Close();
                Dispose();
                Application.Exit();
            }
            else
            {
                //��С��
                WindowState = FormWindowState.Minimized;
            }
        }

        //���ڣ�����
        private void InfluxDbToken_Click(object sender, EventArgs e)
        {
            Token token = new();
            token.Show();
        }

        protected virtual void OnRaiseUpdatePlcClientEvent(S7Client client)
        {
            UpdatePlcClientEvent?.Invoke(this, new UpdatePlcClientEventArgs(client));
        }

        private static void UpdateStripLabel(ref ToolStripLabel label, System.Drawing.Color color)
        {
            label.ForeColor = color;
        }
        //
    }
}