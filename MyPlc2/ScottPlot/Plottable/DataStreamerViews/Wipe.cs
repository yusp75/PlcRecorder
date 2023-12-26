using System.Drawing;

namespace ScottPlot.Plottable.DataStreamerViews;

internal class Wipe : IDataStreamerView
{
    private readonly bool WipeRight;

    public DataStreamer Streamer { get; }
    public bool StepDisplay { get; set; }

    //TODO: Add a BlankFraction property that adds a gap between old and new data

    public Wipe(DataStreamer streamer, bool wipeRight)
    {
        Streamer = streamer;
        WipeRight = wipeRight;
    }

    public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
    {
        int newestCount = Streamer.NextIndex;
        int oldestCount = Streamer.Data.Length - newestCount;

        double xMax = Streamer.Data.Length * Streamer.SamplePeriod + Streamer.OffsetX;

        PointF[] newest = new PointF[newestCount];
        PointF[] oldest = new PointF[oldestCount];

        for (int i = 0; i < newest.Length; i++)
        {
            double xPos = i * Streamer.SamplePeriod + Streamer.OffsetX;
            float x = dims.GetPixelX(WipeRight ? xPos : xMax - xPos);
            float y = dims.GetPixelY(Streamer.Data[i] + Streamer.OffsetY);
            newest[i] = new(x, y);
        }

        for (int i = 0; i < oldest.Length; i++)
        {
            double xPos = (i + Streamer.NextIndex) * Streamer.SamplePeriod + Streamer.OffsetX;
            float x = dims.GetPixelX(WipeRight ? xPos : xMax - xPos);
            float y = dims.GetPixelY(Streamer.Data[i + Streamer.NextIndex] + Streamer.OffsetY);
            oldest[i] = new(x, y);
        }

        using var gfx = ScottPlot.Drawing.GDI.Graphics(bmp, dims, lowQuality);
        using var pen = ScottPlot.Drawing.GDI.Pen(Streamer.Color, Streamer.LineWidth, LineStyle.Solid);

        //if (oldest.Length > 1)
        //    gfx.DrawLines(pen, oldest);

        //if (newest.Length > 1)
        //    gfx.DrawLines(pen, newest);

        if (StepDisplay)
        {
            PointF[] pointsStep = GetStepDisplayPoints(oldest, true);
            gfx.DrawLines(pen, pointsStep);
        }
        else
        {
            if (oldest.Length > 1)
                gfx.DrawLines(pen, oldest);

            if (newest.Length > 1)
                gfx.DrawLines(pen, newest);
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
