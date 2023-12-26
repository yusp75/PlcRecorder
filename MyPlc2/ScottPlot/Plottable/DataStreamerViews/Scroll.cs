using System.Drawing;

namespace ScottPlot.Plottable.DataStreamerViews;

internal class Scroll : IDataStreamerView
{
    private readonly bool NewOnRight;

    public DataStreamer Streamer { get; }
    public bool StepDisplay { get; set; }

    public Scroll(DataStreamer streamer, bool newOnRight)
    {
        Streamer = streamer;
        NewOnRight = newOnRight;
    }

    public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
    {
        PointF[] points = new PointF[Streamer.Data.Length];

        int oldPointCount = Streamer.Data.Length - Streamer.NextIndex;

        for (int i = 0; i < Streamer.Data.Length; i++)
        {
            bool isNewPoint = i < oldPointCount;
            int sourceIndex = isNewPoint ? Streamer.NextIndex + i : i - oldPointCount;
            int targetIndex = NewOnRight ? i : Streamer.Data.Length - 1 - i;
            points[targetIndex] = new(
                x: dims.GetPixelX(targetIndex * Streamer.SamplePeriod + Streamer.OffsetX),
                y: dims.GetPixelY(Streamer.Data[sourceIndex] + Streamer.OffsetY));
        }

        using var gfx = ScottPlot.Drawing.GDI.Graphics(bmp, dims, lowQuality);
        using var pen = ScottPlot.Drawing.GDI.Pen(Streamer.Color, Streamer.LineWidth, LineStyle.Solid);

        //if (points.Length > 1)
        //    gfx.DrawLines(pen, points);
        //step line 2023.11.21
        if (points.Length > 1)
        {
            if (StepDisplay)
            {
                PointF[] pointsStep = GetStepDisplayPoints(points, true);
                gfx.DrawLines(pen, pointsStep);
            }
            else
            {
                gfx.DrawLines(pen, points);
            }
        }
    }

    public static PointF[] GetStepDisplayPoints(PointF[] points, bool right)
    {
        PointF[] pointsStep = new PointF[points.Length * 2 - 1];

        int offsetX = right ? 1 : 0;
        int offsetY = right ? 0 : 1;

        for (int i = 0; i < points.Length - 1; i++)
        {
            pointsStep[i * 2] = points[i];
            pointsStep[i * 2 + 1] = new PointF(points[i + offsetX].X, points[i + offsetY].Y);
        }

        pointsStep[pointsStep.Length - 1] = points[points.Length - 1];

        return pointsStep;
    }



}
