using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    [SerializeField] private int attackPower;
    [SerializeField] private int maxHealth;
    [SerializeField] private int curHealth;
    [SerializeField] private Transform[] firePos;
    [SerializeField] private GameObject missile;
    [SerializeField] private GameObject rock;
    [SerializeField] private int score;
    [SerializeField] private GameObject coin;

    private GameObject target;
    private NavMeshAgent nav;
    private BoxCollider boxCollider;
    private MeshRenderer[] meshes;
    private Vector3 tauntVec;
    private bool canLook;
    private Rigidbody rb;
    private Animator anim;
    private bool isAttacking;
    private bool isDie;
    public int AttackPower
    {
        get { return attackPower; }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    public int CurHealth
    {
        get { return curHealth; }
    }

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        nav = GetComponent<NavMeshAgent>();
        boxCollider = GetComponent<BoxCollider>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        canLook = true;
        isDie = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDie)
        {
            return;
        }

        if (canLook)
        {
            Quaternion LookRot = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, LookRot, 3.0f * Time.deltaTime);            
        }

        if (isAttacking)
        {
            return;
        }

        StartCoroutine(Attack());
    }

    void FixedUpdate()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.1f);
        canLook = false;

        int attackNum = Random.Range(0, 10);
        if (attackNum < 3)
        {
            //Shot
            StartCoroutine(ShotRoutine());
        }
        else if (attackNum < 6)
        {
            //Taunt
            StartCoroutine(TauntRoutine());
        }
        else
        {
            //BigShot
            StartCoroutine(BigShotRoutine());
        };        
    }

    IEnumerator ShotRoutine()
    {
        //attack
        yield return new WaitForSeconds(0.2f);
        anim.SetTrigger("doShot");
        foreach (Transform pos in firePos)
        {
            Instantiate(missile, pos.position, pos.rotation);
        }
        StartCoroutine(EndAttack());
    }

    IEnumerator BigShotRoutine()
    {
        yield return new WaitForSeconds(0.3f);
        anim.SetTrigger("doBigShot");
        Instantiate(rock, transform.position, transform.rotation);
        StartCoroutine(EndAttack());
    }

    IEnumerator TauntRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");

        nav.SetDestination(target.transform.position);
        StartCoroutine(EndAttack());
    }

    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(1.5f);
        boxCollider.enabled = true;

        yield return new WaitForSeconds(1.0f);
        canLook = true;

        yield return new WaitForSeconds(2.0f);
        isAttacking = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            MeleeWeapon weapon = other.GetComponent<MeleeWeapon>();
            curHealth -= weapon.Damage;            
            StartCoroutine(OnDamage());            
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.Damage;            
            StartCoroutine(OnDamage());            
        }
    }

    IEnumerator OnDamage()
    {
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.yellow;
        }
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.white;
            }            
        }
        else
        {
            isDie = true;
            anim.SetTrigger("doDie");
            GameManager.Instance.EnemyDCnt--;
            for (int i = 0; i < 6; i++)
            {
                Instantiate(coin, transform.position, Quaternion.identity);
            }
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.gray;
            }
            gameObject.layer = 10;

            Player pl = target.GetComponent<Player>();
            pl.CurScore += score;

            Destroy(gameObject, 4.0f);
        }
    }
    public void HitByGrenade()
    {
        curHealth -= 100;
        StartCoroutine(OnDamage());
    }

    public void SetHealth(int health)
    {
        maxHealth = health;
        curHealth = health;
    }
}
