using Sharp7;
using System.Diagnostics;
using ScottPlot.Plottable;
using ScottPlot.Renderable;
using InfluxDB.Client.Api.Domain;
using ScottPlot;
using System.IO;
using System.Net;


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
    //变量：解析，采集
    internal class Vc_d : Vc
    {
        public S7Client? Client;

        public string Address { get; set; }
        public double Value { get; set; }

        //画面队列
        private readonly Dictionary<string, DataStreamer> StreamerArray = new();
        private AxisLimits AxisLitmits;
        private ScottPlot.Renderable.Axis Axis_01;

        //订阅
        private string _id;

        public Vc_d(S7Client? client, Record record) : base(record)
        {
            Client = client;
        }

        //双击：加streamer 1
        public void AddStreamer()
        {
            //if (StreamerArray.Keys.Contains(MRecord.Address)) return; //检查：空，重复

            SetTitle();
            var plt = MFormsPlot.Plot;

            //类型：DataStreamer
            var streamer = plt.AddDataStreamer(length: 50);
            streamer.ViewScrollLeft();
            streamer.Label = MRecord.Address;
            //赋轴值给新加图形
            streamer.YAxisIndex = plt.YAxis.AxisIndex;

            //采样周期
            streamer.SamplePeriod = 0.01;
            plt.YAxis.Color(streamer.Color);

            //布尔值，则绘制stepline
            if (Check01(MRecord.Address))
            {
                streamer.Renderer.StepDisplay = true;
                plt.SetAxisLimitsY(0, 50, 0);
                Axis_01 = plt.YAxis;

                Axis_01_Count++;
            }
            else
            {
                plt.SetAxisLimitsY(0, 100, 0);
                Axis_lg_Count++;
            }

            StreamerArray[MRecord.Address] = streamer;
            MFormsPlot.Refresh();
        }

        //拖放：加streamer 2
        public void AddStreamer(string address)
        {
            if (MFormsPlot == null || StreamerArray.Keys.Contains(address)) return; //检查：空，重复

            const int BASE_LINE = 50;
            var plt = MFormsPlot.Plot;

            var streamer = plt.AddDataStreamer(length: 50);
            streamer.ViewScrollLeft();
            streamer.Label = address;
            streamer.SamplePeriod = 0.01;

            if (Check01(address))
            {
                if (Axis_01 == null)
                {
                    Axis_01 = plt.AddAxis(Edge.Left);
                    Axis_01.Color(streamer.Color);
                    plt.SetAxisLimitsY(0, 50, Axis_01.AxisIndex);
                }

                streamer.Renderer.StepDisplay = true;
                streamer.OffsetY = 2 * Axis_01_Count;
                streamer.YAxisIndex = Axis_01.AxisIndex;

                Axis_01_Count++;
            }
            else
            {
                var axis = plt.AddAxis(Edge.Left);
                axis.Color(streamer.Color);
                streamer.YAxisIndex = axis.AxisIndex;
                streamer.OffsetY = 10 * Axis_lg_Count + BASE_LINE;
                plt.SetAxisLimitsY(0, 100, axis.AxisIndex);

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

                //向事件传递值
                this.Address = r.Address;
                this.Value = value;

                //需空检查
                if (StreamerArray.ContainsKey(r.Address))
                {
                    DataStreamer streamer = StreamerArray[r.Address];
                    //增加y值
                    streamer?.Add(value);
                }
                if (r.Address == "q0.0")
                {
                    Debug.WriteLine("Vc.Read:{0},{1},{2}", r.Address, value, Value);
                }

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
        public void HandleReadEvent(object sender, ReadEventArgs e)
        {
            string address = e.Address;
            double value = e.Value;

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
            //更新
            Refresh();
        }

        //
    }
}
