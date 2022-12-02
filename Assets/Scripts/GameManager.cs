using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            { 
                return null; 
            }
            return instance;
        }
    }

    [SerializeField] private GameObject menuCamera;
    [SerializeField] private GameObject gameCamera;
    [SerializeField] private Player player;
    [SerializeField] private Boss boss;
    [SerializeField] private GameObject[] spawnZones;
    [SerializeField] private GameObject[] enemys;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Text maxScoreTxt;
                     
    [SerializeField] private Text stageTxt;
    [SerializeField] private Text scoreTxt;
    [SerializeField] private Text playTimeTxt;
    [SerializeField] private Text playerHealthTxt;
    [SerializeField] private Text handGunAmmohTxt;
    [SerializeField] private Text machineGunAmmohTxt;
    [SerializeField] private Text coinTxt;

    [SerializeField] private Image[] weapon1Img;
    [SerializeField] private Image[] weapon2Img;
    [SerializeField] private Image[] weapon3Img;
    [SerializeField] private Image weaponRImg;
                     
    [SerializeField] private Text enemyATxt;
    [SerializeField] private Text enemyBTxt;
    [SerializeField] private Text enemyCTxt;
                     
    [SerializeField] private RectTransform bossHealth;
    [SerializeField] private RectTransform bossHealthBar;

    [SerializeField] private GunWeapon handGun;
    [SerializeField] private GunWeapon machineGun;

    [SerializeField] private GameObject weaponShop;
    [SerializeField] private GameObject itemShop;
    [SerializeField] private GameObject startZone;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text bestTxt;
    [SerializeField] private Text curScoreTxt;

    [SerializeField] private AudioClip mainSound;
    [SerializeField] private AudioClip gameSound;

    private List<Image[]> weaponImages;
    [SerializeField] private int stage;
    private float sec;
    private int min;
    private bool isBattle;
    private int enemyACnt;
    private int enemyBCnt;
    private int enemyCCnt;
    private int enemyDCnt;
    private Boss curBoss;
    private AudioSource audio;
    public int EnemyACnt
    {
        get { return enemyACnt; }
        set { enemyACnt = value; }
    }

    public int EnemyBCnt
    {
        get { return enemyBCnt; }
        set { enemyBCnt = value; }
    }
    public int EnemyCCnt
    {
        get { return enemyCCnt; }
        set { enemyCCnt = value; }
    }

    public int EnemyDCnt
    {
        get { return enemyDCnt; }
        set { enemyDCnt = value; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        weaponImages = new List<Image[]>();
        weaponImages.Add(weapon1Img);
        weaponImages.Add(weapon2Img);
        weaponImages.Add(weapon3Img);

        enemyACnt = 0;
        enemyBCnt = 0;
        enemyCCnt = 0;
        enemyDCnt = 0;

        maxScoreTxt.text = string.Format("{0:n0}",  PlayerPrefs.GetInt("MaxScore"));
        audio = GetComponent<AudioSource>();
        audio.Play();
    }

    void Update()
    {
        if (isBattle)
        {
            sec += Time.deltaTime;
        }

        if (enemyACnt == 0 && enemyBCnt == 0 && enemyCCnt == 0 && enemyDCnt == 0 && isBattle)
        {
            StartCoroutine(StageEnd());
        }
    }

    void LateUpdate()
    {        
        scoreTxt.text = string.Format("{0:n0}", player.CurScore);

        //오른쪽 위
        stageTxt.text = "STAGE " + stage;
        if ((int)sec >= 60)
        {
            sec = 0.0f;
            min++;
        }
        playTimeTxt.text = string.Format("{0:D2} : {1:D2}", min, (int)sec);        
        
        //오른쪽 아래
        enemyATxt.text = "X " + enemyACnt;
        enemyBTxt.text = "X " + enemyBCnt;
        enemyCTxt.text = "X " + enemyCCnt;

        //가운데 아래, 왼쪽아래
        playerHealthTxt.text = player.Health.ToString();
        coinTxt.text = player.Coin.ToString();

        bool activeGrenade = player.HasGrenades > 0 ? true : false;
        weaponRImg.gameObject.SetActive(activeGrenade);
        if (player.WeaponList != null)
        {
            if (player.WeaponList.Count > 0)
            {
                int i = 0;
                foreach (GameObject weapon in player.WeaponList)
                {
                    Weapon wp = weapon.GetComponent<Weapon>();
                    if (!wp.IsGun)
                    {
                        weaponImages[i][0].gameObject.SetActive(true);
                    }
                    else
                    {
                        GunWeapon gun = wp as GunWeapon;
                        if (gun.Type == EGUN_TYPE.HANDGUN)
                        {
                            weaponImages[i][1].gameObject.SetActive(true);
                            handGunAmmohTxt.text = handGun.CurAmmo + " / " + player.TotalHandGunAmmo;
                        }
                        else
                        {
                            weaponImages[i][2].gameObject.SetActive(true);
                            machineGunAmmohTxt.text = machineGun.CurAmmo + " / " + player.TotalMachinegunAmmo;
                        }
                    }
                    i++;
                }
            }
        }
        
        

        //가운데 위
        if (curBoss != null)
        {
            bossHealth.gameObject.SetActive(true);

            bossHealthBar.localScale = new Vector3(Mathf.Max(0, (float)curBoss.CurHealth / curBoss.MaxHealth), 1, 1);
        }
    }

    public void StageStart()
    {       
        isBattle = true;
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);
        StartCoroutine(InBattle());
    }

    IEnumerator InBattle()
    {
        if (stage % 5 == 0)
        {
            enemyDCnt++;
            GameObject b = Instantiate(boss.gameObject, new Vector3(0.0f, 1.0f, -8.0f), Quaternion.identity);
            curBoss = b.GetComponent<Boss>();            
            curBoss.SetHealth(stage * 100);
        }
        else
        {
            curBoss = null;
            bossHealth.gameObject.SetActive(false);
            foreach (GameObject zone in spawnZones)
            {
                zone.SetActive(true);
            }

            for (int i = 0; i < stage; i++)
            {
                int ranEnemy = Random.Range(0, 3);
                int ranSpawnPos = Random.Range(0, 4);
                GameObject spawn = Instantiate(enemys[ranEnemy], spawnZones[ranSpawnPos].transform.position, Quaternion.identity);
                Enemy enemy = spawn.GetComponent<Enemy>();
                if (enemy is EnemyA)
                {
                    enemyACnt++;
                }
                else if (enemy is EnemyB)
                {
                    enemyBCnt++;
                }
                else
                {
                    enemyCCnt++;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }                
    }

    IEnumerator StageEnd()
    {
        isBattle = false;
        yield return new WaitForSeconds(1.5f);

        GameObject[] enemyBullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach(GameObject bullet in enemyBullets)
        {
            bullet.SetActive(false);
        }

        foreach (GameObject zone in spawnZones)
        {
            zone.SetActive(false);
        }
        player.transform.position = new Vector3(0.0f, 1.0f, -8.0f);
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);
        stage++;
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        curScoreTxt.text = scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if (player.CurScore > maxScore)
        {
            bestTxt.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.CurScore);
        }
    }
    
    public void Restart()
    {
        audio.clip = mainSound;
        audio.Play();
        SceneManager.LoadScene(0);
    }
    public void GameStart()
    {
        audio.clip = gameSound;
        audio.Play();

        menuCamera.SetActive(false);
        gameCamera.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }
}
