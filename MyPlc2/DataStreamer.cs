using NodaTime;
using ScottPlot;
using ScottPlot.AxisLimitCalculators;
using ScottPlot.DataSources;
using ScottPlot.Plottables;
using SkiaSharp;
using System.IO;

namespace MyPlc2;

public class DataStreamer : IPlottable, IManagesAxisLimits
{
    public bool IsVisible { get; set; } = true;
    public IAxes Axes { get; set; } = ScottPlot.Axes.Default;
    public IEnumerable<LegendItem> LegendItems => LegendItem.None;

    public readonly LineStyle LineStyle = new();
    public ScottPlot.Color Color { get => LineStyle.Color; set => LineStyle.Color = value; }

    public DataStreamerSource Data { get; set; }

    public ConnectStyle ConnectStyle = ConnectStyle.Straight;

    public double Period { get => Data.SamplePeriod; set => Data.SamplePeriod = value; }

    /// <summary>
    /// Returns true if data has been added since the last render
    /// </summary>
    public bool HasNewData => Data.CountTotal != Data.CountTotalOnLastRender;

    /// <summary>
    /// If enabled, axis limits will be adjusted automatically if new data runs off the screen.
    /// </summary>
    public bool ManageAxisLimits { get; set; } = true;

    /// <summary>
    /// Contains logic for automatically adjusting axis limits if new data runs off the screen.
    /// Only used if <see cref="ManageAxisLimits"/> is true.
    /// </summary>
    private IAxisLimitManager AxisManager { get; set; } = new FixedWidth();

    /// <summary>
    /// Used to obtain the current axis limits so <see cref="AxisManager"/> can adjust them if needed.
    /// </summary>
    private readonly Plot Plot;

    /// <summary>
    /// Logic for displaying the fixed-length Y values in <see cref="Data"/>
    /// </summary>
    public ScottPlot.DataViews.IDataStreamerView Renderer { get; set; }

    public DataStreamer(Plot plot, double[] data)
    {
        Plot = plot;
        Data = new DataStreamerSource(data);
    }

    /// <summary>
    /// Shift in a new Y value
    /// </summary>
    public void Add(double value)
    {
        Data.Add(value);
    }

    /// <summary>
    /// Shift in a collection of new Y values
    /// </summary>
    public void AddRange(IEnumerable<double> values)
    {
        Data.AddRange(values);
    }

    /// <summary>
    /// Clear the buffer by setting all Y points to the given value
    /// </summary>
    public void Clear(double value = 0)
    {
        Data.Clear(value);
    }

    /*
    // TODO: slide axes
    /// <summary>
    /// Display the data using a view that continuously shifts data to the left, 
    /// placing the newest data on the right, and sliding the horizontal axis
    /// to track the latest data coming in.
    /// </summary>
    public void ViewSlideLeft()
    {
        Renderer = new DataViews.Scroll(this, true);
    }
    */

    /// <summary>
    /// Display the data using a custom rendering function
    /// </summary>
    public void ViewCustom(ScottPlot.DataViews.IDataStreamerView view)
    {
        Renderer = view;
    }

    public AxisLimits GetAxisLimits()
    {
        return Data.GetAxisLimits();
    }

    public void UpdateAxisLimits(Plot plot, bool force = false)
    {
        AxisLimits limits = Plot.Axes.GetLimits(Axes);
        AxisLimits dataLimits = Data.GetAxisLimits();
        AxisLimits newLimits = AxisManager.GetAxisLimits(limits, dataLimits);
        Plot.Axes.SetLimits(newLimits);
    }

    public void Render(RenderPack rp)
    {
        Pixel[] points = new Pixel[Data.Length];
        int oldPointCount = Data.Length - Data.NextIndex;

        if (Data.Length == 0) return;

        for (int i = 0; i < Data.Length; i++)
        {
            bool isNewPoint = i < oldPointCount;
            int sourceIndex = isNewPoint ? Data.NextIndex + i : i - oldPointCount;
            int targetIndex = i;
            points[targetIndex] = new(
                x: Axes.GetPixelX(targetIndex * Data.SamplePeriod + Data.OffsetX),
                y: Axes.GetPixelY(Data.Data[sourceIndex] + Data.OffsetY));
        }

        IEnumerable<Pixel> linePixels = ConnectStyle switch
        {
            ConnectStyle.Straight => points,
            ConnectStyle.StepHorizontal => Scatter.GetStepDisplayPixels(points, true),
            ConnectStyle.StepVertical => Scatter.GetStepDisplayPixels(points, false),
            _ => throw new NotImplementedException(),
        };

        if (linePixels.Count() > 0)
        {
            using SKPaint paint = new();
            Drawing.DrawLines(rp.Canvas, paint, linePixels, LineStyle);
        }

        //
        //Data.CountTotalOnLastRender = Data.CountTotal;
    }

    public void SetupAxisX(double width)
    {
        double viewWidth = width; // this depends on how fast your data is coming in
        //Period = viewWidth / Data.Length;
        Period = 0.01;
        //Data.OffsetX = DateTime.Now.ToOADate() - viewWidth;

    }

    //
}
