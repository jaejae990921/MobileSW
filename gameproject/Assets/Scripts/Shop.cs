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
            case "1��": // ���� ���� ��ȭ
                enterPlayer.equipWeapon.wpUpgrade();
                break;
            case "2��": // ���Ÿ� ���� ��ȭ
                enterPlayer.bullUpgrade();
                break;
            case "3��": //źâ�� ����
                enterPlayer.maxbullUpgrade();
                break;
            case "4��": // �ִ� ü�� ����
                enterPlayer.maxhpUpgrade();
                break;
            case "5��": // ���� �ӵ� ����
                enterPlayer.equipWeapon.gunspeed();
                break;
            case "6��": // �̵��ӵ� ����          
                enterPlayer.speedUpgrade(); // �̵��ӵ�
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
