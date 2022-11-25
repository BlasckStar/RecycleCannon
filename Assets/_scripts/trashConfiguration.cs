using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trashConfiguration : MonoBehaviour
{
    [SerializeField]
    private BulletType type;
    [SerializeField]
    private Material[] materials;

    private void Awake()
    {
        RandomType();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Trash"))
        {
            CollectablesManager manager = FindObjectOfType<CollectablesManager>();
            manager.SpawnCollectable();
            Destroy(this.gameObject);
        } 
    }

    private void OnTriggerStay(Collider other)
    {
        if (GetComponent<Collider>().CompareTag("Trash"))
        {
            CollectablesManager manager = FindObjectOfType<CollectablesManager>();
            manager.SpawnCollectable();
            Destroy(this.gameObject);
        }
    }

    void RandomType()
    {
        int randomInt = Random.Range(1, 4);
        Debug.Log(""+randomInt);
        SetType(randomInt);
    }
    
    public void SetType(int _type)
    {
        Material material = this.GetComponent<MeshRenderer>().material;
        switch (_type)
        {
            case 1:
                type = BulletType.ORGANIC;
                material.color = materials[0].color;
                break;
            case 2:
                type = BulletType.METAL;
                material.color = materials[1].color;
                break;
            case 3:
                type = BulletType.PLASTIC;
                material.color = materials[2].color;
                break;
        }
    }

    public BulletType ReturnType()
    {
        return type;
    }
}
