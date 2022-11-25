using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WallConfigurations : MonoBehaviour
{
    [SerializeField]
    private int life = 20;
    [SerializeField]
    private TextMeshProUGUI wallText;

    public void takeDamage(int damage)
    {
        life -= damage;
        if(life <= 0)
        {
            Destroy(this.gameObject);
        }
        UpdateUi();
    }

    void UpdateUi()
    {
        wallText.text = "Wall life: " + life;
    }
}
