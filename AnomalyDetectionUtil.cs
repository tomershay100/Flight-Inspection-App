using System;

namespace DesktopApp
{
    //Line class
    public class Line
    {
        public float a, b;
        public Line()
        {
            a = 0;
            b = 0;
        }
        public Line(float a, float b)
        {
            this.a = a;
            this.b = b;
        }
        public float f(float x)
        {
            return a * x + b;
        }
    }
    //Point class
    public class Point
    {
        public float x, y;
        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
    //AnomalyDetectionUtil library
    public class AnomalyDetectionUtil
    {
        //average of x
        public static float Avg(float[] x)
        {
            float sum = 0;
            for (int i = 0; i < x.Length; sum += x[i], i++) ;
            return sum / x.Length;
        }
        // returns the variance of X and Y
        public static float Var(float[] x)
        {
            float av = Avg(x);
            float sum = 0;
            for (int i = 0; i < x.Length; i++)
            {
                sum += x[i] * x[i];
            }

            return sum / x.Length - av * av;
        }
        // returns the covariance of X and Y
        public static float Cov(float[] x, float[] y)
        {
            int size = Math.Min(y.Length, x.Length); //same as y.Length
            float sum = 0;
            for (int i = 0; i < size; i++)
            {
                sum += x[i] * y[i];
            }

            sum /= size;

            return sum - Avg(x) * Avg(y);
        }
        // returns the Pearson correlation coefficient of X and Y
        public static float Pearson(float[] x, float[] y)
        {
            float r = Cov(x, y) / (float) (Math.Sqrt(Var(x)) * Math.Sqrt(Var(y)));
            return float.IsNaN(r) ? 0 : r;
        }
        // performs a linear regression and returns the line equation
        public static Line LinearReg(Point[] points)
        {
            int size = points.Length;
            float[] x = new float[size];
            float[] y = new float[size];
            for (int i = 0; i < size; i++)
            {
                x[i] = points[i].x;
                y[i] = points[i].y;
            }

            float a = Cov(x, y) / Var(x);
            float b = Avg(y) - a * (Avg(x));

            return new Line(a, b);
        }
        // returns the deviation between point p and the line equation of the points
        public static float Dev(Point p, Point[] points)
        {
            Line l = LinearReg(points);
            return Dev(p, l);
        }
        // returns the deviation between point p and the line
        public static float Dev(Point p, Line l)
        {
            float x = p.y - l.f(p.x);
            if (x < 0)
                x *= -1;
            return x;
        }
    }
}