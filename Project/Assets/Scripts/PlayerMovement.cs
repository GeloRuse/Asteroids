using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameController gc; //�������� ����
    public Controls ctrls; //���������� �������
    public ObjectData pData; //������ ������� ������

    public float maxSpeed = 5; //������������ �������� �������
    private float speed; //������� �������� �������
    public float accelerationSpeed = 1; //���� ��������� �������
    public float decay = .95f; //���� ������ �������� �������

    public float rotationSpeed = 200; //�������� ��������
    public float radius = .1f; //������ ���� �������� �������

    public GameObject bullet; //������� ������ ����
    public Transform bulletOrigin; //����� �������� ����
    public float fireDelay = 1; //�������� ��������
    private float fireTime; //����� � ������� �������� ����

    public GameObject laser; //������� ������ ������
    public LaserData lData; //������ ������
    public float laserChargeTime = 5; //����� ����������� ������
    public float laserMaxCharges = 3; //������������ ���-�� �������
    public float laserAtkTimeLimit = 1; //������������ ����� ���������� ������
    public float laserColAngle = 2f; //���� �������� ������

    private bool isThrusting; //���������� �� �������
    private float rot; //���� ��������
    private Point2 velo = new Point2(); //������ ���������
    private Point2 oldPos; //���������� ������� �������

    // Start is called before the first frame update
    private void Start()
    {
        //��������� ����������
        ctrls = new Controls();
        ctrls.Player.Thrust.performed += _ => Accelerate(); //���������� ������ ��� �������� ������� ����� �� �������
        ctrls.Player.Thrust.canceled += _ => StopAcceleration(); //��� ����������� �������� �����
        ctrls.Player.Rotate.performed += ctx => Rotate(ctx.ReadValue<Vector2>()); //��� �������� �������
        ctrls.Player.Attack.performed += _ => Attack(); //��� �������� �����
        ctrls.Player.SpecialAttack.performed += _ => SpecialAttack(); //��� �������� �������
        ctrls.Other.Exit.performed += _ => CloseGame(); //��� ������ �� ����
        ctrls.Enable();

        //�������� ������ �������
        pData = new ObjectData(new Point2(), radius, accelerationSpeed, false);
        gc.gdm.playerShip = pData;
        //�������� ������ ������
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
        //���������� ��������� ������
        lData.CalcLaser(gc.V2P(laser.transform.position), t);
        if (!lData.laserActive)
            laser.SetActive(false);

        UpdateText();
    }

    /// <summary>
    /// ���������� ��������� ������� �������
    /// </summary>
    /// <param name="t">�����</param>
    public void CalcMovement(float t)
    {
        //���������� ������� � ��������������� ������� ������, ���� �� ������ ����
        if (gc.gdm.CalcWarp())
            transform.position = gc.P2V(pData.pos);

        oldPos = gc.V2P(transform.position);

        //������ ������� ���������
        if (!isThrusting || (velo.VecMag() > maxSpeed))
        {
            velo *= decay; //������� ���������������
        }
        else
        {
            velo += gc.V2P(transform.up); //������� ����������
        }
        //��������� ����� ������� �������
        pData.CalcMovShip(velo, t);
        transform.position = gc.P2V(pData.pos);
        transform.Rotate(0, 0, rot * t);

        speed = (float)((pData.pos - oldPos).VecMag() / t);
    }

    /// <summary>
    /// ��������� �������
    /// </summary>
    private void Accelerate()
    {
        isThrusting = true;
    }

    /// <summary>
    /// ����������� ���������
    /// </summary>
    private void StopAcceleration()
    {
        isThrusting = false;
    }

    /// <summary>
    /// ������� �����
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
    /// ������� �������
    /// </summary>
    private void SpecialAttack()
    {
        if (lData.Fire(gc.V2P(laser.transform.position)))
            laser.SetActive(true);
    }

    /// <summary>
    /// ���������� ���� ��������
    /// </summary>
    /// <param name="inpDir">����������� ��������</param>
    private void Rotate(Vector2 inpDir)
    {
        rot = (float)pData.CalcRot(-inpDir.x, rotationSpeed);
    }

    /// <summary>
    /// ���������� ������� ����������
    /// </summary>
    private void UpdateText()
    {
        gc.txtCoords.text = "����������: " + transform.position.x.ToString("F2") + ", " + transform.position.y.ToString("F2");
        gc.txtRot.text = "���� ��������: " + Mathf.Round(360 - transform.rotation.eulerAngles.z);
        gc.txtSpeed.text = "��������: " + speed.ToString("F2");
        gc.txtLasChrg.text = "���-�� ������� ������: " + lData.laserCharges;
        gc.txtLasCD.text = "����� �����������: " + lData.LaserCD().ToString("F2");
    }

    /// <summary>
    /// ����� �� ����
    /// </summary>
    private void CloseGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// ���������� ����������
    /// </summary>
    private void OnDisable()
    {
        ctrls.Disable();
    }
}
