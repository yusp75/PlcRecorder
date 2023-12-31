﻿/*
 *  Work in this file is derived from code originally written by Hans-Peter Moser:
 *  http://www.mosismath.com/AngleSplines/EndSlopeSplines.html
 *  It is included in ScottPlot under a MIT license with permission from the original author.
 */
using System;

namespace ScottPlot.Statistics.Interpolation
{
    /// <summary>
    /// The End Slope Spline is a Natural Spline whose first and last point slopes can be defined
    /// </summary>
    [Obsolete("This class has been deprecated. Use ScottPlot.Statistics.Interpolation.Cubic.InterpolateXY()")]
    public class EndSlopeSpline : SplineInterpolator
    {
        public EndSlopeSpline(double[] xs, double[] ys,
            int resolution = 10, double firstSlopeDegrees = 0, double lastSlopeDegrees = 0) :
            base(xs, ys, resolution)
        {
            m = new Matrix(n);
            gauss = new MatrixSolver(n, m);

            a = new double[n];
            b = new double[n];
            c = new double[n];
            d = new double[n];
            h = new double[n];

            CalcParameters(firstSlopeDegrees, lastSlopeDegrees);
            Integrate();
            Interpolate();
        }

        public void CalcParameters(double alpha, double beta)
        {
            for (int i = 0; i < n; i++)
                a[i] = givenYs[i];

            for (int i = 0; i < n - 1; i++)
                h[i] = givenXs[i + 1] - givenXs[i];

            m.a[0, 0] = 2.0 * h[0];
            m.a[0, 1] = h[0];
            m.y[0] = 3 * ((a[1] - a[0]) / h[0] - Math.Tan(alpha * Math.PI / 180));

            for (int i = 0; i < n - 2; i++)
            {
                m.a[i + 1, i] = h[i];
                m.a[i + 1, i + 1] = 2.0 * (h[i] + h[i + 1]);
                if (i < n - 2)
                    m.a[i + 1, i + 2] = h[i + 1];

                if ((h[i] != 0.0) && (h[i + 1] != 0.0))
                    m.y[i + 1] = ((a[i + 2] - a[i + 1]) / h[i + 1] - (a[i + 1] - a[i]) / h[i]) * 3.0;
                else
                    m.y[i + 1] = 0.0;
            }

            m.a[n - 1, n - 2] = h[n - 2];
            m.a[n - 1, n - 1] = 2.0 * h[n - 2];
            m.y[n - 1] = 3.0 * (Math.Tan(beta * Math.PI / 180) - (a[n - 1] - a[n - 2]) / h[n - 2]);

            if (gauss.Eliminate() == false)
                throw new InvalidOperationException();

            gauss.Solve();

            for (int i = 0; i < n; i++)
            {
                c[i] = m.x[i];
            }
            for (int i = 0; i < n; i++)
            {
                if (h[i] != 0.0)
                {
                    d[i] = 1.0 / 3.0 / h[i] * (c[i + 1] - c[i]);
                    b[i] = 1.0 / h[i] * (a[i + 1] - a[i]) - h[i] / 3.0 * (c[i + 1] + 2 * c[i]);
                }
            }
        }
    }
}
