using UnityEngine;

public class UFOScript : MonoBehaviour
{
    public GameController gc; //�������� ����

    public ObjectData uData; //������ �������� �������
    public float speed = 1; //�������� �������
    public float hitRadius = .3f; //������ ���� �������� �������

    private void Start()
    {
        //������� �������
        uData = gc.gdm.Spawn(hitRadius, speed, false);
        transform.position = gc.P2V(uData.pos);

        //��������� ������� � ������
        gc.gdm.enemies.Add(uData);
        gc.ufosAdded.Add(gameObject);
    }

    /// <summary>
    /// ���������� ��������� ������� �������� �������
    /// </summary>
    /// <param name="playerPos">������� ������</param>
    public void CalcMovement(Vector3 playerPos)
    {
        uData.CalcMovUFO(gc.V2P(playerPos), Time.deltaTime);
        transform.position = transform.rotation * gc.P2V(uData.pos);
    }
}
