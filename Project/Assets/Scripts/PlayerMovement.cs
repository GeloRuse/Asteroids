using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameController gc; //менеджер игры
    public Controls ctrls; //управление кораблЄм
    public ObjectData pData; //данные корабл€ игрока

    public float maxSpeed = 5; //максимальна€ скорость корабл€
    private float speed; //текуща€ скорость корабл€
    public float accelerationSpeed = 1; //темп ускорени€ корабл€
    public float decay = .95f; //темп потери скорости корабл€

    public float rotationSpeed = 200; //скорость поворота
    public float radius = .1f; //радиус зоны коллизии корабл€

    public GameObject bullet; //игровой объект пули
    public Transform bulletOrigin; //место выстрела пули
    public float fireDelay = 1; //задержка стрельбы
    private float fireTime; //врем€ с момента выстрела пули

    public GameObject laser; //игровой объект лазера
    public LaserData lData; //данные лазера
    public float laserChargeTime = 5; //врем€ перезар€дки лазера
    public float laserMaxCharges = 3; //максимальное кол-во зар€дов
    public float laserAtkTimeLimit = 1; //максимальное врем€ активности лазера
    public float laserColAngle = 2f; //угол коллизии лазера

    private bool isThrusting; //ускор€етс€ ли корабль
    private float rot; //угол поворота
    private Point2 velo = new Point2(); //вектор ускорени€
    private Point2 oldPos; //предыдуща€ позици€ корабл€

    // Start is called before the first frame update
    private void Start()
    {
        //настройка управлени€
        ctrls = new Controls();
        ctrls.Player.Thrust.performed += _ => Accelerate(); //назначение метода дл€ движени€ корабл€ вперЄд по нажатию
        ctrls.Player.Thrust.canceled += _ => StopAcceleration(); //дл€ прекращени€ движени€ вперЄд
        ctrls.Player.Rotate.performed += ctx => Rotate(ctx.ReadValue<Vector2>()); //дл€ поворота корабл€
        ctrls.Player.Attack.performed += _ => Attack(); //дл€ выстрела пулей
        ctrls.Player.SpecialAttack.performed += _ => SpecialAttack(); //дл€ выстрела лазером
        ctrls.Other.Exit.performed += _ => CloseGame(); //дл€ выхода из игры
        ctrls.Enable();

        //создание данных корабл€
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
        //определ€ем состо€ние лазера
        lData.CalcLaser(gc.V2P(laser.transform.position), t);
        if (!lData.laserActive)
            laser.SetActive(false);

        UpdateText();
    }

    /// <summary>
    /// ¬ычисление следующей позиции корабл€
    /// </summary>
    /// <param name="t">врем€</param>
    public void CalcMovement(float t)
    {
        //перемещаем корабль в противоположную сторону экрана, если он достиг кра€
        if (gc.gdm.CalcWarp())
            transform.position = gc.P2V(pData.pos);

        oldPos = gc.V2P(transform.position);

        //расчЄт вектора ускорени€
        if (!isThrusting || (velo.VecMag() > maxSpeed))
        {
            velo *= decay; //корабль останавливаетс€
        }
        else
        {
            velo += gc.V2P(transform.up); //корабль ускор€етс€
        }
        //вычисл€ем новую позицию корабл€
        pData.CalcMovShip(velo, t);
        transform.position = gc.P2V(pData.pos);
        transform.Rotate(0, 0, rot * t);

        speed = (float)((pData.pos - oldPos).VecMag() / t);
    }

    /// <summary>
    /// ”скорение корабл€
    /// </summary>
    private void Accelerate()
    {
        isThrusting = true;
    }

    /// <summary>
    /// ѕрекращение ускорени€
    /// </summary>
    private void StopAcceleration()
    {
        isThrusting = false;
    }

    /// <summary>
    /// ¬ыстрел пулей
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
    /// ¬ыстрел лазером
    /// </summary>
    private void SpecialAttack()
    {
        if (lData.Fire(gc.V2P(laser.transform.position)))
            laser.SetActive(true);
    }

    /// <summary>
    /// ¬ычисление угла поворота
    /// </summary>
    /// <param name="inpDir">направление поворота</param>
    private void Rotate(Vector2 inpDir)
    {
        rot = (float)pData.CalcRot(-inpDir.x, rotationSpeed);
    }

    /// <summary>
    /// ќбновление таблицы информации
    /// </summary>
    private void UpdateText()
    {
        gc.txtCoords.text = " оординаты: " + transform.position.x.ToString("F2") + ", " + transform.position.y.ToString("F2");
        gc.txtRot.text = "”гол поворота: " + Mathf.Round(360 - transform.rotation.eulerAngles.z);
        gc.txtSpeed.text = "—корость: " + speed.ToString("F2");
        gc.txtLasChrg.text = " ол-во зар€дов лазера: " + lData.laserCharges;
        gc.txtLasCD.text = "¬рем€ перезар€дки: " + lData.LaserCD().ToString("F2");
    }

    /// <summary>
    /// ¬ыход из игры
    /// </summary>
    private void CloseGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// ќтключение управлени€
    /// </summary>
    private void OnDisable()
    {
        ctrls.Disable();
    }
}
