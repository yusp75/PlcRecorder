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
        public string Address { get; set; }
        public string X { get; set; }
        public string Y { get; set; }

        public NearestValue(string address, string x, string y)
        {
            Address = address;
            X = x;
            Y = y;
        }
    }
    public class NearestValueEventArgs : EventArgs
    {
        public List<NearestValue> Values { get; set; }

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
        private VerticalLine MCrosshair;
        private AxisLine? LineBeingDragged = null;
        private int PrevMouseX;

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
            {
                throw new InvalidOperationException($"{address}：点集空，不能添加图形");
            }

            SetTitle();
            var plot = MFormsPlot.Plot;
            //legend
            plot.Legend.Location = Alignment.UpperLeft;
            plot.Legend.IsVisible = true;
            //图形
            var x_array = Points.x.ToArray();
            var subPlot = plot.Add.Scatter(x_array, Points.y.ToArray());
            subPlot.Label = address;

            //垂直尺
            MCrosshair = plot.Add.VerticalLine(x_array[0].ToOADate(), color: ScottPlot.Color.FromHex("#FF0000"), width: 1);
            MCrosshair.IsDraggable = true;
            
            //自动缩放Axis
            plot.Axes.AutoScaleX();

            //鼠标：按下
            MFormsPlot.MouseDown += (s, e) =>
            {
                var lineUnderMouse = GetLineUnderMouse(e.X, e.Y);
                if (lineUnderMouse != null)
                {
                    LineBeingDragged = lineUnderMouse;
                    MFormsPlot.Interaction.Disable();
                }
                else
                {
                    PrevMouseX = e.X;
                }

            };
            //鼠标：松开
            MFormsPlot.MouseUp += (s, e) =>
            {
                if (LineBeingDragged != null)
                {
                    var x = ((VerticalLine)LineBeingDragged).X * MFormsPlot.Plot.ScaleFactor;

                    //事件传值给gridview
                    //var y = 0.0;
                    List<NearestValue> values = new();
                    foreach (var p in PlotArray)
                    {
                        var a = p.Value.Data.GetScatterPoints().ToList();
                        var y = FindNearestValue(a, x);
                        var dt_x = DateTime.FromOADate(x).ToString();
                        values.Add(new NearestValue(p.Key, dt_x, y.ToString()));
                    }

                    OnRaiseNearestValueEvent(values);
                    //释放
                    LineBeingDragged = null;
                }
                else
                {
                    //

                    MCrosshair.X += e.X - PrevMouseX;
                    MFormsPlot.Refresh();

                }


                MFormsPlot.Interaction.Enable();

            };
            //鼠标：移动
            MFormsPlot.MouseMove += (s, e) =>
            {
                CoordinateRect rect = MFormsPlot.Plot.GetCoordinateRect(e.X, e.Y, radius: 10);
                if (LineBeingDragged is null)
                {

                    var lineUnderMouse = GetLineUnderMouse(e.X, e.Y);
                    if (lineUnderMouse is null) MFormsPlot.Cursor = Cursors.Default;
                    else if (lineUnderMouse.IsDraggable && lineUnderMouse is VerticalLine) MFormsPlot.Cursor = Cursors.SizeWE;

                }
                else
                {
                    // update the position of the plottable being dragged
                    if (LineBeingDragged is VerticalLine vl)
                    {
                        vl.X = rect.HorizontalCenter;
                        //vl.Text = $"{vl.X:0.00}";
                    }

                    MFormsPlot.Refresh();
                }
            };

            //缩放，平移
            MFormsPlot.MouseWheel += (s, e) =>
            {
                Debug.WriteLine("scroll");
            };

            // 计算图形index
            int idx = CalIndex(address, Points);
            var axis = (LeftAxis)plot.Axes.Left;
            //AddToAxises(idx);
            SetupPlot(idx, ref subPlot, ref axis);

            //有数据时刷新
            if (Points.x.Count > 0) Refresh();

            //PlotArray.Add(MRecord.Address, subPlot);
            PlotArray[MRecord.Address] = subPlot;
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

        //判断AxisLine在鼠标下方
        private AxisLine? GetLineUnderMouse(float x, float y)
        {
            var plot = MFormsPlot.Plot;
            CoordinateRect rect = plot.GetCoordinateRect(x, y, radius: 10);
            foreach (AxisLine axLine in plot.GetPlottables<AxisLine>().Reverse())
            {
                if (axLine.IsUnderMouse(rect))
                    return axLine;
            }

            return null;
        }

        //二分法：在a中找x
        private int bisect_left(List<Coordinates> a, double x, int lo = 0, int hi = 0)
        {
            if (hi == 0) hi = a.Count();

            while (lo < hi)
            {
                int mid = (lo + hi);
                if (a[mid].X < x)
                    lo = mid + 1;
                else
                    hi = mid;
            }

            return lo;
        }

        private double FindNearestValue(List<Coordinates> a, double x, int lo = 0, int hi = 0)
        {
            //检查边界
            if (hi == 0)
                hi = a.Count() - 1;
            if (x >= a[hi].X)
                return a[hi].Y;
            else if (x <= a[0].X)
                return a[0].Y;

            //二分法查找
            int pos = bisect_left(a, x, lo, hi);
            double x1 = a[pos - 1].X;
            double x2 = a[pos].X;

            if (x2 - x < x - x1)
                return a[pos].Y;
            else
                return a[pos - 1].Y;

        }

        //
    }

}
