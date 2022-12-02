using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private int attackPower;
    [SerializeField] private int maxHealth;
    [SerializeField] private int curHealth;
    [SerializeField] private float detectRad;
    [SerializeField] private float attackRad;
    [SerializeField] private float attackRange;
    [SerializeField] private int score;
    [SerializeField] private GameObject coin;

    private Material mat;   
    private Vector3 targetPos;
    private bool isArriveDest;
    private bool isCheck;
    private GameObject player; 

    protected bool isFindPlayer;
    protected NavMeshAgent nav;
    protected Rigidbody rb;
    protected Animator anim;
    protected bool isAttack;
    protected bool isAttacking;

    public int AttackPower
    {
        get { return attackPower; }
    }

    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();        
        anim = GetComponentInChildren<Animator>();        
        isArriveDest = true;
        isCheck = false;
        isAttack = false;
        isAttacking = false;
        curHealth = maxHealth;
    }

    void Update()
    {             
        if (nav.enabled)
        {
            if (isAttack)
            {
                Attack();
            }
            else
            {
                if (isFindPlayer)
                {
                    ChasePlayer();
                }
                else
                {
                    MoveRandomPosition();
                    CheckArriveDest();
                }
            }
        }
    }

    void FixedUpdate()
    {
        CheckWall();
        FindTarget();
        FreezeVelocity();
    }
    
    public virtual void ChasePlayer()
    {
        CancelInvoke();
        anim.SetBool("isWalk", true);
        nav.SetDestination(targetPos);

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, attackRad, transform.forward, attackRange, LayerMask.GetMask("Character"));
        if (hits.Length > 0)
        {
            isAttack = true;
        }
    }

    void FindTarget()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectRad, Vector3.up, 0, LayerMask.GetMask("Character"));        
        if (hits.Length > 0)
        {
            targetPos = hits[0].transform.position;
            isArriveDest = true;
            isFindPlayer = true;
            isCheck = false;
            return;
        }
        hits = Physics.SphereCastAll(transform.position, detectRad, Vector3.up, 0, LayerMask.GetMask("PlayerDamaged"));
        if (hits.Length > 0)
        {
            targetPos = hits[0].transform.position;
                isArriveDest = true;
                isFindPlayer = true;
            isCheck = false;
            return;
        }
        isFindPlayer = false;        
    }

    void MoveRandomPosition()
    {
        if (!isArriveDest)
        {
            return;
        }

        Vector3 randomPos = transform.position + Random.insideUnitSphere * detectRad;
        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, detectRad, NavMesh.AllAreas))
        {
            targetPos = hit.position;
        }
        anim.SetBool("isWalk", true);
        nav.SetDestination(targetPos);
        isArriveDest = false;        
    }

    void CheckArriveDest()
    {
        if (isCheck)
        {
            return;
        }

        if (Vector3.Distance(transform.position, targetPos) < Vector3.kEpsilon)
        {
            anim.SetBool("isWalk", false);
            isCheck = true;
            Invoke("ArriveDest", 2.0f);
        }
    } 

    void ArriveDest()
    {
        isArriveDest = true;
        isCheck = false;
    }

    void CheckWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5.0f, Color.blue);
        if (Physics.Raycast(transform.position, transform.forward, 5.0f, LayerMask.GetMask("Wall")))
        {
            isArriveDest = true;
            isCheck = false;
        }
    }

    void FreezeVelocity()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;        
    }

    void OnTriggerEnter(Collider other)
    {        
        if (other.tag == "Melee")
        { 
            MeleeWeapon weapon = other.GetComponent<MeleeWeapon>();
            curHealth -= weapon.Damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
            Debug.Log("Melee : " + curHealth);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.Damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
            Debug.Log("Bullet : " + curHealth);
        }
    }
    public void HitByGrenade(Vector3 grenadePos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - grenadePos;
        StartCoroutine(OnDamage(reactVec, true));
    }
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            anim.SetTrigger("doDie");
            nav.enabled = false;
            mat.color = Color.gray;
            gameObject.layer = 10;
            Player pl = player.GetComponent<Player>();
            pl.CurScore += score;
            Instantiate(coin, transform.position, Quaternion.identity);
            decreaseCnt();
            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3.0f;
                rb.freezeRotation = false;

                rb.AddForce(reactVec * 10.0f, ForceMode.Impulse);
                rb.AddTorque(reactVec * 15.0f, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rb.AddForce(reactVec * 10.0f, ForceMode.Impulse);
            }           
            Destroy(gameObject, 4.0f);
        }
    }

    public abstract void decreaseCnt();
    public virtual void Attack()
    {
        isAttacking = true;
        anim.SetBool("isAttack", true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRad);

        Gizmos.color = Color.blue;        
        Gizmos.DrawWireSphere(transform.position + transform.forward * attackRange, attackRad);
    }
}
