using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ETYPE
    {
        AMMO,
        COIN,
        GRENADE,
        HEART,
        WEAPON
    };

    [SerializeField] private ETYPE type;
    
    public ETYPE Type
    {
        get { return type; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime); 
    }
}
