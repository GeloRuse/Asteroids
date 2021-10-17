public class LaserData
{
    public Point2 laserPos; //позиция лазера
    public bool laserActive; //действует ли лазер в данный момент?
    public double laserColAngle; //угол коллизии лазера

    private double laserTime; //время с момента выстрела лазера
    public double laserChargeTime; //время перезарядки лазера
    public double laserCharges; //количество зарядов лазера
    public double laserMaxCharges; //максимальное кол-во зарядов
    private double laserAtkTime;  //время активности лазера
    public double laserAtkTimeLimit; //максимальное время активности лазера

    public LaserData(double laserChargeTime, double laserMaxCharges, double laserAtkTimeLimit, double laserColAngle)
    {
        this.laserChargeTime = laserChargeTime;
        this.laserCharges = laserMaxCharges;
        this.laserMaxCharges = laserMaxCharges;
        this.laserAtkTimeLimit = laserAtkTimeLimit;
        this.laserColAngle = laserColAngle;
    }

    /// <summary>
    /// Расчёты лазера
    /// </summary>
    /// <param name="nPos">позиция лазера</param>
    /// <param name="t">время</param>
    public void CalcLaser(Point2 nPos, float t)
    {
        //если не достигнуто максимальное кол-во зарядов лазера, то считаем время перезарядки
        if (laserCharges < laserMaxCharges)
            laserTime += t;
        //если время перезарядки прошло, то увеличиваем кол-во зарядов
        if (laserTime >= laserChargeTime && laserCharges < laserMaxCharges)
        {
            laserTime = 0;
            laserCharges++;
        }
        //если лазер действует, обновляем его данные
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
    /// Выстрел лазером
    /// </summary>
    /// <param name="nPos">позиция лазера</param>
    /// <returns>возможно ли выстрелить лазером</returns>
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
    /// Время перезарядки лазера
    /// </summary>
    /// <returns>оставшееся время</returns>
    public double LaserCD()
    {
        return laserChargeTime - laserTime;
    }
}
