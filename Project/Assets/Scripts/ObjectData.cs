public class ObjectData
{
    public Point2 pos; //позиция объекта
    public double rad; //радиус зоны попадания объекта
    public double speed; //скорость объекта
    public bool collided; //столкнулся ли объект?
    public bool breakable; //может ли объект разломаться на несколько частей?

    private double timePassed; //прошедшее время
    public double lifeTime; //время "жизни" объекта

    public ObjectData(Point2 pos, double rad, double speed, bool breakable)
    {
        this.pos = pos;
        this.rad = rad;
        this.speed = speed;
        this.breakable = breakable;
    }

    /// <summary>
    /// Вычисление угла поворота
    /// </summary>
    /// <param name="dir">направление поворота</param>
    /// <param name="speed">скорость вращения</param>
    /// <returns>угол поворота</returns>
    public double CalcRot(float dir, float speed)
    {
        return dir * speed;
    }

    /// <summary>
    /// Проверка коллизии объектов
    /// </summary>
    /// <param name="c">объект проверки на коллизию</param>
    /// <returns>состояние коллизии данного объекта</returns>
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
    /// Вычисление следующей позиции корабля
    /// </summary>
    /// <param name="pos2">направляющий вектор</param>
    /// <param name="t">время</param>
    public void CalcMovShip(Point2 pos2, double t)
    {
        pos += pos2 * t * speed;
    }

    /// <summary>
    /// Вычисление следующей позиции летающей тарелки
    /// </summary>
    /// <param name="pos2">позиция корабля игрока</param>
    /// <param name="t">время</param>
    public void CalcMovUFO(Point2 pos2, double t)
    {
        Point2 nV = pos2 - pos;
        double playerDist = nV.VecMag();
        Point2 normV = nV / playerDist;

        pos += normV * speed * t;
    }

    /// <summary>
    /// Вычисление следующей позиции астероида/пули
    /// </summary>
    /// <param name="pos2">направляющий вектор</param>
    /// <param name="t">время</param>
    /// <returns>состояние астероида/пули</returns>
    public bool CalcMovAstBullet(Point2 pos2, double t)
    {
        //если пуля ни в кого не попала через определенное время - удалить её
        if (timePassed >= lifeTime)
            return true;
        //иначе вычисляем следующую позицию
        pos += pos2 * t * speed;
        timePassed += t;
        return false;
    }
}
