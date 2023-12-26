using ScottPlot.Plottable;
using ScottPlot;
using MyPlc2;
using System.Diagnostics;

namespace Historical
{

    //变量：解析，采集
    internal class Vc_h : Vc
    {
        //多画面队列 添加/移除
        private readonly Dictionary<string, ScatterPlot> PlotArray = new();
        //订阅
        private string _id;

        public Vc_h(Record record) : base(record)
        {

        }

        //双击：增加plot 1
        public void AddPlot()
        {
            var address = MRecord.Address;
            if (Points.x.Count == 0)
                throw new InvalidOperationException($"{address}：点集空，不能添加图形");

            SetTitle();
            var scatterPlot = MFormsPlot.Plot.AddScatter(Points.x.ToArray(), Points.y.ToArray(), label: address);
            // 计算图形index
            int idx = CalIndex(address, Points);
            AddToAxises(idx);
            SetupPlot(idx, ref scatterPlot);            
 
            Debug.WriteLine($"Axis_01_Count:{Axis_01_Count}, Axis_lg_Count:{Axis_lg_Count}");

            //刷新图形
            if (Points.x.Count > 0) Refresh();

            PlotArray.Add(MRecord.Address, scatterPlot);
        }

        //更新数据，刷新
        public void UpdateData(string address, MPoint points)
        {
            if (MFormsPlot == null) return;

            Points = points;

            if (!PlotArray.ContainsKey(address)) { return; }

            ScatterPlot plt = PlotArray[address];
            plt.Update(points.x.ToArray(), points.y.ToArray());

            //设置X轴limit
            var xs = points.x[0];
            var xe = points.x[points.x.Count - 1];
            //plt.AxisAuto();

            if (points.x.Count > 0)
            {
                Refresh();
            }
        }

        //拖曳：增加plot 2
        public void AddPlot(string address, MPoint points)
        {
            if (MFormsPlot == null) return;

            //避免重复plot
            if (PlotArray.ContainsKey(address))
            {
                Debug.WriteLine($"{address}重复");
                return;
            }

            if (points.x.Count == 0) { return; }

            var scatterPlot = MFormsPlot.Plot.AddScatter(points.x.ToArray(), points.y.ToArray(), label: address);

            //添加轴
            int axis_index = WhichAxis(address, points);
            scatterPlot.YAxisIndex = axis_index;            

            //0-1图形？
            int idx = CalIndex(address, Points);
            SetupPlot(idx, ref scatterPlot);

            //有数据时刷新
            if (points.x.Count > 0)
            {
                Refresh();
            }
            PlotArray.Add(address, scatterPlot);
        }

        public override void Read()
        {
            throw new NotImplementedException();
        }

        //事件：订阅
        public void Subscriber(string id, History pub)
        {
            _id = id;

            //Subscribe to the event
            pub.UpdateDataEvent += HandleUpdateDataEvent;
        }

        //事件：处理
        public void HandleUpdateDataEvent(object? sender, UpdateDataEventArgs e)
        {
            UpdateData(e.Address, e.Points);
        }
        //
    }


}
