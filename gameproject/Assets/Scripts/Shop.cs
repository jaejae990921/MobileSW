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

    public Player enterPlayer;

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enterPlayer = other.GetComponent<Player>();
            uiGroup.anchoredPosition = Vector3.zero;
            enterPlayer.setShop(true);
        }
    }

    public void Buy(string index)
    {
        switch (index) {
            case "1번": // 근접 무기 강화
                enterPlayer.equipWeapon.wpUpgrade();
                break;
            case "2번": // 원거리 무기 강화
                enterPlayer.bullUpgrade();
                break;
            case "3번": //탄창수 증가
                enterPlayer.maxbullUpgrade();
                break;
            case "4번": // 최대 체력 증가
                enterPlayer.maxhpUpgrade();
                break;
            case "5번": // 연사 속도 증가
                enterPlayer.equipWeapon.gunspeed();
                break;
            case "6번": // 이동속도 증가          
                enterPlayer.speedUpgrade(); // 이동속도
                break;
            default:
                break;
        
        }
        Exit();
     
    }

    public void Exit()
    {
        
        uiGroup.anchoredPosition = Vector3.down * 1000;
        enterPlayer.setShop(false);
        manager.StageStart();
     
    }

    


}
