using System;
using System.Runtime.Serialization;

namespace CAM.Utils;

public sealed class MyBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName) => Type.GetType($"{typeName}, {assemblyName}");
}

public static class Helpers
{
    public static (double, int) FindExtremum(double[] x, double[] y, double x1, double x2, Func<double, double, double> extremumFunc, int startIndex = 0)
    {
        var i = startIndex;
        while (x[i] < x1)
            i++;

        var index = i - 1;
        var extremum = i == 0
            ? y[0]
            : GetY(x1);

        while (i < x.Length && x[i] < x2)
        {
            extremum = extremumFunc(y[i], extremum);
            i++;
        }

        if (i < x.Length)
            extremum = GetY(x2);

        return (extremum, index);

        double GetY(double p) => LinearInterpolate(p, x[i - 1], x[i], y[i - 1], y[i]);
    }

    public static double LinearInterpolate(double x, double x0, double x1, double y0, double y1) => y0 + (y1 - y0) * (x - x0) / (x1 - x0);
}