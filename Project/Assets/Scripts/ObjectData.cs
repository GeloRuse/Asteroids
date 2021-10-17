public class ObjectData
{
    public Point2 pos; //������� �������
    public double rad; //������ ���� ��������� �������
    public double speed; //�������� �������
    public bool collided; //���������� �� ������?
    public bool breakable; //����� �� ������ ����������� �� ��������� ������?

    private double timePassed; //��������� �����
    public double lifeTime; //����� "�����" �������

    public ObjectData(Point2 pos, double rad, double speed, bool breakable)
    {
        this.pos = pos;
        this.rad = rad;
        this.speed = speed;
        this.breakable = breakable;
    }

    /// <summary>
    /// ���������� ���� ��������
    /// </summary>
    /// <param name="dir">����������� ��������</param>
    /// <param name="speed">�������� ��������</param>
    /// <returns>���� ��������</returns>
    public double CalcRot(float dir, float speed)
    {
        return dir * speed;
    }

    /// <summary>
    /// �������� �������� ��������
    /// </summary>
    /// <param name="c">������ �������� �� ��������</param>
    /// <returns>��������� �������� ������� �������</returns>
    public bool Collided(ObjectData c)
    {
        double vMag = (pos - c.pos).VecMag();
        double radSumSq = (rad + c.rad) * (rad + c.rad);
        if (vMag * vMag < radSumSq)
            return true;
        else
            return false;
    }

    /// <summary>
    /// ���������� ��������� ������� �������
    /// </summary>
    /// <param name="pos2">������������ ������</param>
    /// <param name="t">�����</param>
    public void CalcMovShip(Point2 pos2, double t)
    {
        pos += pos2 * t * speed;
    }

    /// <summary>
    /// ���������� ��������� ������� �������� �������
    /// </summary>
    /// <param name="pos2">������� ������� ������</param>
    /// <param name="t">�����</param>
    public void CalcMovUFO(Point2 pos2, double t)
    {
        Point2 nV = pos2 - pos;
        double playerDist = nV.VecMag();
        Point2 normV = nV / playerDist;

        pos += normV * speed * t;
    }

    /// <summary>
    /// ���������� ��������� ������� ���������/����
    /// </summary>
    /// <param name="pos2">������������ ������</param>
    /// <param name="t">�����</param>
    /// <returns>��������� ���������/����</returns>
    public bool CalcMovAstBullet(Point2 pos2, double t)
    {
        //���� ���� �� � ���� �� ������ ����� ������������ ����� - ������� �
        if (timePassed >= lifeTime)
            return true;
        //����� ��������� ��������� �������
        pos += pos2 * t * speed;
        timePassed += t;
        return false;
    }
}
