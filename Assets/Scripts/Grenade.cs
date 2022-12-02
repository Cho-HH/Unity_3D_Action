using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private GameObject particle;
    [SerializeField] private GameObject mesh;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Invoke("Explose", 3.0f);
    }

    void Explose()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        mesh.SetActive(false);
        particle.SetActive(true);

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 15.0f, Vector3.up, 0, LayerMask.GetMask("Enemy"));

        foreach (RaycastHit hit in hits)
        {
            hit.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        hits = Physics.SphereCastAll(transform.position, 15.0f, Vector3.up, 0, LayerMask.GetMask("Boss"));
        foreach (RaycastHit hit in hits)
        {
            hit.transform.GetComponent<Boss>().HitByGrenade();
        }

        Destroy(gameObject, 5.0f);
    }
}
