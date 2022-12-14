using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float jumpPower;

    [SerializeField] private GameObject[] weapons;
    [SerializeField] private GameObject[] grenades;

    [SerializeField] private GameObject throwGrenade;

    [SerializeField] private int totalHandGunAmmo;
    [SerializeField] private int totalMachineGunAmmo;
    [SerializeField] private int coin;
    [SerializeField] private int health;
    [SerializeField] private int hasGrenades;

    [SerializeField] private int maxHasGrenades;

    [SerializeField] private Text pressTxt;
    private new AudioSource audio;

    private List<GameObject> weaponList;
    private Dictionary<string, bool> weaponStates;

    private Animator anim;
    private Rigidbody rigid;

    private float x;
    private float z;
    private bool isWalk;
    private bool isJump;
    private bool isDie;
    private bool iDown;
    private bool s1Down;
    private bool s2Down;
    private bool s3Down;
    private bool isSwap;
    private bool fDown;
    private bool rDown; // reload
    private bool gDown; // grenade

    private Vector3 moveVec;
    private float walkSpeed;

    private float fireDelay;
    private bool isFireReady;
    private bool isAttacking;
    private bool isReloading;
    private bool isShop;
    private bool isBorder;
    private int curScore;

    private Weapon curEquipWeapon;
    private GameObject nearObj;
    private MeshRenderer[] meshes;

    public int Coin
    {
        get { return coin; }
        set { coin = value; }
    }

    public int Health
    {
        get { return health; }
    }

    public int TotalHandGunAmmo
    {
        get { return totalHandGunAmmo; }
    }

    public int TotalMachinegunAmmo
    {
        get { return totalMachineGunAmmo; }
    }

    public int HasGrenades
    {
        get { return hasGrenades; }
    }

    public List<GameObject> WeaponList
    {
        get { return weaponList; }
    }

    public int CurScore
    {
        get { return curScore; }
        set { curScore = value; }
    }

    void Awake()
    {        
        isSwap = false;
        weaponList = new List<GameObject>();
        weaponStates = new Dictionary<string, bool>();
        foreach (GameObject weapon in weapons)
        {
            weaponStates[weapon.name] = false;
        }

        walkSpeed = speed;
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        audio = GetComponent<AudioSource>();
        PlayerPrefs.SetInt("MaxScore", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isSwap || isAttacking || isReloading || isDie)
        {
            return;
        }
        GetInput();
        Turn();
        Jump();
        Interaction();
        SwapWeapon();
        Attack();
        Reload();
        Grenade();

        moveVec = new Vector3(x, 0, z).normalized;

        speed = isWalk ? walkSpeed : 20.0f;

        anim.SetBool("isWalk", moveVec != Vector3.zero && isWalk);
        anim.SetBool("isRun", moveVec != Vector3.zero && !isWalk);       
    }

    void FixedUpdate()
    {
        if (isDie)
        {
            return;
        }

        StopToWall();
        if (isSwap || isAttacking || isReloading)
        {
            return;
        }
        Move();        

        rigid.angularVelocity = Vector3.zero;
    }

    void GetInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
        iDown = Input.GetButtonDown("Interaction");
        s1Down = Input.GetButtonDown("Swap1");
        s2Down = Input.GetButtonDown("Swap2");
        s3Down = Input.GetButtonDown("Swap3");
        fDown = Input.GetButton("Fire1");
        rDown = Input.GetButtonDown("Reload");
        gDown = Input.GetButtonDown("Fire2");

        isWalk = Input.GetButton("Walk");
    }
    void Move()
    {       
        if (!isBorder)
        {
            rigid.MovePosition(rigid.position + moveVec * speed * Time.deltaTime);
        }
    }
    void Turn()
    {
        //??????       
        if (fDown && curEquipWeapon is GunWeapon && !isJump && moveVec == Vector3.zero)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
            {
                Vector3 nextVec = hit.point - transform.position;
                nextVec.y = 0;
                transform.rotation = Quaternion.LookRotation(nextVec);
            }
        }

        //??????
        if (x == 0 && z == 0 || moveVec == Vector3.zero)
        {
            return;
        }
        Quaternion newRot = Quaternion.LookRotation(moveVec);
        rigid.MoveRotation(Quaternion.Slerp(rigid.rotation, newRot, turnSpeed * Time.deltaTime));
    }

    void Jump()
    {
        anim.SetBool("isJump", isJump);
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJump = true;
        }
    }

    void Interaction()
    {
        if (iDown && nearObj != null && !isJump)
        {            
            if (nearObj.tag == "Weapon")
            {
                string[] wp = nearObj.name.Split('(');
                if (weaponStates[wp[0]] == true)
                {
                    return;
                }
                pressTxt.enabled = false;
                Item item = nearObj.GetComponent<Item>();     
                foreach (string weaponName in weaponStates.Keys)
                {
                    if (weaponName + "(Clone)" == nearObj.name)
                    {
                        weaponStates[weaponName] = true;
                        foreach (GameObject weapon in weapons)
                        {
                            if (weapon.name == weaponName)
                            {
                                weaponList.Add(weapon);
                                audio.Play();
                                break;
                            }
                        }
                        break;
                    }
                }                
                Destroy(nearObj);
            }
            else if (nearObj.tag == "Shop")
            {
                pressTxt.enabled = false;
                isShop = true;
                Shop shop = nearObj.GetComponent<Shop>();
                shop.Enter(this);
            }
            
        }
    }
    void SwapWeapon()
    {               
        if (isSwap || isJump || isReloading)
        {
            return;
        }

        if ((s1Down || s2Down || s3Down) && !isJump)
        {           
            int weaponIdx = -1;
            if (s1Down)
            {
                weaponIdx = 0;

            }
            else if (s2Down)
            {
                weaponIdx = 1;
            }
            else if (s3Down)
            {
                weaponIdx = 2;
            }

            if (weaponIdx >= weaponList.Count)
            {
                return;
            }

            if (curEquipWeapon != null)
            {
                if (curEquipWeapon.gameObject == weaponList[weaponIdx])
                {
                    return;
                }
                curEquipWeapon.gameObject.SetActive(false);
            }

            if (weaponStates[weaponList[weaponIdx].name])
            {
                anim.SetTrigger("doSwap");
                curEquipWeapon = weaponList[weaponIdx].GetComponent<Weapon>();
                curEquipWeapon.gameObject.SetActive(true);
                isSwap = true;
                Invoke("SwapOut", 0.5f);
            }
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }
    void Attack()
    {
        if (curEquipWeapon == null || isAttacking || isShop)
        {
            return;
        }

        fireDelay += Time.deltaTime;
        Weapon curWeapon = curEquipWeapon.GetComponent<Weapon>();
        isFireReady = curWeapon.Rate < fireDelay;

        if (fDown && !isSwap && isFireReady && !isJump && !isReloading)
        {
            if (curWeapon.CanAttack())
            {
                isAttacking = true;
                fireDelay = 0;
                Invoke("AttackOut", curWeapon.Rate + 0.2f);
            }
        }
    }

    void AttackOut()
    {
        isAttacking = false;
    }

    void Grenade()
    {
        if (hasGrenades == 0)
        {
            return;
        }

        if (gDown && !isReloading && !isSwap)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
            {
                Vector3 throwVec = hit.point - transform.position;
                if (throwVec.magnitude > 15.0f)
                {
                    throwVec = throwVec.normalized * 15.0f;
                }
                throwVec += Vector3.up * 10.0f;

                GameObject grenade = Instantiate(throwGrenade, transform.position, transform.rotation);
                Rigidbody rb = grenade.GetComponent<Rigidbody>();
                rb.AddForce(throwVec, ForceMode.Impulse);
                rb.AddTorque(Vector3.back * 10.0f, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }            
        }
    }

    void Reload()
    {
        if (curEquipWeapon == null || !curEquipWeapon.IsGun)
        {
            return;
        }

        GunWeapon gun = curEquipWeapon as GunWeapon;
        if (gun.Type == EGUN_TYPE.HANDGUN && totalHandGunAmmo <= 0)
        {
            return;
        }
        else if (gun.Type == EGUN_TYPE.MACHINEGUN && totalMachineGunAmmo <= 0)
        {
            return;
        }       

        if (gun.CurAmmo == gun.ReloadAmmo)
        {
            return;
        }

        if (rDown && !isJump && !isSwap && !isAttacking)
        {
            anim.SetTrigger("doReload");
            isReloading = true;

            Invoke("ReloadOut", 2.6f);
        }
    }

    void ReloadOut()
    {
        GunWeapon gun = curEquipWeapon as GunWeapon;
        int ammo = gun.ReloadAmmo - gun.CurAmmo;
        int reload = 0;
        if (gun.Type == EGUN_TYPE.HANDGUN)
        {
            reload = Mathf.Min(totalHandGunAmmo, ammo) ;
            totalHandGunAmmo -= reload;
            
        }
        else if (gun.Type == EGUN_TYPE.MACHINEGUN)
        {
            reload = Mathf.Min(totalMachineGunAmmo, ammo);
            totalMachineGunAmmo -= reload;
        }
        gun.CurAmmo += reload;
        isReloading = false;
    }
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        isBorder = Physics.Raycast(transform.position, transform.forward, 1.0f, LayerMask.GetMask("Wall"));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }

        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Boss")
        {
            if (collision.gameObject.tag == "Enemy")
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                health -= enemy.AttackPower;
            }
            else
            {
                Boss boss = collision.gameObject.GetComponent<Boss>();
                health -= boss.AttackPower;
            }                        
            gameObject.layer = 12;
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.grey;
            }
            StartCoroutine(OnDamage());
            Vector3 reactVec = transform.position - collision.transform.position;
            rigid.AddForce(reactVec * 5.0f, ForceMode.Impulse);
        }        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            audio.Play();
            Item item = other.GetComponent<Item>();
            switch (item.Type)
            {
                case ETYPE.HANDGUN_AMMO:
                    totalHandGunAmmo += 30;                    
                    break;
                case ETYPE.MACHINEGUN_AMMO:
                    totalMachineGunAmmo += 30;
                    break;
                case ETYPE.COIN:
                    coin += 500;
                    break;
                case ETYPE.GRENADE:
                    if (hasGrenades < maxHasGrenades)
                    {
                        grenades[hasGrenades++].SetActive(true);
                    }
                    break;
                case ETYPE.HEART:
                    health += 15;
                    break;
                default:
                    break;
            }

            Destroy(other.gameObject);            
        }
        else if (other.tag == "EnemyBullet")
        {            
            EnemyBullet bullet = other.GetComponent<EnemyBullet>();
            health -= bullet.Damage;
            gameObject.layer = 12;
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.grey;
            }
            StartCoroutine(OnDamage());
            Vector3 reactVec = transform.position - other.transform.position;
            rigid.AddForce(reactVec * 5.0f, ForceMode.Impulse);
        }       
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Weapon" || other.gameObject.tag == "Shop")
        {
            pressTxt.enabled = true;
            nearObj = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Weapon")
        {
            pressTxt.enabled = false;
            nearObj = null;
        }
        else if (other.gameObject.tag == "Shop")
        {
            pressTxt.enabled = false;
            Shop shop = nearObj.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObj = null;
        }
    }

    IEnumerator OnDamage()
    {
        if (health <= 0 && !isDie)
        {
            anim.SetTrigger("doDie");
            isDie = true;
            gameObject.layer = 15;
            GameManager.Instance.GameOver();            
        }
        else
        {
            yield return new WaitForSeconds(2.0f);
            gameObject.layer = 7;
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = Color.white;
            }
        }
    }
}
