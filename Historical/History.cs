using Serilog;
using System.Collections.Specialized;
using System.Diagnostics;
using MyPlc2;
using ScottPlot;
using ScottPlot.WinForms;

namespace Historical
{
    public partial class History : Form
    {

        private List<Record> records = new();
        private readonly Dictionary<string, Vc_h> vcs = new();

        private string Addresses;
        private readonly OrderedDictionary data = new();

        //事件：读值
        public event EventHandler<UpdateDataEventArgs> UpdateDataEvent;

        public History()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(History_FormClosing);

            MTreeView mTreeView = new();
            mTreeView.CheckBoxes = false;
            mTreeView.Dock = DockStyle.Fill;
            this.split1.Panel1.Controls.Add(mTreeView);

            //委托 双击
            mTreeView.dblClickDelegate += DblClicHandler;
            mTreeView.clickDelegate += ClickHandler;

            QueryAddresses(mTreeView);
        }

        private void ClickHandler(List<string> addresses)
        {
            //为flux拼凑array形式字符串
            Addresses = "[";
            for (int i = 0; i < addresses.Count; i++)
            {
                Addresses += "\"" + addresses[i] + "\",";
            }
            Addresses = Addresses.Substring(0, Addresses.Length - 1);
            Addresses += "]";
        }

        private void QueryAddresses(MTreeView mTreeView)
        {
            ClickHandler(mTreeView.AddressesList);
        }

        private void History_Load(object sender, EventArgs e)
        {
            //载入

            //初始化时间范围
            DateTime dt = DateTime.Now;
            DtStart.Value = dt.AddMinutes(-30);
            //DtStart.Value = dt.AddDays(-1);
            DtStop.Value = dt;

            //读取变量
            records = Vc_h.Parse();
            records.ForEach(record =>
            {
                Vc_h vc_h = new(record);
                vc_h.itemDropped += ItemDroppedHandler;

                //记录到列表
                vcs.Add(record.Address, vc_h);
                this.UpdateDataEvent += vc_h.HandleUpdateDataEvent;
            });

        }

        //事件：关闭
        private void History_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        //响应：树形菜单双击
        private void DblClicHandler(string name, string address)
        {
            //Log.Information(name + " " + address);
            Debug.WriteLine(name + " " + address);
            try
            {
                var vc = vcs[address];
                FormsPlot d = vc.MFormsPlot;
                d.Plot.Axes.DateTimeTicksBottom();
                d.Anchor = AnchorStyles.Left;
                d.Dock = DockStyle.Fill;
                //最小高度
                d.MinimumSize = new Size(0, 300);

                table1.Controls.Add(d);
                //关联
                vc.delePlot += DeletePlotHandler;
                vc.NearestValueEvent += HandleNearestValueEvent;

                vc.AddPlot();
            }
            catch (KeyNotFoundException ex)
            {
                Debug.WriteLine("DblClicHandler：" + ex.Message);
                //Log.Information("DblClicHandler：" + ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                UpdateStatus(ex.Message);
            }

        }

        //处理：显示最近值
        private void HandleNearestValueEvent(object? sender, NearestValueEventArgs e)
        {
            var v = e;
        }

        //处理：删除图形
        private void DeletePlotHandler(string address)
        {
            var vc = vcs[address];
            vc.ClearPlot(vc.MFormsPlot);
            table1.Controls.Remove(vc.MFormsPlot);
        }

        //处理：item拖放
        private void ItemDroppedHandler(string address, string parent)
        {
            try
            {
                Vc_h vc1 = vcs[address];
                Vc_h vc_p = vcs[parent];

                MPoint mPoint = vc1.GetPoints();
                if (mPoint.x.Count > 0)
                {
                    vc_p.AddPlot(address, mPoint);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        private void btnQuery_Click(object sender, EventArgs e)
        {
            //按钮：查询
            Db db = new();

            //var a = NodaTime.Instant.FromDateTimeUtc(DtStart.Value);
            //string dt1=NodaTime.Instant.FromDateTimeUtc(DtStart.Value).ToString();
            //string dt2 = NodaTime.Instant.FromDateTimeUtc(DtStop.Value).ToString();

            string dt1 = DtStart.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            string dt2 = DtStop.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

            ThreadPool.QueueUserWorkItem((obj) =>
            {
                Dictionary<string, MPoint> records = db.Query(Addresses, dt1, dt2).Result;
                UpdateStatus("记录数：" + records.Count.ToString());

                foreach (var d in records)
                {
                    string a = d.Key;
                    //vcs[a].UpdateData(d.Key, d.Value);
                    OnRaiseUpdateDataEvent(a, d.Value);
                }

            });

            //List<MyTypes> r = db.Query(dt1, dt2).Result;
            Debug.WriteLine("async read complete");
        }

        //代理 更新状态栏
        private void UpdateStatus(string msg)
        {
            if (status1.InvokeRequired)
            {
                Action<string> action = delegate (string msg) { status1.Items[0].Text = msg; };
                status1.Invoke(action, msg);
            }
            else
            {
                status1.Items[0].Text = "";
                status1.Items[0].Text = msg;
            }
        }

        //单击 关闭FormPlot缩放
        private void table1_Click(object sender, EventArgs e)
        {
            ChangeScrollMode(false);
        }

        //设置Vc_h滚动轮
        private void ChangeScrollMode(bool b)
        {
            foreach (var v in vcs)
            {
                //v.Value.MFormsPlot.Configuration.ScrollWheelZoom = b;
            }
        }

        //事件：触发
        protected virtual void OnRaiseUpdateDataEvent(string address, MPoint poinsts)
        {
            EventHandler<UpdateDataEventArgs> raiseEvent = UpdateDataEvent;
            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                // raise the event.
                raiseEvent(this, new UpdateDataEventArgs(address, poinsts));
            }
        }

        //class
    }


    //namespace
}