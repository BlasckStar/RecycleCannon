using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyConfigurations : MonoBehaviour
{
    private enum State
    {
        WALKING,
        ATTACKING,
        DAMAGED
    }

    #region Enemy Variables
    [SerializeField]
    private int life = 3;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private BulletType type;
    #endregion

    #region Material & managers (Isso não devia estar nesse script!)
    [SerializeField]
    private Material[] materials;
    private PlayerTouchMovement player;
    private WallConfigurations wall;
    private CollectablesManager collectablesManager;
    private EnemyManager enemyManager;
    #endregion

    #region Data & Control variables
    private State state;
    private bool isPlayerInRange = false;
    private bool isWallInRange = false;
    private State lastState;
    private bool isDamaged = false;
    private bool isAttacking = false;
    private float damagedCooldown = 1f;
    private float attackCooldown = 1.5f;
    private int damageCount = 0;
    #endregion

    #region Monobehaviour Methods

    void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        collectablesManager = FindObjectOfType<CollectablesManager>();
        player = FindObjectOfType<PlayerTouchMovement>();
        wall = FindObjectOfType<WallConfigurations>();
        SetState(State.WALKING);
        RandomType();
        
    }

    void Update()
    {
        switch (state)
        {
            case State.WALKING:
                WalkingState();
                break;
            case State.ATTACKING:
                AttackingState();
                break;
            case State.DAMAGED:
                DamagedState();
                break;
        }
    }
    #endregion

    #region Listenersmethods
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Wall":
                isWallInRange = true;
                SetState(State.ATTACKING);
                break;
            case "Player":
                isPlayerInRange = true;
                SetState(State.ATTACKING);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Wall":
                isWallInRange = false;
                SetState(State.WALKING);
                break;
            case "Player":
                isPlayerInRange = false;
                SetState(State.WALKING);
                break;
        }
    }
    #endregion

    #region Private Methods

    void WalkingState()
    {
        if (!isDamaged)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    void AttackingState()
    {
        if (!isDamaged)
        {
            IsAnythingInRange();
            if(!isAttacking)
            StartCoroutine(AttackStateRotine());
        }
    }

    void DamagedState()
    {
        if (!isDamaged)
        {
            DamagedAction();
        }
    }

    void SetState(State _state)
    {
        lastState = state;
        state = _state;
    }

    void DealDamage()
    {
        if (isWallInRange)
        {
            wall.takeDamage(damage);
        }
        if (isPlayerInRange)
        {
            player.TakeDamage();
        }
    }

    void DamagedAction()
    {
        StartCoroutine(DamageStateRotine());
    }

    void IsAnythingInRange()
    {
        if (!isPlayerInRange && !isWallInRange)
        {
            SetState(State.WALKING);
        }
    }

    void RandomType()
    {
        int randomInt = Random.Range(1, 4);
        SetType(randomInt);
    }

    void SpawnTrash()
    {
        switch (type)
        {
            case BulletType.ORGANIC:
                collectablesManager.SpawnCollectableByType(1, transform);
                break;
            case BulletType.METAL:
                collectablesManager.SpawnCollectableByType(2, transform);
                break;
            case BulletType.PLASTIC:
                collectablesManager.SpawnCollectableByType(3, transform);
                break;
        }
    }
    void SpawnRandomTrash()
    {
        collectablesManager.SpawnCollectableByType(Random.Range(1, 4), transform);
    }

    #endregion

    #region Corroutine Methods
    IEnumerator DamageStateRotine()
    {
        isDamaged = true;
        DamagedAction();
        yield return new WaitForSeconds(damagedCooldown);
        if(state == State.DAMAGED)
        {
            SetState(lastState);
        }
        isDamaged = false;
    }

    IEnumerator AttackStateRotine()
    {
        isAttacking = true;
        DealDamage();
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
    #endregion

    #region Public Methods
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

    public void TakeDamage()
    {
        life--;
        if(damageCount >= 3)
        {
            SpawnRandomTrash();
            damageCount = 0;
        }
        else
        {
            damageCount++;
        }
        if (life <= 0)
        {
            SpawnTrash();
            enemyManager.Enemydie();
            Destroy(this.gameObject);
        }

    }

    public void CriticalDamage()
    {
        life--;
        if(!isDamaged)
            SetState(State.DAMAGED);
    }

    public BulletType ReturnType()
    {
        return type;
    }

    public void EnemyModifiers(int _speed, int _life, int _damage)
    {
        speed = _speed;
        life = _life;
        damage = _damage;
    }
    #endregion

}
