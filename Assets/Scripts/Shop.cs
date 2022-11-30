using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private RectTransform ui;
    [SerializeField] private Animator anim;

    [SerializeField] private GameObject[] itemObjs;
    [SerializeField] private int[] prices;
    [SerializeField] private Transform[] itemPos;

    private Player enterPlayer;
    public void Enter(Player player)
    {
        enterPlayer = player;
        ui.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        anim.SetTrigger("doHello");
        ui.anchoredPosition = Vector3.down * 1500.0f;
    }    

    public void Buy(int index)
    {
        if (prices[index] > enterPlayer.Coin)
        {
            return;
        }

        enterPlayer.Coin -= prices[index];
        Instantiate(itemObjs[index], itemPos[index].position, itemPos[index].rotation);
    }
}
