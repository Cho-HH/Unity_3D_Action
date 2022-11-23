using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ETYPE
{
    HANDGUN_AMMO,
    MACHINEGUN_AMMO,
    COIN,
    GRENADE,
    HEART,
    WEAPON
};

public class Item : MonoBehaviour
{   
    [SerializeField] private ETYPE type;
    
    public ETYPE Type
    {
        get { return type; }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime); 
    }
}
