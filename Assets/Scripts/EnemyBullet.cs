using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private int damage;

    public int Damage
    {
        get { return damage; }
    }

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {        
        rb.MovePosition(rb.position + transform.forward * 20.0f * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Floor" || other.tag == "Wall" || other.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
