using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Sharp7;
using System.Diagnostics;
using ScottPlot;
using ScottPlot.WinForms;
using ScottPlot.AxisPanels;
using ScottPlot.Plottables;
using static Python.Runtime.TypeSpec;
using ScottPlot.Colormaps;
using ScottPlot.Control;

namespace MyPlc2
{
    public class ReadEventArgs : EventArgs
    {
        public ReadEventArgs(string address, double value)
        {
            Address = address;
            Value = value;
        }

        public string Address { get; set; }
        public double Value { get; set; }

    }
    public class UpdateDataEventArgs : EventArgs
    {
        public UpdateDataEventArgs(string address, MPoint points)
        {
            Address = address;
            Points = points;
        }

        public string Address { get; set; }
        public MPoint Points { get; set; }

    }
    public class Record
    {
        public Record(string name, string address, int area, int db_number, int start, string data_type, string cycle, int word_len, int bit_pos)
        {
            Name = name;
            Address = address;
            Area = area;
            Db_number = db_number;
            Start = start;
            Data_type = data_type;
            Cycle = cycle;
            WordLen = word_len;
            Bit_pos = bit_pos;
        }

        public string Name { get; set; }
        public string Address { get; set; }
        public int Db_number { get; }
        public int Start { get; }
        public string Data_type { get; set; }
        public int WordLen { get; }
        public int Bit_pos { get; }
        public int Area { get; }
        public string Cycle { get; }
    }

    public class MPoint
    {
        public List<DateTime> x { get; } = new();
        public List<double> y { get; } = new();

        public MPoint()
        {
            //x.Add(DateTime.Now.ToOADate());
            //y.Add(0.0);
        }

        public void AddPoint(DateTime time, double value)
        {
            this.x.Add(time);
            this.y.Add(value);
        }
    }

    public class MAxis
    {
        public int Count { get; set; }
        public LeftAxis MyLeftAxis { get; set; }

        public MAxis()
        {
        }

        public MAxis(int count, LeftAxis myLeftAxis)
        {
            this.Count = count;
            this.MyLeftAxis = myLeftAxis;
        }

    }
    //变量：解析，采集
    public abstract class Vc
    {
        public Record MRecord;
        public bool Enable { get; set; } //使能
        public bool Drawed { get; set; } //绘画
        public FormsPlot MFormsPlot { get; set; }
        public MPoint Points = new();
        //多轴
        public Dictionary<int, MAxis> Axises = new();
        public int Axis_lg_Count { get; set; } = 0; //值>100
        public int Axis_01_Count { get; set; } = 0; //值0-1

        //抽象类
        public abstract int Read();
        public enum Areas
        {
            PE = 0x81,
            PA = 0x82,
            MK = 0x83,
            DB = 0x84,
            CT = 0x1C,
            TM = 0x1D,
        };


        //委托：拖曳
        public delegate void ItemDropped(string address, string parent);
        public ItemDropped itemDropped;
        //委托：删除图形
        public delegate void DeletePlotDelegate(string address);
        public DeletePlotDelegate delePlot;

        public Vc()
        {
            MFormsPlot = new();
        }

        public Vc(Record record)
        {

            MRecord = record;

            Enable = true;

            //生成一个Plot
            MFormsPlot = new()
            {
                //拖放
                AllowDrop = true
            };

            MFormsPlot.DragEnter += MFormsPlot_DragEnter;
            MFormsPlot.DragOver += MFormsPlot_DragOver;
            MFormsPlot.DragDrop += MFormsPlot_DragDrop;

            //鼠标右键菜单
            var menu = (FormsPlotMenu)MFormsPlot.Menu;            
            menu.Clear();

            menu.Add("适应", FitPlot);
            //menu.Add("重置", ResetPlot);
            menu.Add("清空图形", ClearPlot);
            menu.Add("删除图形", DeletePlot);

        }

        //菜单：重置图形
        private void ResetPlot(IPlotControl control)
        {
            MFormsPlot.Reset();
            Refresh();
        }

        //菜单：适应图形
        private void FitPlot(IPlotControl control)
        {
            MFormsPlot.Plot.Axes.AutoScale(); 
            Refresh();
        }

        //菜单：删除图形
        private void DeletePlot(IPlotControl control)
        {
            delePlot(MRecord.Address);
        }

        public abstract void ClearPlot(IPlotControl control);

        //菜单：清空图形
        private void FitToPlot(object? sender, EventArgs e)
        {
            MFormsPlot.Plot.Clear();
        }

        private void MFormsPlot_DragOver(object? sender, DragEventArgs e)
        {

        }

        private void MFormsPlot_DragDrop(object? sender, DragEventArgs e)
        {
            MFormsPlot.DoDragDrop(this, DragDropEffects.None);
        }

        //拖放进入
        private void MFormsPlot_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string address = e.Data.GetData(DataFormats.Text).ToString();
                if (address != null)
                {
                    itemDropped(address, MRecord.Address);
                }

            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        public void SetTitle()
        {
            //Plot标题
            MFormsPlot.Plot.Title(MRecord.Name + "-" + MRecord.Address);
        }

        public static List<Record> Parse()
        {
            Dictionary<string, string> patterns = new() {
            { "dbx","^db([0-9]+)\\.dbx([0-9]+\\.[0-9]+)" },
            { "dbb","^db([0-9]+)\\.dbb([0-9]+)"},
            { "dbw","^db([0-9]+)\\.dbw([0-9]+)"},
            { "dbd","^db([0-9]+)\\.dbd([0-9]+)"},
            { "m","^m([0-9]+\\.[0-9]+)"},
            { "i","^i([0-9]+\\.[0-9]+)"},
            { "q","^q([0-9]+\\.[0-9]+)"}
        };

            List<Record> records = new List<Record>();

            string config_path = AppDomain.CurrentDomain.BaseDirectory + "\\config\\";
            //读json文件
            if (!File.Exists(config_path + "vars.json"))
            {
                Debug.WriteLine("Vc.Parse, 无PLC变量");
                return records;
            }
            using StreamReader reader = new(config_path + "vars.json");
            string s = reader.ReadToEnd();
            if (!string.IsNullOrEmpty(s))
            {
                List<PlcVar> vars = JsonConvert.DeserializeObject<List<PlcVar>>(s);
                if (vars is not null && vars.Count > 0)
                {
                    //翻译
                    foreach (PlcVar var in vars)
                    {
                        if (var is null) continue;

                        int area = (int)Areas.DB;
                        int db_number = 0;
                        int start = 0;
                        int size = S7Consts.S7WLByte;
                        int bit_pos = -1;
                        bool is_db = false;
                        string name = var.name;
                        string pattern = "";

                        string addr = var.address.Trim().ToLower(); //去空格，改小写
                        string[] addr_s = addr.Split('.');
                        if (addr_s[0].StartsWith("db"))
                        {
                            if (addr_s[1].StartsWith("dbx"))
                            {
                                pattern = patterns["dbx"];
                                is_db = true;
                            }
                            else if (addr_s[1].StartsWith("dbb"))
                            {
                                pattern = patterns["dbb"];
                                is_db = true;
                            }
                            else if (addr_s[1].StartsWith("dbw"))
                            {
                                pattern = patterns["dbw"];
                                size = S7Consts.S7WLInt;
                                is_db = true;
                            }
                            else if (addr_s[1].StartsWith("dbd"))
                            {
                                pattern = patterns["dbd"];
                                size = S7Consts.S7WLReal;
                                is_db = true;
                            }
                        }
                        else if (addr_s[0].StartsWith("m"))
                        {
                            pattern = patterns["m"];
                            area = (int)Areas.MK;
                        }
                        else if (addr_s[0].StartsWith("i"))
                        {
                            pattern = patterns["i"];
                            area = (int)Areas.PE;
                        }
                        else if (addr_s[0].StartsWith("q"))
                        {
                            pattern = patterns["q"];
                            area = (int)Areas.PA;
                        }
                        else
                        {
                            Debug.WriteLine("Vc.Parse：不支持的格式 %s，" + addr);
                            continue;
                        }

                        Regex regex = new Regex(pattern);
                        Match m = regex.Match(addr);
                        if (m.Success)
                        {
                            //分割DB
                            if (is_db)
                            {
                                string db_str = m.Groups[1].Value;
                                db_number = Convert.ToInt16(m.Groups[1].Value);
                                string start_str = m.Groups[2].Value;
                                if (start_str.Contains('.'))
                                {
                                    string[] str = start_str.Split(".");
                                    if (str.Length > 1)
                                    {
                                        start = Convert.ToInt16(str[0]);
                                        bit_pos = Convert.ToInt16(str[1]);
                                    }
                                }

                            }
                            else
                            {
                                //分割I，Q，M
                                string start_str = m.Groups[1].Value;
                                if (start_str.Contains('.'))
                                {
                                    string[] str = start_str.Split(".");
                                    if (str.Length > 1)
                                    {
                                        start = Convert.ToInt16(str[0]);
                                        bit_pos = Convert.ToInt16(str[1]);
                                    }
                                }
                            }

                        }
                        else
                        {
                            Debug.WriteLine("Vc.Parse：格式错误，" + addr);
                            continue;
                        }

                        string[] cycles = new string[] { "10ms", "20ms", "50ms", "100ms", "1s" };
                        if (Array.IndexOf(cycles, var.cycle) == -1)
                        {
                            var.cycle = "100ms";
                        }

                        records.Add(new Record(
                            name,
                            var.address,
                            area,
                            db_number,
                            start,
                            var.type,
                            var.cycle,
                            size,
                            bit_pos
                            ));
                    } //foreach
                }
            }

            return records;
        } //Parse             

        public void Refresh()
        {
            if (MFormsPlot is null) return;
            if (MFormsPlot.Plot is null) return;

            if (MFormsPlot.InvokeRequired)
            {
                Action action = delegate
                {
                    MFormsPlot.Refresh();
                };
                MFormsPlot.Invoke(action);
            }
            else
            {
                MFormsPlot.Refresh();

            }
        }

        #region 1
        // 字节数组转换为字符串
        protected static string ByteArrayToString(Byte[] array)
        {
            string result = "";
            for (int i = 0; i < array.Length; i++)
            {
                result += "\0x" + array[i].ToString("X");
            }
            return result;
        }

        public MPoint GetPoints()
        {
            return Points;
        }

        // 字节数组计算值
        protected static double CalByteArray(Byte[] data, int word_len, int pos)
        {
            double result = 0.0f;

            if (pos >= 0)
            {
                int value = Convert.ToUInt16(S7.GetByteAt(data, 0));
                result = Convert.ToDouble((value >> pos) & 0x01);
            }
            else
            {
                switch (word_len)
                {
                    case S7Consts.S7WLByte:
                        result = Convert.ToDouble(S7.GetByteAt(data, 0));
                        break;
                    case S7Consts.S7WLInt:
                        result = Convert.ToDouble(S7.GetIntAt(data, 0));
                        break;
                    case S7Consts.S7WLReal:
                        result = S7.GetRealAt(data, 0);
                        break;
                }
            }

            return result;
        }

        //是数字量？
        public bool Check01(string address)
        {
            address = address.ToLower();
            return address.Contains('i') || address.Contains('q') || address.Contains('m') || address.Contains("dbx");
        }

        public LeftAxis WhichAxis(string address, MPoint points)
        {
            int LimitZeroOne = 50;
            int index = CalIndex(address, points);

            if (!Axises.ContainsKey(index))
            {
                var plot = MFormsPlot.Plot;

                MAxis mAxis = new();
                LeftAxis axis = plot.Axes.AddLeftAxis();

                //设置轴极限                
                if (index == 1)
                {
                    plot.Axes.SetLimitsY(0, LimitZeroOne, axis);
                }
                else
                {
                    plot.Axes.SetLimitsY(-1 * index, index, axis);
                }

                mAxis.MyLeftAxis = axis;
                Axises.Add(index, mAxis);
            }

            return Axises[index].MyLeftAxis;
        }
        //根据点计算轴极限
        public int CalIndex(string address, MPoint points)
        {
            int index;
            double[] ys = points.y.ToArray();

            if (Check01(address))
            {
                index = 1;
            }
            else
            {
                double max = Math.Abs(ys.Max());
                if (max < 100.0f)
                {
                    index = 100;
                }
                else if (max < 1000)
                {
                    index = 1000;
                }
                else
                {
                    index = 1000;
                }
            }

            return index;

        }

        //设置图形
        public void SetupPlot(int idx, ref Scatter subPlot, int space = 3)
        {
            int base_line = 50;

            var my_axis = Axises[idx].MyLeftAxis;
            var plot = MFormsPlot.Plot;

            //plt.AddCrosshair(30, 0.36);
            //图形与轴颜色
            if (Axises.ContainsKey(idx))
            {
                Axises[idx].MyLeftAxis.Color(subPlot.Color);
            }

            if (idx == 1)
            {
                plot.Axes.SetLimitsY(0, 50, my_axis);
                //Y轴禁用zoom

                subPlot.ConnectStyle = ConnectStyle.StepHorizontal;
                subPlot.MarkerStyle = MarkerStyle.None;

                Axis_01_Count++;

                //subPlot.Data.YOffset = space * Axis_01_Count;
            }
            else
            {
                Axis_lg_Count++;

                //subPlot.Data.YOffset = space * Axis_lg_Count + base_line;
                plot.Axes.SetLimitsY(-1 * idx, idx, my_axis);
            }
        }

        public void AddToAxises(int key)
        {
            Axises.Add(key, new MAxis(0, MFormsPlot.Plot.Axes.AddLeftAxis()));
        }

        public static Pixel[] GetStepDisplayPoints(Pixel[] points, bool right)
        {
            Pixel[] pointsStep = new Pixel[points.Length * 2 - 1];

            int offsetX = right ? 1 : 0;
            int offsetY = right ? 0 : 1;

            for (int i = 0; i < points.Length - 1; i++)
            {
                pointsStep[i * 2] = points[i];
                pointsStep[i * 2 + 1] = new Pixel(points[i + offsetX].X, points[i + offsetY].Y);
            }

            pointsStep[pointsStep.Length - 1] = points[points.Length - 1];

            return pointsStep;
        }

        //

        #endregion

    }
}
