using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    public int enemyCntD;

    public GameObject ShopZone;

    // 적 생성
    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    //UI 관리
    public GameObject menuPanel; // 메뉴
    public Text maxScoretext; // 자신의 최고점수

    public GameObject gamePanel; // 인게임
    public Text ScoreText; // 점수
    public Text stageText; // 스테이지
    public Text playTimeText; // 플레이 시간

    public GameObject overPanel;
    public Text curScoreText;
    public Text bestScoreText;


    public Player player; // 플레이어 스테이터스
    public Text playerHealthText; // 피
    public Text playerAmmoText; // 총알

    private void Awake()
    {
        enemyList = new List<int>();
        maxScoretext.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
        
    }

    public void GameStart() // 게임시작 버튼 함수
    {
        menuCam.SetActive(false); // 메뉴 화면 비활성화 
        gameCam.SetActive(true); // 게임화면 시작

        menuPanel.SetActive(false); // 메뉴 비활성화
        gamePanel.SetActive(true);  // 게임 활성화

        player.gameObject.SetActive(true);
        StageEnd();
    }
    //게임 오버
    public void GameOver()
    {
        // 게임 잰힝에 관한 패널 비활성화, 게임오버 화면 활성화
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = ScoreText.text;
        // 기록 관리
        int maxScore = PlayerPrefs.GetInt("MaxScore"); // 프리팹에 최고 기록 가져옴
        if (player.score > maxScore) // 최고 기록 갱신했으면 best문구 활성화 및 프리팹 수정
        {
            bestScoreText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }
    
    // 게임 재시작 버튼 기능 
    public void Restart()
    {
        SceneManager.LoadScene(0);

    }

    // 스테이지 시작
    public void StageStart()
    {
        stage++;
        ShopZone.SetActive(false);  // 게임 진행 중 상점 이용 불가

        foreach(Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(true);
        }
        isBattle = true; // 전투 상태
        StartCoroutine(InBattle());
    }

    IEnumerator InBattle() // 전투관련 로직이 담긴 코루틴
    {
        if (stage % 5 == 0)
        {
            // 5라운드 마다 보스 등장
            enemyCntD++;
            GameObject instanceEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instanceEnemy.GetComponent<Enemy>(); // 몬스터 스크립트 가져옴
            enemy.manager = this;
            enemy.target = player.transform;    //플레이어를 따라가게 함
            boss = instanceEnemy.GetComponent<Boss>();
        }
        else // 일반 라운드  
        {
            for (int i = 0; i < stage; i++) // 랜덤하게 일반 몬스터 3마리 중 한마리 생성
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran) // 몬스터 생성시 해당 몬스터 수를 늘려줌
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;

                }//switch

            }//for

            while (enemyList.Count > 0) // 라운드 만큼 몬스터 생성 
            {
                // 4개의 모서리에서 랜덤하게 몬스터 등장
                int ranZone = Random.Range(0, 4);
                GameObject instanceEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                // prefebs화 했기 때문에 변수들을 채워야 됨
                Enemy enemy = instanceEnemy.GetComponent<Enemy>(); // 몬스터 스크립트 가져옴
                enemy.target = player.transform;    //플레이어를 따라가게 함
                enemy.manager = this;
                enemyList.RemoveAt(0);  //소환된 몬스터는 리스트에서 삭제
                //yield return new WaitForSeconds(4f);    // 안하면 한 프레임에 다 나옴
            }

        }//else

        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0) // 몬스터 수의 합이 0이면 게임 종료
        {
          //  Debug.Log(enemyCntA + enemyCntB + enemyCntC + enemyCntD);
            yield return null;
        }
        //yield return new WaitForSeconds(3f);
        StageEnd();

    }
    // 스테이지 종료
    public void StageEnd()
    {
        //플레이어 원위치로 
        player.transform.position= Vector3.up * 0.8f;
        player.health = player.maxHealth; // 체력 회복
        foreach (Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(false);
        }
        //상점 활성화
        ShopZone.SetActive(true);
        isBattle = false;
        
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
