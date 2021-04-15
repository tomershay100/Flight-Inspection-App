using System;
using System.Collections.Generic;

namespace DesktopApp
{
    //Line class
    public class Line
    {
        private readonly float _a;
        private readonly float _b;

        public Line()
        {
            _a = 0;
            _b = 0;
        }

        public Line(float a, float b)
        {
            this._a = a;
            this._b = b;
        }

        public float F(float x)
        {
            return _a * x + _b;
        }
    }

    //Point class
    public class Point
    {
        public readonly float X;
        public readonly float Y;

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    //AnomalyDetectionUtil library
    public static class AnomalyDetectionUtil
    {
        //average of x
        private static float Avg(IReadOnlyList<float> x)
        {
            float sum = 0;
            for (var i = 0; i < x.Count; sum += x[i], i++)
            {
            }

            return sum / x.Count;
        }

        // returns the variance of X and Y
        private static float Var(IReadOnlyList<float> x)
        {
            var av = Avg(x);
            float sum = 0;
            foreach (var t in x)
            {
                sum += t * t;
            }

            return sum / x.Count - av * av;
        }

        // returns the covariance of X and Y
        private static float Cov(IReadOnlyList<float> x, IReadOnlyList<float> y)
        {
            var size = Math.Min(y.Count, x.Count); //same as y.Length
            float sum = 0;
            for (var i = 0; i < size; i++)
            {
                sum += x[i] * y[i];
            }

            sum /= size;

            return sum - Avg(x) * Avg(y);
        }

        // returns the Pearson correlation coefficient of X and Y
        public static float Pearson(float[] x, float[] y)
        {
            var r = Cov(x, y) / (float) (Math.Sqrt(Var(x)) * Math.Sqrt(Var(y)));
            return float.IsNaN(r) ? 0 : r;
        }

        // performs a linear regression and returns the line equation
        public static Line LinearReg(Point[] points)
        {
            var size = points.Length;
            var x = new float[size];
            var y = new float[size];
            for (var i = 0; i < size; i++)
            {
                x[i] = points[i].X;
                y[i] = points[i].Y;
            }

            var a = Cov(x, y) / Var(x);
            var b = Avg(y) - a * (Avg(x));

            return new Line(a, b);
        }

        // returns the deviation between point p and the line equation of the points
        public static float Dev(Point p, Point[] points)
        {
            var l = LinearReg(points);
            return Dev(p, l);
        }

        // returns the deviation between point p and the line
        private static float Dev(Point p, Line l)
        {
            var x = p.Y - l.F(p.X);
            if (x < 0)
                x *= -1;
            return x;
        }
    }
}