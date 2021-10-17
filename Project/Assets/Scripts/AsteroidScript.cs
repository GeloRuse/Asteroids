using UnityEngine;

public class AsteroidScript : MonoBehaviour
{
    public GameController gc; //�������� ����

    public ObjectData aData; //������ ���������
    public float speed = 1; //�������� ���������
    public float hitRadius = .2f; //������ ���� �������� ���������

    public float angleVar = 15f; //�������� ����������� �������� ���������
    public float lifeTime = 20f; //����� "�����" ���������

    /// <summary>
    /// �������� ���������
    /// </summary>
    public void CreateAsteroid()
    {
        //������� ��������
        aData = gc.gdm.Spawn(hitRadius, speed, true);
        aData.lifeTime = lifeTime;
        transform.position = gc.P2V(aData.pos);

        //���������� �������� � ����� ������ � ��������� �����������
        float variance = Random.Range(-angleVar, angleVar);
        transform.up = (gc.transform.position - transform.position);
        transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + variance);

        //��������� �������� � ������
        gc.gdm.enemies.Add(aData);
        gc.astAdded.Add(gameObject);

    }

    /// <summary>
    /// �������� ������� ���������
    /// </summary>
    /// <param name="od">�������� ��������</param>
    public void CreateSmallAsteroid(ObjectData od)
    {
        //������� ������� ���������
        transform.localScale = transform.localScale * .5f;
        aData = new ObjectData(od.pos, od.rad / 2, od.speed * 2, false);
        aData.lifeTime = lifeTime;
        transform.position = gc.P2V(od.pos);

        //�������� ������� � ��������� �����������
        float variance = Random.Range(-180, 180);
        transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + variance);

        //��������� ������� � ������
        gc.gdm.enemies.Add(GetComponent<AsteroidScript>().aData);
        gc.astAdded.Add(gameObject);
    }

    /// <summary>
    /// ���������� ��������� ������� ���������
    /// </summary>
    /// <param name="t">�����</param>
    /// <returns>��������� ���������</returns>
    public bool CalcMovement(float t)
    {
        if (aData.CalcMovAstBullet(gc.V2P(transform.up), t))
            return true;
        transform.position = gc.P2V(aData.pos);
        return false;
    }
}
