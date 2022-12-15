using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    // ���� ����
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

    // �� ����
    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    //UI ����
    public GameObject menuPanel; // �޴�
    public Text maxScoretext; // �ڽ��� �ְ�����

    public GameObject gamePanel; // �ΰ���
    public Text ScoreText; // ����
    public Text stageText; // ��������
    public Text playTimeText; // �÷��� �ð�

    public GameObject overPanel;
    public Text curScoreText;
    public Text bestScoreText;


    public Player player; // �÷��̾� �������ͽ�
    public Text playerHealthText; // ��
    public Text playerAmmoText; // �Ѿ�

    private void Awake()
    {
        enemyList = new List<int>();
        maxScoretext.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
        
    }

    public void GameStart() // ���ӽ��� ��ư �Լ�
    {
        menuCam.SetActive(false); // �޴� ȭ�� ��Ȱ��ȭ 
        gameCam.SetActive(true); // ����ȭ�� ����

        menuPanel.SetActive(false); // �޴� ��Ȱ��ȭ
        gamePanel.SetActive(true);  // ���� Ȱ��ȭ

        player.gameObject.SetActive(true);
        StageEnd();
    }
    //���� ����
    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = ScoreText.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if (player.score > maxScore)
        {
            bestScoreText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);

    }

    // �������� ����
    public void StageStart()
    {
        stage++;
        ShopZone.SetActive(false);  

        foreach(Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(true);
        }
        isBattle = true; // ���� ����
        StartCoroutine(InBattle());
    }

    IEnumerator InBattle()
    {
        if (stage % 5 == 0)
        {
            // 5���� ���� ���� ����
            enemyCntD++;
            GameObject instanceEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            Enemy enemy = instanceEnemy.GetComponent<Enemy>(); // ���� ��ũ��Ʈ ������
            enemy.manager = this;
            enemy.target = player.transform;    //�÷��̾ ���󰡰� ��
            boss = instanceEnemy.GetComponent<Boss>();
        }
        else
        {
            for (int i = 0; i < stage; i++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
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

            while (enemyList.Count > 0)
            {
                // 4���� �𼭸����� �����ϰ� ���� ����
                int ranZone = Random.Range(0, 4);
                GameObject instanceEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                // prefebsȭ �߱� ������ �������� ä���� ��
                Enemy enemy = instanceEnemy.GetComponent<Enemy>(); // ���� ��ũ��Ʈ ������
                enemy.target = player.transform;    //�÷��̾ ���󰡰� ��
                enemy.manager = this;
                enemyList.RemoveAt(0);  //��ȯ�� ���ʹ� ����Ʈ���� ����
                //yield return new WaitForSeconds(4f);    // ���ϸ� �� �����ӿ� �� ����
            }

        }//else

        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0) // ���� ���� ���� 0�̸� ���� ����
        {
          //  Debug.Log(enemyCntA + enemyCntB + enemyCntC + enemyCntD);
            yield return null;
        }
        //yield return new WaitForSeconds(3f);
        StageEnd();

    }

    public void StageEnd()
    {
        //�÷��̾� ����ġ�� 
        player.transform.position= Vector3.up * 0.8f;
        player.health = player.maxHealth;
        foreach (Transform zone in enemyZones)
        {
            zone.gameObject.SetActive(false);
        }
        //���� Ȱ��ȭ
        ShopZone.SetActive(true);
        isBattle = false;
        
    }


    private void Update()
    {
        // �ð�������Ʈ
        if (isBattle)
        {
            playTime += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        //���ھ� ��������
        ScoreText.text = string.Format("{0:n0}", player.score); // õ�������� , ����
        stageText.text = "STAGE " + stage;
        
        //ü�� 
        playerHealthText.text = player.health + "/" + player.maxHealth; // ü��/�ִ�ü��
        
        //�ð�
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime-hour*3600) / 60);
        int second = (int)playTime % 60;
        playTimeText.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second) ;

        // ���� ����� �� źâ�� ������
        if(player.equipWeapon == null) // ����x
        {
            playerAmmoText.text = "-/-";
        }else if(player.equipWeapon.type == Weapon.Type.Melee) //��������
        {
            playerAmmoText.text = "-/-";
        }
        else
        {
            //���� źâ / �ִ�źâ
            playerAmmoText.text = player.equipWeapon.curAmmo+ "/" + player.equipWeapon.maxAmmo;
        }
    }
}
