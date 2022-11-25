using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    #region Objects variables

    [SerializeField]
    public GameObject prefab;
    [SerializeField]
    private GameObject floor;
    [SerializeField]
    private Bounds bnbFloor;

    #endregion

    #region Data and Control variables
    [SerializeField]
    private int waves = 3;
    [SerializeField]
    private int waveActual = 0;
    [SerializeField]
    private int wavesEnemysSpawn = 5;
    [SerializeField]
    private int wavesEnemysActual;
    [SerializeField]
    private int[] bossStats = new int[3];
    #endregion

    #region UI Variables
    [SerializeField]
    private TextMeshProUGUI waveText;
    #endregion

    #region Monobehaviour Methods
    void Start()
    {
        bnbFloor = floor.GetComponent<Renderer>().bounds;
        SpawnWave();
        UpdateUi();
    }

    #endregion

    #region Public methods

    public void SpawnEnemyByType(int _type)
    {
        Vector3 randomPosition = GeneratedPosition(bnbFloor);
        randomPosition.y = 1;
        var instCollectable = Instantiate(prefab, randomPosition, Quaternion.identity);
        instCollectable.GetComponent<trashConfiguration>().SetType(_type);
        wavesEnemysActual++;
    }
    public void SpawnEnemyByRandomType()
    {
        Vector3 randomPosition = GeneratedPosition(bnbFloor);
        randomPosition.y = 2.5f;
        var instCollectable = Instantiate(prefab, randomPosition, Quaternion.Euler(0, -90, 0));
        instCollectable.GetComponent<EnemyConfigurations>().SetType(BetterRandomType());
        wavesEnemysActual++;
    }

    public void Enemydie()
    {
        wavesEnemysActual--;
        CheckWaveEnds();
    }

    #endregion

    #region Private Methods
    Vector3 GeneratedPosition(Bounds bounds)
    {
        float x, y, z;
        x = Random.Range(bounds.min.x, bounds.max.x);
        y = 2;
        z = Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, y, z);
    }


    int BetterRandomType()
    {
        int teste = Random.Range(0, 100);
        if (teste < 50)
        {
            teste = 1;
        }
        else
        if (teste >= 70 && teste < 85)
        {
            teste = 2;
        }
        else
        {
            teste = 3;
        }
        return teste;
    }

    void CheckWaveEnds()
    {
        if (wavesEnemysActual <= 0)
        {
            waveActual++;
            if (waveActual == waves)
            {
                SpawnBoss();
            }
            else
            {
                SpawnWave();
            }
        }
        UpdateUi();
    }

    void SpawnWave()
    {
        Debug.Log("Enemys to spawn: " + wavesEnemysSpawn);
        for (int i = 0; i < wavesEnemysSpawn; i++)
        {
            SpawnEnemyByRandomType();
        }
    }

    void SpawnBoss()
    {
        Vector3 randomPosition = GeneratedPosition(bnbFloor);
        randomPosition.y = 1;
        var instEnemy = Instantiate(prefab, randomPosition, Quaternion.identity);
        instEnemy.GetComponent<EnemyConfigurations>().SetType(BetterRandomType());
        instEnemy.GetComponent<Transform>().Rotate(new Vector3(0, -90, 0));
        instEnemy.GetComponent<EnemyConfigurations>().EnemyModifiers(bossStats[0], bossStats[1], bossStats[2]);
    }

    void UpdateUi()
    {
        if (waveActual != waves)
        {
            waveText.text = "Wave: " + waveActual;
        }
        else
        {
            waveText.text = "Wave: BOSS";
        }
    }

    #endregion

}
