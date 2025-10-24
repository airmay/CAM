using System;
using System.Runtime.Serialization;

namespace CAM.Utils;

public sealed class MyBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName) => Type.GetType($"{typeName}, {assemblyName}");
}

public static class Helpers
{
    public static (double, int) FindMax(double[] x, double[] y, double x1, double x2, int startIndex = 0)
    {
        var i = startIndex;
        while (x[i] < x1)
            i++;

        var (max, index) = i == 0
            ? (y[0], 0)
            : (GetY(x1), i - 1);

        while (i < x.Length && x[i] <= x2)
        {
            max = Math.Max(y[i], max);
            i++;
        }

        if (i < x.Length)
            max = Math.Max(GetY(x2), max);

        return (max, index);

        double GetY(double p) => LinearInterpolate(p, x[i - 1], x[i], y[i - 1], y[i]);
    }

    public static double LinearInterpolate(double x, double x0, double x1, double y0, double y1) => y0 + (y1 - y0) * (x - x0) / (x1 - x0);
}