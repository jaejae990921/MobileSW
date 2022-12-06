using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // 변수 관리
    public GameObject menuCam;
    public GameObject gameCam;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public GameObject ShopZone;

    public GameObject menuPanel; // 메뉴
    public Text maxScoretext; // 자신의 최고점수

    public GameObject gamePanel; // 인게임
    public Text ScoreText; // 점수
    public Text stageText; // 스테이지
    public Text playTimeText; // 플레이 시간


    public Player player; // 플레이어 스테이터스
    public Text playerHealthText; // 피
    public Text playerAmmoText; // 총알

    private void Awake()
    {
        maxScoretext.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

    }

    public void GameStart() // 게임시작 버튼 함수
    {
        menuCam.SetActive(false); // 메뉴 화면 미활성화 
        gameCam.SetActive(true); // 게임화면 시작

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
        StageStart();
    }
    
    // 스테이지 시작
    public void StageStart()
    {

        ShopZone.SetActive(false);
        isBattle = true; // 전투 상태
               
    }

    public void StageEnd()
    {
        //플레이어 원위치로 
        player.transform.position= Vector3.up * 0.8f;

        //상점 활성화
        ShopZone.SetActive(true);
        isBattle = false;
        stage++;
    }

  



    private void Update()
    {
        // 시간업데이트
        if (isBattle)
        {
            playTime += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        //스코어 가져오기
        ScoreText.text = string.Format("{0:n0}", player.score); // 천단위마다 , 넣음
        stageText.text = "STAGE " + stage;
        
        //체력 
        playerHealthText.text = player.health + "/" + player.maxHealth; // 체력/최대체력
        
        //시간
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime-hour*3600) / 60);
        int second = (int)playTime % 60;
        playTimeText.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second) ;

        // 총을 들었을 때 탄창수 보여줌
        if(player.equipWeapon == null) // 무기x
        {
            playerAmmoText.text = "-/-";
        }else if(player.equipWeapon.type == Weapon.Type.Melee) //근접무기
        {
            playerAmmoText.text = "-/-";
        }
        else
        {
            //현재 탄창 / 최대탄창
            playerAmmoText.text = player.equipWeapon.curAmmo+ "/" + player.equipWeapon.maxAmmo;
        }
    }
}
