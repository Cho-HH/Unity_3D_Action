using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int curHealth;
    [SerializeField] private float detectRad;

    private Rigidbody rb;
    private BoxCollider boxCollider;
    private Material mat;
    private NavMeshAgent nav;
    [SerializeField] private Vector3 targetPos;
    private Animator anim;
    private bool isFindPlayer;
    private bool isArriveDest;
    private bool isCheck;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();        
        anim = GetComponentInChildren<Animator>();
        isArriveDest = true;
        isCheck = false;
        curHealth = maxHealth;
    }

    void Update()
    {             
        if (isFindPlayer)
        {
            CancelInvoke();
            anim.SetBool("isWalk", true);
            nav.SetDestination(targetPos);
        }
        else
        {
            MoveRandomPosition();
            CheckArriveDest();
        }
    }

    void FixedUpdate()
    {
        CheckWall();
        FindTarget();
        FreezeVelocity();    
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
            Debug.Log("Check Wall");
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
}
