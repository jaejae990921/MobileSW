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

   
    private void OnTriggerEnter(Collider other)  // 상점 기능
    {
        if (other.gameObject.tag == "Player") // Player가 들어오면
        {
            enterPlayer = other.GetComponent<Player>(); // 상점 UI를 화면 중앙으로 보여줌
            uiGroup.anchoredPosition = Vector3.zero; // Player의 상태를 상점 이용 중으로 설정
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

    public void Exit() // 상점 종료
    {
        
        uiGroup.anchoredPosition = Vector3.down * 1000; // 상점 기능 종료시 UI를 화면 밑으로 내림
        enterPlayer.setShop(false); // Player의 상태를 상점이용 하지않도록 변경
        manager.StageStart();  // 다음 스테이지 시작
     
    }

    


}
