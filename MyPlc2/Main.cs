using ScottPlot;
using Serilog;
using Sharp7;
using System.Diagnostics;

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


        //����
        private siemens400 io;

        public Main()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Main_FormClosing);

            mTreeView.CheckBoxes = false;
            mTreeView.Dock = DockStyle.Fill;
            this.split1.Panel1.Controls.Add(mTreeView);

            //ί�� ˫��
            mTreeView.dblClickDelegate += DblClicHandler;

        }

        private void VcIntoQueue()
        {
            //��ն���
            queue_10ms.Clear();
            queue_20ms.Clear();
            queue_50ms.Clear();
            queue_100ms.Clear();
            queue_1s.Clear();

            //Vc_d�����
            records = Vc_d.Parse();
            foreach (Record r in records)
            {
                if (r == null) continue;
                Vc_d vc = new(client, r);
                vc.itemDropped += ItemDroppedHandler;


                //��¼���б�
                vcs.Add(r.Address, vc);

                if (r.Cycle == "10ms") queue_10ms.Enqueue(vc);
                if (r.Cycle == "20ms") queue_20ms.Enqueue(vc);
                if (r.Cycle == "50ms") queue_50ms.Enqueue(vc);
                if (r.Cycle == "100ms") queue_100ms.Enqueue(vc);
                if (r.Cycle == "1s") queue_1s.Enqueue(vc);

            }

        }
        //��Ӧ�����β˵�˫�� 1
        private void DblClicHandler(string name, string address)
        {
            Debug.WriteLine(name + " " + address);
            try
            {
                ScottPlot.FormsPlot d = vcs[address].MFormsPlot;
                //������
                d.Anchor = AnchorStyles.Left;
                d.Dock = DockStyle.Fill;
                table1.Controls.Add(d);

                vcs[address].AddStreamer();
            }
            catch (KeyNotFoundException ex)
            {
                Debug.WriteLine("DblClicHandler��" + ex.Message);
            }

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

        //action������
        private void action_start_Click(object sender, EventArgs e)
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

            //����
            foreach (var vc in vcs)
            {
                Vc_d vc_d = vc.Value;
                worker_10.ReadEvent += vc_d.HandleReadEvent;
                worker_20.ReadEvent += vc_d.HandleReadEvent;
                worker_50.ReadEvent += vc_d.HandleReadEvent;
                worker_100.ReadEvent += vc_d.HandleReadEvent;
                worker_1s.ReadEvent += vc_d.HandleReadEvent;
            }

            //�̣߳���
            ThreadPool.QueueUserWorkItem(worker_10.Run, worker_10);
            ThreadPool.QueueUserWorkItem(worker_20.Run, worker_20);
            ThreadPool.QueueUserWorkItem(worker_50.Run, worker_50);
            ThreadPool.QueueUserWorkItem(worker_100.Run, worker_100);
            ThreadPool.QueueUserWorkItem(worker_1s.Run, worker_1s);
            //�̣߳�PLC����
            ThreadPool.QueueUserWorkItem(CheckPlcConnect);

        }

        private void Act_1(object sender, EventArgs e)
        {

        }

        private void btnIO_Click(object sender, EventArgs e)
        {
            // ��PLC��IO����ҳ
            siemens400 siemens = new siemens400();
            siemens.sendMsg += new siemens400.SendMsg(ReceiveMsg);
            siemens.Show();

        }

        //��Ϣ��plc�����ı�
        private void ReceiveMsg(bool isApplied)
        {
            this.updateVar = isApplied;
            Debug.WriteLine("�����޸���");
            //��������tree�˵�
            //mTreeView.ReadVars();

            //���¼�¼����
            //ֹͣ�����߳�
            //VcIntoQueue();

        }

        private void Main_Load(object sender, EventArgs e)
        {
            //����
            io = new();
            VcIntoQueue();
            ChangeScreenSize();

        }

        //�¼����ر�
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
            //minimize
            WindowState = FormWindowState.Minimized;
        }


        // ��ť�ı䱳��ɫ
        private void ChangeBgColor(bool a, bool b)
        {
            if (a)
                action_start.BackColor = Color.Green;
            else
                action_start.BackColor = SystemColors.Control;
        }

        //action:ֹͣ
        private void action_stop_Click(object sender, EventArgs e)
        {
            ChangeBgColor(false, true);
        }

        //��ʷ���ݷ���
        private void action_history_Click(object sender, EventArgs e)
        {
            //�����ⲿ������ʷ����

        }

        //��ʱ���s7 client����
        public void CheckPlcConnect(Object threadContext)
        {
            while (true)
            {
                if (!client.Connected)
                {
                    io.TryConnect();
                    this.client = io.GetClient();
                    if (this.client.Connected) { CountOfClientLost = 0; }
                    else { }
                    CountOfClientLost++;
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

        //
    }
}