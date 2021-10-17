using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameController gc; //менеджер игры
    public Controls ctrls; //управление кораблём
    public ObjectData pData; //данные корабля игрока

    public float maxSpeed = 5; //максимальная скорость корабля
    private float speed; //текущая скорость корабля
    public float accelerationSpeed = 1; //темп ускорения корабля
    public float decay = .95f; //темп потери скорости корабля

    public float rotationSpeed = 200; //скорость поворота
    public float radius = .1f; //радиус зоны коллизии корабля

    public GameObject bullet; //игровой объект пули
    public Transform bulletOrigin; //место выстрела пули
    public float fireDelay = 1; //задержка стрельбы
    private float fireTime; //время с момента выстрела пули

    public GameObject laser; //игровой объект лазера
    public LaserData lData; //данные лазера
    public float laserChargeTime = 5; //время перезарядки лазера
    public float laserMaxCharges = 3; //максимальное кол-во зарядов
    public float laserAtkTimeLimit = 1; //максимальное время активности лазера
    public float laserColAngle = 2f; //угол коллизии лазера

    private bool isThrusting; //ускоряется ли корабль
    private float rot; //угол поворота
    private Point2 velo = new Point2(); //вектор ускорения
    private Point2 oldPos; //предыдущая позиция корабля

    // Start is called before the first frame update
    private void Start()
    {
        //настройка управления
        ctrls = new Controls();
        ctrls.Player.Thrust.performed += _ => Accelerate(); //назначение метода для движения корабля вперёд по нажатию
        ctrls.Player.Thrust.canceled += _ => StopAcceleration(); //для прекращения движения вперёд
        ctrls.Player.Rotate.performed += ctx => Rotate(ctx.ReadValue<Vector2>()); //для поворота корабля
        ctrls.Player.Attack.performed += _ => Attack(); //для выстрела пулей
        ctrls.Player.SpecialAttack.performed += _ => SpecialAttack(); //для выстрела лазером
        ctrls.Other.Exit.performed += _ => CloseGame(); //для выхода из игры
        ctrls.Enable();

        //создание данных корабля
        pData = new ObjectData(new Point2(), radius, accelerationSpeed, false);
        gc.gdm.playerShip = pData;
        //создание данных лазера
        lData = new LaserData(laserChargeTime, laserMaxCharges, laserAtkTimeLimit, laserColAngle);
        gc.gdm.laser = lData;

        fireTime = fireDelay;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float t = Time.deltaTime;
        fireTime += t;
        CalcMovement(t);
        //определяем состояние лазера
        lData.CalcLaser(gc.V2P(laser.transform.position), t);
        if (!lData.laserActive)
            laser.SetActive(false);

        UpdateText();
    }

    /// <summary>
    /// Вычисление следующей позиции корабля
    /// </summary>
    /// <param name="t">время</param>
    public void CalcMovement(float t)
    {
        //перемещаем корабль в противоположную сторону экрана, если он достиг края
        if (gc.gdm.CalcWarp())
            transform.position = gc.P2V(pData.pos);

        oldPos = gc.V2P(transform.position);

        //расчёт вектора ускорения
        if (!isThrusting || (velo.VecMag() > maxSpeed))
        {
            velo *= decay; //корабль останавливается
        }
        else
        {
            velo += gc.V2P(transform.up); //корабль ускоряется
        }
        //вычисляем новую позицию корабля
        pData.CalcMovShip(velo, t);
        transform.position = gc.P2V(pData.pos);
        transform.Rotate(0, 0, rot * t);

        speed = (float)((pData.pos - oldPos).VecMag() / t);
    }

    /// <summary>
    /// Ускорение корабля
    /// </summary>
    private void Accelerate()
    {
        isThrusting = true;
    }

    /// <summary>
    /// Прекращение ускорения
    /// </summary>
    private void StopAcceleration()
    {
        isThrusting = false;
    }

    /// <summary>
    /// Выстрел пулей
    /// </summary>
    private void Attack()
    {
        if (fireTime >= fireDelay && !lData.laserActive)
        {
            fireTime = 0;
            BulletScript bul = Instantiate(bullet, bulletOrigin.position, transform.rotation).GetComponent<BulletScript>();
            bul.gc = gc;
        }
    }

    /// <summary>
    /// Выстрел лазером
    /// </summary>
    private void SpecialAttack()
    {
        if (lData.Fire(gc.V2P(laser.transform.position)))
            laser.SetActive(true);
    }

    /// <summary>
    /// Вычисление угла поворота
    /// </summary>
    /// <param name="inpDir">направление поворота</param>
    private void Rotate(Vector2 inpDir)
    {
        rot = (float)pData.CalcRot(-inpDir.x, rotationSpeed);
    }

    /// <summary>
    /// Обновление таблицы информации
    /// </summary>
    private void UpdateText()
    {
        gc.txtCoords.text = "Координаты: " + transform.position.x.ToString("F2") + ", " + transform.position.y.ToString("F2");
        gc.txtRot.text = "Угол поворота: " + Mathf.Round(360 - transform.rotation.eulerAngles.z);
        gc.txtSpeed.text = "Скорость: " + speed.ToString("F2");
        gc.txtLasChrg.text = "Кол-во зарядов лазера: " + lData.laserCharges;
        gc.txtLasCD.text = "Время перезарядки: " + lData.LaserCD().ToString("F2");
    }

    /// <summary>
    /// Выход из игры
    /// </summary>
    private void CloseGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Отключение управления
    /// </summary>
    private void OnDisable()
    {
        ctrls.Disable();
    }
}
