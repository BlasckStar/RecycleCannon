using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectablesManager : MonoBehaviour
{
    #region Manager Variables
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private GameObject floor;
    [SerializeField]
    private float dropCooldown = 5f;
    [SerializeField]
    private int initialCollectables = 5;

    private Bounds bnbFloor;
    #endregion

    private void Start()
    {
        bnbFloor = floor.GetComponent<Renderer>().bounds;
        SpawnCollectablesByNumbers(initialCollectables);

        StartCoroutine(InfinitySpawn());
    }

    Vector3 GeneratedPosition(Bounds bounds)
    {
        float x, y, z;
        x = Random.Range(bounds.min.x, bounds.max.x) ;
        y = 2;
        z = Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, y, z);
    }
    IEnumerator InfinitySpawn()
    {
        SpawnCollectable();
        yield return new WaitForSeconds(dropCooldown);
        StartCoroutine(InfinitySpawn());
    }

    #region Public Methods
    public void SpawnCollectable()
    {
        Vector3 randomPosition = GeneratedPosition(bnbFloor);
        randomPosition.y = 1;
        Instantiate(prefab, randomPosition, Quaternion.identity);
    }

    public void SpawnCollectablesByNumbers(int times)
    {
        for (int i = 0; i < times; i++)
        {
            SpawnCollectable();
        }

    }
    public void SpawnCollectableByType(int _type)
    {
        Vector3 randomPosition = GeneratedPosition(bnbFloor);
        randomPosition.y = 1;
        var instCollectable = Instantiate(prefab, randomPosition, Quaternion.identity);
        instCollectable.GetComponent<trashConfiguration>().SetType(_type);
    }

    public void SpawnCollectableByType(int _type, Transform targetTransform)
    {
        Vector3 spawnPosition = targetTransform.position;
        spawnPosition.y = 1;
        var instCollectable = Instantiate(prefab, spawnPosition, Quaternion.identity); 
        instCollectable.GetComponent<trashConfiguration>().SetType(_type);
    }

    #endregion
}
