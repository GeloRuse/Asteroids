using UnityEngine;

public class UFOScript : MonoBehaviour
{
    public GameController gc; //менеджер игры

    public ObjectData uData; //данные летающей тарелки
    public float speed = 1; //скорость тарелки
    public float hitRadius = .3f; //радиус зоны коллизии тарелки

    private void Start()
    {
        //создаем тарелку
        uData = gc.gdm.Spawn(hitRadius, speed, false);
        transform.position = gc.P2V(uData.pos);

        //добавляем тарелку в списки
        gc.gdm.enemies.Add(uData);
        gc.ufosAdded.Add(gameObject);
    }

    /// <summary>
    /// Вычисление следующей позиции летающей тарелки
    /// </summary>
    /// <param name="playerPos">позиция игрока</param>
    public void CalcMovement(Vector3 playerPos)
    {
        uData.CalcMovUFO(gc.V2P(playerPos), Time.deltaTime);
        transform.position = transform.rotation * gc.P2V(uData.pos);
    }
}
