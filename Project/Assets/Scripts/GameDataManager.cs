using System;
using System.Collections.Generic;

public class GameDataManager
{
    static Random rng = new Random();

    public double screenWidth; //������ ������ � �����������
    public double screenHeight; //������ ������ � �����������
    public int score; //���� ������

    public List<ObjectData> projectiles = new List<ObjectData>(); //������ ������ ����
    public List<ObjectData> projectilesToRemove = new List<ObjectData>(); //������ ����, ���������� �� ��������

    public List<ObjectData> enemies = new List<ObjectData>(); //������ ������ ����������� (���������� � �������� �������)
    public List<ObjectData> smallAsteroids = new List<ObjectData>(); //������ �������� ����������
    public List<ObjectData> enemiesToRemove = new List<ObjectData>(); //������ �����������, ���������� �� ��������

    public ObjectData playerShip; //������ ������� ������
    public bool playerDead; //��������� ������� ������

    public LaserData laser; //������ ������


    /// <summary>
    /// �������� �������� �� ��������
    /// </summary>
    public void CheckCollisions()
    {
        //�������� ��� ������� "����������" � ������
        foreach (ObjectData e in enemies)
        {
            //���� ��������� �������� � �������, �� ��������� ����
            if (!playerDead && e.Collided(playerShip))
            {
                enemiesToRemove.Add(e);
                playerDead = true;
                return;
            }
            //�������� �� �������� � ������� ������
            if (laser.laserActive && !e.collided)
            {
                //��������� ���� ����� ������� � ����������� ��� ����������� ��������
                Point2 v1 = laser.laserPos - playerShip.pos;
                Point2 v2 = e.pos - playerShip.pos;
                double angl = (Math.Atan2(v1.y, v1.x) - Math.Atan2(v2.y, v2.x)) * (180 / Math.PI);
                //���� ��������� ��������, �� ��������� ���������� � ������ �� �������� � ����������� ����
                if (Math.Abs(angl) < laser.laserColAngle)
                {
                    e.collided = true;
                    enemiesToRemove.Add(e);
                    score++;
                }
            }
            //��������� �������� � ������ ������
            foreach (ObjectData p in projectiles)
            {
                //���� ��������� ���������� � ����� ������, �� ��������� ���� � ���������� � ������ �� �������� � ����������� ����
                if (!e.collided && !p.collided && e.Collided(p))
                {
                    p.collided = true;
                    e.collided = true;
                    enemiesToRemove.Add(e);
                    projectilesToRemove.Add(p);
                    //���� ������ ����� �����������, �� ��������� ��� � ������ �� �������� ��������
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
    /// ����������� ������� � ��������������� �������, ���� ��������� ���� ������
    /// </summary>
    /// <returns>������ �����������</returns>
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
    /// ����������� ������� ��������� ������� � ��� �������� � ��������� �����������
    /// </summary>
    /// <param name="radius">���� ��������� �������</param>
    /// <param name="speed">�������� �������</param>
    /// <param name="brkbl">����� �� ������ ����������� �� �����</param>
    /// <returns>����� ������ � ��������� �����������</returns>
    public ObjectData Spawn(double radius, double speed, bool brkbl)
    {
        Point2 nPoint = new Point2();
        //�������� ��������� ������� ��� ��������� �������
        switch (rng.Next(4))
        {
            case 0:
                nPoint.x = rng.NextDouble() * (screenWidth + screenWidth) - screenWidth; //��������� ����� ������
                nPoint.y = screenHeight;
                break;
            case 1:
                nPoint.x = rng.NextDouble() * (screenWidth + screenWidth) - screenWidth; //�����
                nPoint.y = -screenHeight;
                break;
            case 2:
                nPoint.y = rng.NextDouble() * (screenHeight + screenHeight) - screenHeight; //�����
                nPoint.x = -screenWidth;
                break;
            case 3:
                nPoint.y = rng.NextDouble() * (screenHeight + screenHeight) - screenHeight; //������
                nPoint.x = screenWidth;
                break;
        }
        return new ObjectData(nPoint, radius, speed, brkbl);
    }
}
