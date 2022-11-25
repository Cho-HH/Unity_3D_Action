using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int curHealth;

    private Rigidbody rb;
    private BoxCollider boxCollider;
    private Material mat;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
        curHealth = maxHealth;
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
