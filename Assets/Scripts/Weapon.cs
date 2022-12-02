using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected float rate;
    [SerializeField] private bool isGun;

    protected new AudioSource audio;
    public float Rate
    {
        get { return rate; }
    }

    public bool IsGun
    {
        get { return isGun; }
    }

    protected Animator anim;

    void Awake()
    {
        anim = GetComponentInParent<Animator>();
        audio = GetComponent<AudioSource>();
    }
    
    public abstract bool CanAttack();
}
