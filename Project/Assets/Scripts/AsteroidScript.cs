using UnityEngine;

public class AsteroidScript : MonoBehaviour
{
    public GameController gc; //менеджер игры

    public ObjectData aData; //данные астероида
    public float speed = 1; //скорость астероида
    public float hitRadius = .2f; //радиус зоны коллизии астероида

    public float angleVar = 15f; //вариация направления движения астероида
    public float lifeTime = 20f; //время "жизни" астероида

    /// <summary>
    /// Создание астероида
    /// </summary>
    public void CreateAsteroid()
    {
        //создаем астероид
        aData = gc.gdm.Spawn(hitRadius, speed, true);
        aData.lifeTime = lifeTime;
        transform.position = gc.P2V(aData.pos);

        //направляем астероид в центр экрана с указанным отклонением
        float variance = Random.Range(-angleVar, angleVar);
        transform.up = (gc.transform.position - transform.position);
        transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + variance);

        //добавляем астероид в списки
        gc.gdm.enemies.Add(aData);
        gc.astAdded.Add(gameObject);

    }

    /// <summary>
    /// Создание осколка астероида
    /// </summary>
    /// <param name="od">исходный астероид</param>
    public void CreateSmallAsteroid(ObjectData od)
    {
        //создаем осколок астероида
        transform.localScale = transform.localScale * .5f;
        aData = new ObjectData(od.pos, od.rad / 2, od.speed * 2, false);
        aData.lifeTime = lifeTime;
        transform.position = gc.P2V(od.pos);

        //движение осколка в случайном направлении
        float variance = Random.Range(-180, 180);
        transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + variance);

        //добавляем осколок в списки
        gc.gdm.enemies.Add(GetComponent<AsteroidScript>().aData);
        gc.astAdded.Add(gameObject);
    }

    /// <summary>
    /// Вычисление следующей позиции астероида
    /// </summary>
    /// <param name="t">время</param>
    /// <returns>состояние астероида</returns>
    public bool CalcMovement(float t)
    {
        if (aData.CalcMovAstBullet(gc.V2P(transform.up), t))
            return true;
        transform.position = gc.P2V(aData.pos);
        return false;
    }
}
