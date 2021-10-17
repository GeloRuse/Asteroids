using System;

public class Point2
{
    public double x; //X �����
    public double y; //Y �����

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
    /// ������������ �����
    /// </summary>
    /// <param name="p1">����� 1</param>
    /// <param name="p2">����� 2</param>
    /// <returns>����� �����</returns>
    public static Point2 operator +(Point2 p1, Point2 p2)
    {
        return new Point2(p1.x + p2.x, p1.y + p2.y);
    }

    /// <summary>
    /// ��������� �����
    /// </summary>
    /// <param name="p1">����� 1</param>
    /// <param name="p2">����� 2</param>
    /// <returns>������� �����</returns>
    public static Point2 operator -(Point2 p1, Point2 p2)
    {
        return new Point2(p1.x - p2.x, p1.y - p2.y);
    }

    /// <summary>
    /// ��������� ����� �� �����
    /// </summary>
    /// <param name="v">�����</param>
    /// <param name="m">�����</param>
    /// <returns>�����, ���������� �� �����</returns>
    public static Point2 operator *(Point2 v, double m)
    {
        return new Point2(v.x * m, v.y * m);
    }

    /// <summary>
    /// ������� ����� �� �����
    /// </summary>
    /// <param name="p">�����</param>
    /// <param name="d">�����</param>
    /// <returns>�����, ����������� �� �����</returns>
    public static Point2 operator /(Point2 p, double d)
    {
        return new Point2(p.x / d, p.y / d);
    }

    /// <summary>
    /// ���������� �������� ������� 
    /// </summary>
    /// <returns>�������� �������</returns>
    public double VecMag()
    {
        return Math.Sqrt(x * x + y * y);
    }
}
