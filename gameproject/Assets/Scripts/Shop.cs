using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public GameObject[] itemObj;
    public GameManager manager;

    Player enterPlayer;

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Buy(int index)
    {
        switch (index) {
            case 6:
                enterPlayer.speed *= 2; // 이동속도
                break;
            default:
                break;
        
        }



        Exit();
        enterPlayer.isShop = false;
    }

    public void Exit()
    {
        uiGroup.anchoredPosition = Vector3.down * 1000;
        manager.StageStart();
     
    }

    


}
