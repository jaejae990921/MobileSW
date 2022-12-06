using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject ShopZone;

    public GameObject menuPanel; // �޴�
    public Text maxScoretext; // �ڽ��� �ְ�����

    public GameObject gamePanel; // �ΰ���
    public Text ScoreText; // ����
    public Text stageText; // ��������
    public Text playTimeText; // �÷��� �ð�


    public Player player; // �÷��̾� �������ͽ�
    public Text playerHealthText; // ��
    public Text playerAmmoText; // �Ѿ�

    private void Awake()
    {
        maxScoretext.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

    }

    public void GameStart() // ���ӽ��� ��ư �Լ�
    {
        menuCam.SetActive(false); // �޴� ȭ�� ��Ȱ��ȭ 
        gameCam.SetActive(true); // ����ȭ�� ����

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
        StageStart();
    }
    
    // �������� ����
    public void StageStart()
    {

        ShopZone.SetActive(false);
        isBattle = true; // ���� ����
               
    }

    public void StageEnd()
    {
        //�÷��̾� ����ġ�� 
        player.transform.position= Vector3.up * 0.8f;

        //���� Ȱ��ȭ
        ShopZone.SetActive(true);
        isBattle = false;
        stage++;
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
