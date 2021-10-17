public class LaserData
{
    public Point2 laserPos; //������� ������
    public bool laserActive; //��������� �� ����� � ������ ������?
    public double laserColAngle; //���� �������� ������

    private double laserTime; //����� � ������� �������� ������
    public double laserChargeTime; //����� ����������� ������
    public double laserCharges; //���������� ������� ������
    public double laserMaxCharges; //������������ ���-�� �������
    private double laserAtkTime;  //����� ���������� ������
    public double laserAtkTimeLimit; //������������ ����� ���������� ������

    public LaserData(double laserChargeTime, double laserMaxCharges, double laserAtkTimeLimit, double laserColAngle)
    {
        this.laserChargeTime = laserChargeTime;
        this.laserCharges = laserMaxCharges;
        this.laserMaxCharges = laserMaxCharges;
        this.laserAtkTimeLimit = laserAtkTimeLimit;
        this.laserColAngle = laserColAngle;
    }

    /// <summary>
    /// ������� ������
    /// </summary>
    /// <param name="nPos">������� ������</param>
    /// <param name="t">�����</param>
    public void CalcLaser(Point2 nPos, float t)
    {
        //���� �� ���������� ������������ ���-�� ������� ������, �� ������� ����� �����������
        if (laserCharges < laserMaxCharges)
            laserTime += t;
        //���� ����� ����������� ������, �� ����������� ���-�� �������
        if (laserTime >= laserChargeTime && laserCharges < laserMaxCharges)
        {
            laserTime = 0;
            laserCharges++;
        }
        //���� ����� ���������, ��������� ��� ������
        if (laserAtkTime < laserAtkTimeLimit && laserActive)
        {
            laserPos = nPos;
            laserAtkTime += t;
        }
        else
        {
            laserActive = false;
        }
    }

    /// <summary>
    /// ������� �������
    /// </summary>
    /// <param name="nPos">������� ������</param>
    /// <returns>�������� �� ���������� �������</returns>
    public bool Fire(Point2 nPos)
    {
        if (laserCharges > 0)
        {
            laserCharges--;
            laserAtkTime = 0;
            laserActive = true;
            laserPos = nPos;
            return true;
        }
        return false;
    }

    /// <summary>
    /// ����� ����������� ������
    /// </summary>
    /// <returns>���������� �����</returns>
    public double LaserCD()
    {
        return laserChargeTime - laserTime;
    }
}
