using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject menuCamera;
    [SerializeField] GameObject gameCamera;
    [SerializeField] Player player;
    [SerializeField] Boss boss;
    [SerializeField] int stage;
    [SerializeField] float playTime;
    [SerializeField] bool isBattle;
    [SerializeField] int enemyACnt;
    [SerializeField] int enemyBCnt;
    [SerializeField] int enemyCCnt;

    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] Text maxScoreTxt;

    [SerializeField] Text stageTxt;
    [SerializeField] Text playTimeTxt;
    [SerializeField] Text playerHealthTxt;
    [SerializeField] Text handGunAmmohTxt;
    [SerializeField] Text machineGunAmmohTxt;
    [SerializeField] Text coinhTxt;

    [SerializeField] Image weapon1Img;
    [SerializeField] Image weapon2Img;
    [SerializeField] Image weapon3Img;
    [SerializeField] Image weaponRImg;

    [SerializeField] Text enemyATxt;
    [SerializeField] Text enemyBTxt;
    [SerializeField] Text enemyCTxt;

    [SerializeField] RectTransform bossHealth;
    [SerializeField] RectTransform bossHealthBar;

}
