using ScottPlot.Plottables;
using ScottPlot;
using MyPlc2;
using System.Diagnostics;
using ScottPlot.Legends;
using ScottPlot.AxisPanels;

namespace Historical
{
    public class NearestValue
    {
        private string Address { get; set; }
        private string X { get; set; }
        private string Y { get; set; }

        public NearestValue(string address, string x, string y)
        {
            Address = address;
            X = x;
            Y = y;
        }
    }
    public class NearestValueEventArgs : EventArgs
    {
        private List<NearestValue> Values { get; set; }

        public NearestValueEventArgs(List<NearestValue> values)
        {
            Values = values;
        }
    }

    internal class Vc_h : Vc
    {
        //多画面队列 添加/移除
        private readonly Dictionary<string, Scatter> PlotArray = new();
        //订阅
        private string _id;
        //在table布局行数
        public int Position { get; set; }
        private Crosshair MCrosshair;

        public event EventHandler<NearestValueEventArgs> NearestValueEvent;

        public Vc_h(Record record) : base(record)
        {

        }
        private void OnRaiseNearestValueEvent(List<NearestValue> values)
        {
            NearestValueEvent?.Invoke(this, new NearestValueEventArgs(values));
        }
        //双击：增加plot 1
        public void AddPlot()
        {
            var address = MRecord.Address;
            if (Points.x.Count == 0)
                throw new InvalidOperationException($"{address}：点集空，不能添加图形");

            SetTitle();
            var plot = MFormsPlot.Plot;
            plot.Legend.Location = Alignment.UpperLeft;
            plot.Legend.IsVisible = true;
            //legend
            var subPlot = plot.Add.Scatter(Points.x.ToArray(), Points.y.ToArray());
            subPlot.Label = address;
            MCrosshair = plot.Add.Crosshair(0, 0);
            MCrosshair.HorizontalLineIsVisible = false;
            
            MFormsPlot.MouseDown += (s, e) =>
            {
                Pixel mousePixel = new(e.X, e.Y);
                Coordinates mouseCoordinates = MFormsPlot.Plot.GetCoordinates(mousePixel);
                MCrosshair.Position = new Coordinates(mouseCoordinates.X, 0);

                //事件传值给gridview
                List<NearestValue> values = new();
                DataPoint nearest = subPlot.Data.GetNearest(mouseCoordinates, plot.LastRender);

                values.Add(new NearestValue(MRecord.Address, nearest.X.ToString(), nearest.Y.ToString()));
                OnRaiseNearestValueEvent(values);
            };

            // 计算图形index
            int idx = CalIndex(address, Points);
            var axis = (LeftAxis)plot.Axes.Left;
            //AddToAxises(idx);
            SetupPlot(idx, ref subPlot, ref axis);

            //有数据时刷新
            if (Points.x.Count > 0) Refresh();

            PlotArray.Add(MRecord.Address, subPlot);
        }

        //更新数据，刷新
        public void UpdateData(string address, MPoint points)
        {
            if (MFormsPlot == null) return;

            if (MRecord.Address == address)
            {
                Points = points;
                if (PlotArray.ContainsKey(address))
                {
                    Scatter plot = PlotArray[address];
                    plot = MFormsPlot.Plot.Add.Scatter(points.x.ToArray(), points.y.ToArray());

                    //设置X轴limit
                    //plot.AxisAuto();

                    if (points.x.Count > 0)
                    {
                        Refresh();
                    }
                } //if contain
            } //if equal
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

            var plot = MFormsPlot.Plot;
            plot.Legend.Location = Alignment.UpperLeft;
            plot.Legend.IsVisible = true;
            var subPlot = plot.Add.Scatter(points.x.ToArray(), points.y.ToArray());
            //legend
            subPlot.Label = address;
            //分配轴
            var axis = WhichAxis(address, points);
            subPlot.Axes.YAxis = axis;

            //0-1图形？
            int idx = CalIndex(address, Points);
            SetupPlot(idx, ref subPlot, ref axis);

            //有数据时刷新
            if (points.x.Count > 0) Refresh();

            PlotArray.Add(address, subPlot);
        }

        public override int Read()
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

        //菜单：图形清除
        public override void ClearPlot(IPlotControl control)
        {
            //clear legend
            var a = MFormsPlot.Plot.Legend;

            //clear plot
            MFormsPlot.Plot.Clear();
            var yAxises = MFormsPlot.Plot.Axes.GetAxes(Edge.Left).ToList();
            for (int i = 1; i < yAxises.Count(); i++)
            {
                MFormsPlot.Plot.Axes.Remove(yAxises[i]);
            }

            Axises.Clear();
            PlotArray.Clear();

            //display original plot
            AddPlot();

            MFormsPlot.Refresh();

        }
        //
    }


}
