using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameDataManager gdm = new GameDataManager(); //�������� ������, � ������� ���������� �������

    public GameObject asteroid; //������� ������ ���������
    public GameObject ufo; //������� ������ �������� �������

    public List<GameObject> projs = new List<GameObject>(); //������ ����������� ����
    public List<GameObject> astAdded = new List<GameObject>(); //������ ����������� ����������
    public List<GameObject> ufosAdded = new List<GameObject>(); //������ ����������� �������

    public float spawnRateAst = 1f; //���� �������� ����������
    public int spawnAmountAst = 1; //���-�� ���������� � "�����"
    public int astParts = 2; //���-�� �������� ���������

    public float spawnRateUFO = 1f; //���� �������� �������
    public int spawnAmountUFO = 1; //���������� ������� � "�����"

    public GameObject player; //������� ������ ������� ������

    public Text txtCoords; //���������� ������
    public Text txtRot; //���� �������� ������
    public Text txtSpeed; //�������� ������
    public Text txtLasChrg; //���-�� ������� ������
    public Text txtLasCD; //����� ����������� ������

    public GameObject gameoverPanel; //UI ������� � ����������� � ����� ����
    public Text score; //���� ������


    // Start is called before the first frame update
    private void Start()
    {
        //��������� �������� ���������� � �������� ������� � ���������� �����������
        InvokeRepeating(nameof(SpawnAst), spawnRateAst, spawnRateAst);
        InvokeRepeating(nameof(SpawnUFO), spawnRateUFO, spawnRateUFO);
        //���������� ������� ������ � �����������
        gdm.screenHeight = Camera.main.orthographicSize;
        gdm.screenWidth = gdm.screenHeight * Screen.width / Screen.height;
    }

    // Update is called once per frame
    private void Update()
    {
        //������� ���� ��������� ���� ������� ������ "���"
        if (player != null)
        {
            SpawnSmallAst(); //�������� �������� ���������
            gdm.CheckCollisions(); //�������� �������� �� ��������
            Delete(); //�������� ���������� ��������
            //��������� ���� ���� ������� ����
            if (gdm.playerDead)
                EndGame();
            //�������� ������� ��������
            UpdatePositions();
        }
        else
            CancelInvoke();
    }

    /// <summary>
    /// �������� ����������
    /// </summary>
    private void SpawnAst()
    {
        for (int i = 0; i < spawnAmountAst; i++)
        {
            AsteroidScript ast = Instantiate(asteroid, Vector3.zero, Quaternion.identity).GetComponent<AsteroidScript>();
            ast.gc = this;
            ast.CreateAsteroid();
        }
    }

    /// <summary>
    /// �������� �������� �������
    /// </summary>
    private void SpawnUFO()
    {
        if (player != null)
        {
            for (int i = 0; i < spawnAmountAst; i++)
            {
                UFOScript uf = Instantiate(ufo, Vector3.zero, Quaternion.identity).GetComponent<UFOScript>();
                uf.gc = this;
            }
        }
    }

    /// <summary>
    /// �������� ������ ������������� ���������
    /// </summary>
    private void SpawnSmallAst()
    {
        foreach (ObjectData od in gdm.smallAsteroids)
        {
            for (int i = 0; i < astParts; i++)
            {
                //������ ������� ��������� � ��������� ��� ObjectData, � ������� �������� ��� ���������
                AsteroidScript ast = Instantiate(asteroid, Vector3.zero, Quaternion.identity).GetComponent<AsteroidScript>();
                ast.gc = this;
                ast.CreateSmallAsteroid(od);
            }
        }
        gdm.smallAsteroids.Clear();
    }

    /// <summary>
    /// �������� ���������� ��������
    /// </summary>
    private void Delete()
    {
        //�������� ����
        while (gdm.projectilesToRemove.Count > 0)
        {
            int it = 0;
            while (it < projs.Count)
            {
                ObjectData bD = projs[it].GetComponent<BulletScript>().bData;
                if (gdm.projectilesToRemove[0].Equals(bD))
                {
                    gdm.projectiles.Remove(bD);
                    gdm.projectilesToRemove.Remove(bD);
                    Destroy(projs[it]);
                    projs.RemoveAt(it);
                    break;
                }
                it++;
            }
        }

        //�������� ���������� � �������� �������
        while (gdm.enemiesToRemove.Count > 0)
        {
            int it = 0;
            //�������
            while (it < ufosAdded.Count)
            {
                ObjectData uD = ufosAdded[it].GetComponent<UFOScript>().uData;
                if (gdm.enemiesToRemove.Count > 0 && gdm.enemiesToRemove[0].Equals(uD))
                {
                    gdm.enemies.Remove(uD);
                    gdm.enemiesToRemove.Remove(uD);
                    Destroy(ufosAdded[it]);
                    ufosAdded.RemoveAt(it);
                    break;
                }
                it++;
            }
            it = 0;
            //���������
            while (it < astAdded.Count)
            {
                ObjectData aD = astAdded[it].GetComponent<AsteroidScript>().aData;
                if (gdm.enemiesToRemove.Count > 0 && gdm.enemiesToRemove[0].Equals(aD))
                {
                    gdm.enemies.Remove(aD);
                    gdm.enemiesToRemove.Remove(aD);
                    Destroy(astAdded[it]);
                    astAdded.RemoveAt(it);
                    break;
                }
                it++;
            }
        }
    }

    /// <summary>
    /// ���������� ������� ��������
    /// </summary>
    private void UpdatePositions()
    {
        //����
        foreach (GameObject go in projs)
        {
            if (go.GetComponent<BulletScript>().CalcMovement(Time.deltaTime))
                gdm.projectilesToRemove.Add(go.GetComponent<BulletScript>().bData);
        }
        //���������
        foreach (GameObject go in astAdded)
        {
            if (go.GetComponent<AsteroidScript>().CalcMovement(Time.deltaTime))
                gdm.enemiesToRemove.Add(go.GetComponent<AsteroidScript>().aData);
        }
        //�������� �������
        foreach (GameObject go in ufosAdded)
        {
            if (player != null)
            {
                go.GetComponent<UFOScript>().CalcMovement(player.transform.position);
            }
        }
    }

    /// <summary>
    /// ���������� ����
    /// </summary>
    private void EndGame()
    {
        if (player != null)
        {
            Destroy(player);
            gameoverPanel.SetActive(true);
            score.text = gdm.score.ToString();
        }
    }

    /// <summary>
    /// ���������� ����
    /// </summary>
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// �������� Vector3 �� Point2
    /// </summary>
    /// <param name="p">�����</param>
    /// <returns>������</returns>
    public Vector3 P2V(Point2 p)
    {
        return new Vector3((float)p.x, (float)p.y);
    }

    /// <summary>
    /// �������� Point2 �� Vector3
    /// </summary>
    /// <param name="v">������</param>
    /// <returns>�����</returns>
    public Point2 V2P(Vector3 v)
    {
        return new Point2(v.x, v.y);
    }
}
