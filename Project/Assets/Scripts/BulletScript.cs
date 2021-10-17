using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public GameController gc; //�������� ����

    public ObjectData bData; //������ ����
    public float speed = 10; //�������� ����
    public float hitRadius = .1f; //������ ���� �������� ����

    public float lifeTime; //����� "�����" ����

    private void Start()
    {
        //������� ����
        bData = new ObjectData(gc.V2P(transform.position), hitRadius, speed, false);
        bData.lifeTime = lifeTime;

        //��������� ���� � ������
        gc.gdm.projectiles.Add(bData);
        gc.projs.Add(gameObject);
    }

    /// <summary>
    /// ���������� ��������� ������� ����
    /// </summary>
    /// <param name="t">�����</param>
    /// <returns>��������� ����</returns>
    public bool CalcMovement(float t)
    {
        if (bData.CalcMovAstBullet(gc.V2P(transform.up), t))
            return true;
        transform.position = gc.P2V(bData.pos);
        return false;
    }
}
