using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] private float orbitSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, orbitSpeed * Time.deltaTime);    
    }
}
