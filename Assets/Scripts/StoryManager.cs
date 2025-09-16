using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;

public class StoryManagere : MonoBehaviour
{
    public static StoryManagere instance;
    public static Action<List<Card>> OnBossCards;

    [Header ("All Boss")]
    public List<GameObject> bossType=new List<GameObject>();
    public List<GameObject>Levels=new List<GameObject>();
    public GameObject[] CheckBox=new GameObject[6];
    //0 gamma 1 beta  2 gamma  4 bots
    public Sprite[] bossImages=new Sprite[4];
    public GameObject[] bossAvatar;
    [Space]
    [Header ("Boss Cards")]
    public List<Card> tutoialBossCards;
    public List<Card> enemy1BossCards;
    public List<Card> enemy2BossCards;
    public List<Card> enemy3BossCards;
    public List<Card> enemy4BossCards;
    public List<Card> gammaBossCards;
    public List <Card> defaultCards;
    [Space]
    [Header("Panels")]
    //public GameObject bossSelectionPanel;
    public GameObject StoryBoardPanels;
    [SerializeField] private int currentBoss;
    public GameObject badgePanel;

    private void Awake()
    {
        if(instance == null) { instance = this; }
    }
    public void ContinueButton()
    {
        if (currentBoss != 6)
        {
            LoadBoss();
            StartCoroutine(ContinueGame());
        }
    }

    IEnumerator ContinueGame()
    {
        if (currentBoss != 0 && currentBoss<=2) 
        {
            badgePanel.SetActive(true);
            yield return new WaitForSeconds(2f);
            badgePanel.SetActive(false);
        }
        //yield return new WaitForSeconds(0.5f);
        //bossSelectionPanel.SetActive(true);
        SetBossUI();
        SetBossCards();

        SetLevel();
        SetCheckBox();
        StoryBoardPanels.SetActive(true);
        //yield return new WaitForSeconds(2f);
        //bossSelectionPanel.SetActive(false);

    }
    private void LoadBoss()
    {
        //load from uimanager after game won
        currentBoss = PlayerPrefs.GetInt("BossType");
    }
    private void SetBossUI()
    {
        foreach (GameObject go in bossType) 
        { 
            go.SetActive(false);
        }
        bossType[currentBoss].SetActive(true);
    }
    public void StartStory()
    {
        GameManager.instance.MainPanel.SetActive(true);
    }
    private void SetLevel()
    {
        foreach (GameObject lvl in Levels)
        {
            lvl.SetActive(false);
        }
        Levels[currentBoss].SetActive(true);
    }
    private void SetCheckBox()
    {
        foreach (var chkbx in CheckBox)
        {
            chkbx.SetActive(false);
        }
        //CheckBox[0].SetActive(true);
        for (int i = 0; i < currentBoss; i++)
        {
            CheckBox[i].SetActive(true);
        }      
    }
    private void SetBossCards()
    {
        Debug.Log("Setting Boss Cards" + currentBoss);
        switch (currentBoss)
        {
            case 0:
                OnBossCards?.Invoke(tutoialBossCards);
                foreach (var boss in bossAvatar)
                {
                    boss.GetComponent<Image>().sprite = bossImages[0];
                }
                Debug.Log("bossGammaCards");
                break;

            case 1:
                OnBossCards?.Invoke(enemy1BossCards);
                foreach (var boss in bossAvatar)
                {
                    boss.GetComponent<Image>().sprite = bossImages[1];
                }
                Debug.Log("bossBetaCards");
                FirebaseManager.Instance.SaveBadge(1);
                //sendbadge to leaderBoard
                break;

            case 2:
                OnBossCards?.Invoke(enemy2BossCards);
                foreach (var boss in bossAvatar)
                {
                    boss.GetComponent<Image>().sprite = bossImages[2];
                }
                FirebaseManager.Instance.SaveBadge(2);
                //sendbadgetoleaderboard
                break;
            case 3:
                OnBossCards?.Invoke(enemy3BossCards);
                foreach (var boss in bossAvatar)
                {
                    boss.GetComponent<Image>().sprite = bossImages[3];
                }
                //sendbadgetoleaderboard
                break;
            case 4:
                OnBossCards?.Invoke(enemy4BossCards);
                foreach (var boss in bossAvatar)
                {
                    boss.GetComponent<Image>().sprite = bossImages[4];
                }                //sendbadgetoleaderboard
                break;
            case 5:
                OnBossCards?.Invoke(gammaBossCards);
                foreach (var boss in bossAvatar)
                {
                    boss.GetComponent<Image>().sprite = bossImages[5];
                }                //sendbadgetoleaderboard
                break;

            default:
                OnBossCards?.Invoke(defaultCards);
                foreach (var boss in bossAvatar)
                {
                    boss.GetComponent<Image>().sprite = bossImages[2];
                }
                Debug.Log("this is Default card");
                break;


        }
    }

    //private void EarnBadgePanel()
    //{
    //    StartCoroutine(ActiveBadge());
    //}
    //IEnumerator ActiveBadge()
    //{
    //    badgePanel.SetActive(true);
    //    yield return new WaitForSeconds(2f);
    //    badgePanel.SetActive(false);
    //}
}
