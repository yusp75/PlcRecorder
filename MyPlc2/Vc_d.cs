using Sharp7;
using System.Diagnostics;
using ScottPlot;
using ScottPlot.AxisPanels;

namespace MyPlc2
{
    internal class MyWorker
    {
        private Queue<Vc_d> MQueue;
        private bool Running = true;
        private readonly int Cycle;
        private readonly S7Client Client;
        private Mutex mutex;

        //事件：读值
        public event EventHandler<ReadEventArgs> ReadEvent;

        public MyWorker(Queue<Vc_d> queue, S7Client client, int cycle, Mutex mutex)
        {
            MQueue = queue;
            Client = client;
            Cycle = cycle;
            this.mutex = mutex;
        }
        protected virtual void OnRaiseReadEvent(string address, double value)
        {
            EventHandler<ReadEventArgs> raiseEvent = ReadEvent;
            // Event will be null if there are no subscribers
            if (raiseEvent != null)
            {
                // raise the event.
                raiseEvent(this, new ReadEventArgs(address, value));
            }
        }
        public void Run(object threadContext)
        {
            while (Running)
            {
                if (MQueue.Count > 0)
                {
                    Vc_d vc = MQueue.Dequeue();
                    if (vc != null)
                    {
                        vc.SetClient(Client);

                        //使用锁信号
                        mutex.WaitOne();
                        if (Client != null && Client.Connected)
                        {
                            if (vc.Read() == 0)
                            {
                                string address = vc.Address;
                                double value = vc.Value;
                                OnRaiseReadEvent(address, value);
                            }
                        }
                        mutex.ReleaseMutex();

                        if (vc.Enable)
                        {
                            MQueue.Enqueue(vc);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Vc_d is null");
                    }
                }
                // Debug.WriteLine("线程ID"+threadContext.ToString());
                // 延迟1个cycle
                //Thread.Sleep(3000);
                Thread.Sleep(Cycle);

            } //while
        }
        public void SetRunning(bool running)
        {
            Running = running;
        }
    }

    #region 0

    #endregion

    //变量：解析，采集
    internal class Vc_d : Vc
    {
        public S7Client? Client;

        public string Address { get; set; }
        public double Value { get; set; }

        private List<LegendItem> Legends { get; set; } = new();

        //画面队列
        private readonly Dictionary<string, DataStreamer> StreamerArray = new();
        private AxisLimits AxisLitmits;
        private LeftAxis? Axis_01;

        //订阅
        private string _id;
        //刷新定时器
        private readonly System.Windows.Forms.Timer UpdatePlotTimer = new() { Interval = 10, Enabled = true };

        public Vc_d(S7Client? client, Record record) : base(record)
        {
            Client = client;
            //关联定时器更新函数
            UpdatePlotTimer.Tick += UpdatePlotTimer_Tick;
        }

        private void UpdatePlotTimer_Tick(object? sender, EventArgs e)
        {
            bool fresh = false;
            foreach (var a in StreamerArray)
            {
                var streamer = a.Value;
                if (streamer.HasNewData)
                {
                    streamer.SetupAxisX(1);

                    fresh = true;
                }
            }
            if (fresh)
            {
                MFormsPlot.Refresh();
            }
        }

        //双击：加streamer 1
        public void AddStreamer()
        {
            SetTitle();
            var plot = MFormsPlot.Plot;
            //plot.Axes.DateTimeTicksBottom();
            plot.Legend.Location = Alignment.UpperLeft;
            plot.Legend.IsVisible = true;

            //类型：DataStreamer
            var streamer = AddDataStreamer(1000);
            plot.Axes.AutoScale();
            streamer.ManageAxisLimits = false;
            AddLegend(MRecord.Address, streamer.Color);
            //轴颜色
            //plot.Axes.Color(streamer.Color);

            //布尔值，则绘制stepline
            if (Check01(MRecord.Address))
            {
                streamer.ConnectStyle = ConnectStyle.StepHorizontal;
                Axis_01 = (LeftAxis)plot.Axes.Left;
                Axis_01.Label.Text = "0-1";
                plot.Axes.SetLimitsY(0, 50, Axis_01);
                Axis_01_Count++;
            }
            else
            {
                var axis = (LeftAxis)plot.Axes.Left;
                axis.Label.Text = MRecord.Address;
                plot.Axes.SetLimitsY(0, 1000, axis);
                Axis_lg_Count++;
            }

            StreamerArray[MRecord.Address] = streamer;

        }

        //拖放：加streamer 2
        public void AddStreamer(string address)
        {
            if (MFormsPlot == null || StreamerArray.Keys.Contains(address)) return; //检查：空，重复

            const int BASE_LINE = 50;
            var plot = MFormsPlot.Plot;

            var streamer = AddDataStreamer(1000);
            streamer.ManageAxisLimits = false;
            streamer.Period = 0.5;

            //legend
            AddLegend(address, streamer.Color);

            if (Check01(address))
            {
                if (Axis_01 == null)
                {
                    Axis_01 = plot.Axes.AddLeftAxis();
                    Axis_01.Color(streamer.Color);
                    Axis_01.Label.Text = "0-1";
                    streamer.ConnectStyle = ConnectStyle.StepHorizontal;
                    //Axis_01.SetBoundary(0, 50);
                    //Axis_01.SetInnerBoundary(0, 10);
                    plot.Axes.SetLimitsY(0, 50, Axis_01);
                }

                streamer.Axes.YAxis = Axis_01;
                streamer.Data.OffsetY = 2 * Axis_01_Count;

                Axis_01_Count++;
            }
            else
            {
                var axis = plot.Axes.AddLeftAxis();
                axis.Color(streamer.Color);
                plot.Axes.SetLimitsY(0, 1000, axis);
                axis.Label.Text = address;
                streamer.Axes.YAxis = axis;
                streamer.Data.OffsetY = 10 * Axis_lg_Count + BASE_LINE;

                Axis_lg_Count++;
            }

            StreamerArray[address] = streamer;

            MFormsPlot.Refresh();
        }


        public override int Read()
        {
            //读PLC
            int code = -1;
            if (Client == null) return code;

            Record r = MRecord;
            int pos = -1;
            if (Check01(r.Address))
            {
                pos = r.Bit_pos;
            }

            //ReadArea(int Area, int DBNumber, int Start, int Amount, int WordLen, byte[] Buffer)

            byte[] data = new byte[S7.DataSizeByte(r.WordLen)];
            try
            {
                code = Client.ReadArea(r.Area, r.Db_number, r.Start, 1, r.WordLen, data);
            }
            catch (Exception ex)
            {
                return code;
            }

            if (code == 0)
            {
                string raw_value = ByteArrayToString(data);
                double value = CalByteArray(data, r.WordLen, pos);
                Debug.WriteLine("读{0}:{1}", r.Address, value);

                //向事件传递值
                this.Address = r.Address;
                this.Value = value;

                //db
                try
                {
                    Db db = new();
                    db.Write(r.Name, r.Address, r.Bit_pos, raw_value, value, r.WordLen);
                }
                catch (ArgumentException e)
                {
                    Debug.WriteLine("Vc.Read, db wrtie: " + e.ToString());
                }

            }

            return code;
        }

        public void UpdateData(MPoint points)
        {
            throw new NotImplementedException();
        }

        public void SetClient(S7Client client)
        {
            Client = client;
        }

        public void Subscriber(string id, MyWorker pub)
        {
            _id = id;

            // Subscribe to the event
            pub.ReadEvent += HandleReadEvent;
        }

        //事件处理：读取
        public void HandleReadEvent(object sender, ReadEventArgs e)
        {
            string address = e.Address;
            double value = e.Value;
            MFormsPlot.Plot.RenderManager.PreRenderLock += (s, e) =>
            {
                foreach (var streamer in StreamerArray)
                {
                    //只接收地址相同
                    if (address != null)
                    {
                        if (address.Equals(streamer.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            streamer.Value.Add(value);
                        }
                    }
                }
            };
            //更新
            //Refresh();
        }

        //取系统颜色
        public ScottPlot.Color GetNextColor()
        {
            List<KnownColor> colorList = Enum.GetValues(typeof(KnownColor)).Cast<KnownColor>().ToList();
            Random rand = new Random(DateTime.Now.Ticks.GetHashCode());
            var color = System.Drawing.Color.FromKnownColor(colorList[rand.Next(0, colorList.Count())]);
            return new ScottPlot.Color(color.R + 10, color.G, color.B);
        }
        public DataStreamer AddDataStreamer(int points)
        {
            double[] data = new double[points];
            var plot = MFormsPlot.Plot;

            DataStreamer streamer = new(plot, data)
            {
                Color = GetNextColor(),
                //Period = period,
            };

            plot.PlottableList.Add(streamer);

            return streamer;
        }

        private void AddLegend(string name, ScottPlot.Color color)
        {
            //避免重复legend
            if (Legends.Any(a => a.Label == name)) return;

            LegendItem item = new()
            {
                LineColor = color,
                MarkerColor = color,
                LineWidth = 2,
                Label = name
            };
            Legends.Add(item);
            MFormsPlot.Plot.ShowLegend(Legends, location: Alignment.UpperLeft);

        }

        //菜单处理：清空图形
        public override void ClearPlot(IPlotControl control)
        {
            //clear legend
            Legends.Clear();
            MFormsPlot.Plot.ShowLegend(Legends);

            //clear plot
            MFormsPlot.Plot.Clear();
            var yAxises = MFormsPlot.Plot.Axes.GetAxes(Edge.Left).ToList();
            for (int i = 1; i < yAxises.Count(); i++)
            {
                MFormsPlot.Plot.Axes.Remove(yAxises[i]);
            }
            Axis_01 = null;

            Axises.Clear();
            StreamerArray.Clear();

            //display original plot
            AddStreamer();

            MFormsPlot.Refresh();


        }


        //
    }
}
