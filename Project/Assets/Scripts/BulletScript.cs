using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public GameController gc; //менеджер игры

    public ObjectData bData; //данные пули
    public float speed = 10; //скорость пули
    public float hitRadius = .1f; //радиус зоны коллизии пули

    public float lifeTime; //время "жизни" пули

    private void Start()
    {
        //создаем пулю
        bData = new ObjectData(gc.V2P(transform.position), hitRadius, speed, false);
        bData.lifeTime = lifeTime;

        //добавляем пулю в списки
        gc.gdm.projectiles.Add(bData);
        gc.projs.Add(gameObject);
    }

    /// <summary>
    /// Вычисление следующей позиции пули
    /// </summary>
    /// <param name="t">время</param>
    /// <returns>состояние пули</returns>
    public bool CalcMovement(float t)
    {
        if (bData.CalcMovAstBullet(gc.V2P(transform.up), t))
            return true;
        transform.position = gc.P2V(bData.pos);
        return false;
    }
}
