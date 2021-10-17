using System;

public class Point2
{
    public double x; //X точки
    public double y; //Y точки

    public Point2()
    {
        this.x = 0;
        this.y = 0;
    }

    public Point2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// Суммирование точек
    /// </summary>
    /// <param name="p1">точка 1</param>
    /// <param name="p2">точка 2</param>
    /// <returns>сумма точек</returns>
    public static Point2 operator +(Point2 p1, Point2 p2)
    {
        return new Point2(p1.x + p2.x, p1.y + p2.y);
    }

    /// <summary>
    /// Вычитание точек
    /// </summary>
    /// <param name="p1">точка 1</param>
    /// <param name="p2">точка 2</param>
    /// <returns>разница точек</returns>
    public static Point2 operator -(Point2 p1, Point2 p2)
    {
        return new Point2(p1.x - p2.x, p1.y - p2.y);
    }

    /// <summary>
    /// Умножение точки на число
    /// </summary>
    /// <param name="v">точка</param>
    /// <param name="m">число</param>
    /// <returns>точка, умноженная на число</returns>
    public static Point2 operator *(Point2 v, double m)
    {
        return new Point2(v.x * m, v.y * m);
    }

    /// <summary>
    /// Деление точки на число
    /// </summary>
    /// <param name="p">точка</param>
    /// <param name="d">число</param>
    /// <returns>точка, разделенная на число</returns>
    public static Point2 operator /(Point2 p, double d)
    {
        return new Point2(p.x / d, p.y / d);
    }

    /// <summary>
    /// Вычисление величины вектора 
    /// </summary>
    /// <returns>величина вектора</returns>
    public double VecMag()
    {
        return Math.Sqrt(x * x + y * y);
    }
}
