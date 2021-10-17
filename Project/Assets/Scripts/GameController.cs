using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameDataManager gdm = new GameDataManager(); //менеджер данных, в котором происходят расчёты

    public GameObject asteroid; //игровой объект астероида
    public GameObject ufo; //игровой объект летающей тарелки

    public List<GameObject> projs = new List<GameObject>(); //список добавленных пуль
    public List<GameObject> astAdded = new List<GameObject>(); //список добавленных астероидов
    public List<GameObject> ufosAdded = new List<GameObject>(); //список добавленных тарелок

    public float spawnRateAst = 1f; //темп создания астероидов
    public int spawnAmountAst = 1; //кол-во астероидов в "волне"
    public int astParts = 2; //кол-во осколков астероида

    public float spawnRateUFO = 1f; //темп создания тарелок
    public int spawnAmountUFO = 1; //количество тарелок в "волне"

    public GameObject player; //игровой объект корабля игрока

    public Text txtCoords; //координаты игрока
    public Text txtRot; //угол поворота игрока
    public Text txtSpeed; //скорость игрока
    public Text txtLasChrg; //кол-во зарядов лазера
    public Text txtLasCD; //время перезарядки лазера

    public GameObject gameoverPanel; //UI область с информацией о конце игры
    public Text score; //счёт игрока


    // Start is called before the first frame update
    private void Start()
    {
        //запускаем создание астероидов и летающих тарелок с указанными параметрами
        InvokeRepeating(nameof(SpawnAst), spawnRateAst, spawnRateAst);
        InvokeRepeating(nameof(SpawnUFO), spawnRateUFO, spawnRateUFO);
        //определяем размеры экрана в координатах
        gdm.screenHeight = Camera.main.orthographicSize;
        gdm.screenWidth = gdm.screenHeight * Screen.width / Screen.height;
    }

    // Update is called once per frame
    private void Update()
    {
        //игровой цикл действует пока корабль игрока "жив"
        if (player != null)
        {
            SpawnSmallAst(); //создание осколков астероида
            gdm.CheckCollisions(); //проверка объектов на коллизию
            Delete(); //удаление помеченных объектов
            //завершить игру если корабль сбит
            if (gdm.playerDead)
                EndGame();
            //обновить позиции объектов
            UpdatePositions();
        }
        else
            CancelInvoke();
    }

    /// <summary>
    /// Создание астероидов
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
    /// Создание летающих тарелок
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
    /// Создание частей уничтоженного астероида
    /// </summary>
    private void SpawnSmallAst()
    {
        foreach (ObjectData od in gdm.smallAsteroids)
        {
            for (int i = 0; i < astParts; i++)
            {
                //создаём осколок астероида и назначаем ему ObjectData, в котором хранятся его параметры
                AsteroidScript ast = Instantiate(asteroid, Vector3.zero, Quaternion.identity).GetComponent<AsteroidScript>();
                ast.gc = this;
                ast.CreateSmallAsteroid(od);
            }
        }
        gdm.smallAsteroids.Clear();
    }

    /// <summary>
    /// Удаление помеченных объектов
    /// </summary>
    private void Delete()
    {
        //удаление пуль
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

        //удаление астероидов и летающих тарелок
        while (gdm.enemiesToRemove.Count > 0)
        {
            int it = 0;
            //тарелки
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
            //астероиды
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
    /// Обновление позиций объектов
    /// </summary>
    private void UpdatePositions()
    {
        //пули
        foreach (GameObject go in projs)
        {
            if (go.GetComponent<BulletScript>().CalcMovement(Time.deltaTime))
                gdm.projectilesToRemove.Add(go.GetComponent<BulletScript>().bData);
        }
        //астероиды
        foreach (GameObject go in astAdded)
        {
            if (go.GetComponent<AsteroidScript>().CalcMovement(Time.deltaTime))
                gdm.enemiesToRemove.Add(go.GetComponent<AsteroidScript>().aData);
        }
        //летающие тарелки
        foreach (GameObject go in ufosAdded)
        {
            if (player != null)
            {
                go.GetComponent<UFOScript>().CalcMovement(player.transform.position);
            }
        }
    }

    /// <summary>
    /// Завершение игры
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
    /// Перезапуск игры
    /// </summary>
    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Создание Vector3 из Point2
    /// </summary>
    /// <param name="p">точка</param>
    /// <returns>вектор</returns>
    public Vector3 P2V(Point2 p)
    {
        return new Vector3((float)p.x, (float)p.y);
    }

    /// <summary>
    /// Создание Point2 из Vector3
    /// </summary>
    /// <param name="v">вектор</param>
    /// <returns>точка</returns>
    public Point2 V2P(Vector3 v)
    {
        return new Point2(v.x, v.y);
    }
}
