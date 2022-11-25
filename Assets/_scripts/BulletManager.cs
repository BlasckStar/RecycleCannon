using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletManager : MonoBehaviour
{

    #region Bullet variables
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private BulletType typeToSpawn;
    [SerializeField]
    private Transform spawnPoint;
    #endregion

    #region Materials & Ui Variables
    [SerializeField]
    private TMPro.TextMeshProUGUI OrganicText;
    [SerializeField]
    private TMPro.TextMeshProUGUI MetalText;
    [SerializeField]
    private TMPro.TextMeshProUGUI PlasticText;
    [SerializeField]
    private Material[] materials;
    [SerializeField]
    private Image[] images;
    [SerializeField]
    private Material Selected;
    #endregion

    #region Data and control Variables
    [SerializeField]
    private bool canSpawn = true;
    [SerializeField]
    private float bulletCooldown = 0.5f;
    [SerializeField]
    private int amountToAdd = 5;
    [SerializeField]
    private int[] munition;
    [SerializeField]
    private int maxMunition = 30;
    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        UiUpdate();
        ImageUpdate();
    }
    #endregion

    #region Public methods
    public void SpawnBullet()
    {
        if (canSpawn)
        {
            int selectedMunition = 0;
            switch (typeToSpawn)
            {
                case BulletType.ORGANIC:
                    selectedMunition = 0;
                    break;
                case BulletType.METAL:
                    selectedMunition = 1;
                    break;
                case BulletType.PLASTIC:
                    selectedMunition = 2;
                    break;
            }
            if (munition[selectedMunition] > 0)
            {
                munition[selectedMunition]--;
                StartCoroutine(BulletCooldown());
            }
        }
    }

    public void SetBulletType(int type)
    {
        switch (type)
        {
            case 0:
                typeToSpawn = BulletType.ORGANIC;
                break;
            case 1:
                typeToSpawn = BulletType.METAL;
                break;
            case 2:
                typeToSpawn = BulletType.PLASTIC;
                break;
        }
    }


    public void AddMunition(BulletType type)
    {
        switch (type)
        {
            case BulletType.ORGANIC:
                munition[0] += amountToAdd;
                munition[0] = LimitCheck(munition[0]);
                break;
            case BulletType.METAL:
                munition[1] += amountToAdd;
                munition[1] = LimitCheck(munition[1]);
                break;
            case BulletType.PLASTIC:
                munition[2] += amountToAdd;
                munition[2] = LimitCheck(munition[2]);
                break;
        }

        UiUpdate();
    }

    public void UiUpdate()
    {
        TextUpdate();
    }

    public void TypeSelect(int i)
    {
        switch (i)
        {
            case 0:
                typeToSpawn = BulletType.ORGANIC;
                ImageUpdate();
                break;
            case 1:
                typeToSpawn = BulletType.METAL;
                ImageUpdate();
                break;
            case 2:
                typeToSpawn = BulletType.PLASTIC;
                ImageUpdate();
                break;
        }
    }

    #endregion

    #region Private methods
    int LimitCheck(int i)
    {
        if (i >= maxMunition)
        {

            return maxMunition;
        }
        else
        {
            return i;
        }

    }

    void TextUpdate()
    {
        OrganicText.text = "Organic: " + munition[0];
        MetalText.text = "Metal: " + munition[1];
        PlasticText.text = "Plastic: " + munition[2];
    }

    void ImageUpdate()
    {
        switch (typeToSpawn)
        {
            case BulletType.ORGANIC:
                images[0].color = Selected.color;
                images[1].color = materials[1].color;
                images[2].color = materials[2].color;
                break;
            case BulletType.METAL:
                images[0].color = materials[0].color;
                images[1].color = Selected.color;
                images[2].color = materials[2].color;
                break;
            case BulletType.PLASTIC:
                images[0].color = materials[0].color;
                images[1].color = materials[1].color;
                images[2].color = Selected.color;
                break;
        }
    }
    #endregion

    #region Corroutine Methods
    IEnumerator BulletCooldown()
    {
        canSpawn = false;
        var instBullet = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
        instBullet.GetComponent<Bullets>().SetType(typeToSpawn);
        UiUpdate();
        yield return new WaitForSeconds(bulletCooldown);
        canSpawn = true;
    }
    #endregion

}
