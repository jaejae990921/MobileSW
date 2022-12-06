using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public GameObject[] itemObj;


    Player enterPlayer;

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {

    }


}
