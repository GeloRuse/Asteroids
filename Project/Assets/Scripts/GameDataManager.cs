using System;
using System.Collections.Generic;

public class GameDataManager
{
    static Random rng = new Random();

    public double screenWidth; //ширина экрана в координатах
    public double screenHeight; //высота экрана в координатах
    public int score; //счЄт игрока

    public List<ObjectData> projectiles = new List<ObjectData>(); //список данных пуль
    public List<ObjectData> projectilesToRemove = new List<ObjectData>(); //список пуль, помеченных на удаление

    public List<ObjectData> enemies = new List<ObjectData>(); //список данных противников (астероидов и летающих тарелок)
    public List<ObjectData> smallAsteroids = new List<ObjectData>(); //список осколков астероидов
    public List<ObjectData> enemiesToRemove = new List<ObjectData>(); //список противников, помеченных на удаление

    public ObjectData playerShip; //данные корабл€ игрока
    public bool playerDead; //состо€ние корабл€ игрока

    public LaserData laser; //данные лазера


    /// <summary>
    /// ѕроверка объектов на коллизию
    /// </summary>
    public void CheckCollisions()
    {
        //проверка дл€ каждого "противника" в списке
        foreach (ObjectData e in enemies)
        {
            //если произошла коллизи€ с игроком, то завершаем игру
            if (!playerDead && e.Collided(playerShip))
            {
                enemiesToRemove.Add(e);
                playerDead = true;
                return;
            }
            //проверка на коллизию с лазером игрока
            if (laser.laserActive && !e.collided)
            {
                //вычисл€ем угол между лазером и противником дл€ определени€ коллизии
                Point2 v1 = laser.laserPos - playerShip.pos;
                Point2 v2 = e.pos - playerShip.pos;
                double angl = (Math.Atan2(v1.y, v1.x) - Math.Atan2(v2.y, v2.x)) * (180 / Math.PI);
                //если произошла коллизи€, то добавл€ем противника в список на удаление и увеличиваем счЄт
                if (Math.Abs(angl) < laser.laserColAngle)
                {
                    e.collided = true;
                    enemiesToRemove.Add(e);
                    score++;
                }
            }
            //провер€ем коллизию с пул€ми игрока
            foreach (ObjectData p in projectiles)
            {
                //если противник столкнулс€ с пулей игрока, то добавл€ем пулю и противника в списки на удаление и увеличиваем счЄт
                if (!e.collided && !p.collided && e.Collided(p))
                {
                    p.collided = true;
                    e.collided = true;
                    enemiesToRemove.Add(e);
                    projectilesToRemove.Add(p);
                    //если объект может расколотьс€, до добавл€ем его в список на создание осколков
                    if (e.breakable)
                    {
                        smallAsteroids.Add(e);
                    }
                    score++;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// ѕеремещение корабл€ в противоположную сторону, если достигнут край экрана
    /// </summary>
    /// <returns>статус перемещени€</returns>
    public bool CalcWarp()
    {
        if (playerShip.pos.x >= screenWidth || playerShip.pos.x <= -screenWidth)
        {
            playerShip.pos.x = -playerShip.pos.x;
            return true;
        }
        if (playerShip.pos.y >= screenHeight || playerShip.pos.y <= -screenHeight)
        {
            playerShip.pos.y = -playerShip.pos.y;
            return true;
        }
        return false;
    }

    /// <summary>
    /// ќпределение стороны по€влени€ объекта и его создание с указаными параметрами
    /// </summary>
    /// <param name="radius">зона попадани€ объекта</param>
    /// <param name="speed">скорость объекта</param>
    /// <param name="brkbl">может ли объект разломатьс€ на части</param>
    /// <returns>новый объект с указаными параметрами</returns>
    public ObjectData Spawn(double radius, double speed, bool brkbl)
    {
        Point2 nPoint = new Point2();
        //выбираем случайную сторону дл€ по€влени€ объекта
        switch (rng.Next(4))
        {
            case 0:
                nPoint.x = rng.NextDouble() * (screenWidth + screenWidth) - screenWidth; //случайное место сверху
                nPoint.y = screenHeight;
                break;
            case 1:
                nPoint.x = rng.NextDouble() * (screenWidth + screenWidth) - screenWidth; //снизу
                nPoint.y = -screenHeight;
                break;
            case 2:
                nPoint.y = rng.NextDouble() * (screenHeight + screenHeight) - screenHeight; //слева
                nPoint.x = -screenWidth;
                break;
            case 3:
                nPoint.y = rng.NextDouble() * (screenHeight + screenHeight) - screenHeight; //справа
                nPoint.x = screenWidth;
                break;
        }
        return new ObjectData(nPoint, radius, speed, brkbl);
    }
}
