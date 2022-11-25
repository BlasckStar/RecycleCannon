using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    public float speed = 15f;
    public BulletType type;
    public Material[] materials;
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    public void SetType(BulletType _type)
    {
        Material material = this.GetComponent<MeshRenderer>().material;
        switch (_type)
        {
            case BulletType.ORGANIC:
                type = BulletType.ORGANIC;
                material.color = materials[0].color;
                break;
            case BulletType.METAL:
                type = BulletType.METAL;
                material.color = materials[1].color;
                break;
            case BulletType.PLASTIC:
                type = BulletType.PLASTIC;
                material.color = materials[2].color;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "out":
                Destroy(this.gameObject);
                break;
            case "Enemy":
                var enemy = other.GetComponent<EnemyConfigurations>();
                switch (type)
                {
                    case BulletType.ORGANIC:
                        if(enemy.ReturnType() == BulletType.METAL || enemy.ReturnType() == BulletType.PLASTIC)
                        {
                            enemy.TakeDamage();
                        }
                        break;
                    case BulletType.PLASTIC:
                        if(enemy.ReturnType() == BulletType.ORGANIC)
                        {
                            enemy.TakeDamage();
                        }
                        break;
                    case BulletType.METAL:
                        if (enemy.ReturnType() == BulletType.ORGANIC)
                        {
                            enemy.TakeDamage();
                        }
                        break;
                }
                Destroy(this.gameObject);
                break;
        }
    }

}
