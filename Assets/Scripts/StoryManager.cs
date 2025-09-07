using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class StoryManagere : MonoBehaviour
{
    public static StoryManagere instance;
    public static Action<List<Card>> OnBossCards;

    [Header ("All Boss")]
    public List<GameObject> bossType=new List<GameObject>();
    public List<GameObject>Levels=new List<GameObject>();
    [Space]
    [Header ("Boss Cards")]
    public List<Card> bossGammaCards;
    public List<Card> bossBetaCards;
    public List<Card> bossAlphaCards;
    public List<Card> boss4Cards;
    public List<Card> boss5Cards;
    public List<Card> boss6Cards;
    public List <Card> defaultCards;
    [Space]
    [Header("Panels")]
    public GameObject bossSelectionPanel;
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
        bossSelectionPanel.SetActive(true);
        SetBossUI();
        SetBossCards();
        yield return new WaitForSeconds(2f);
        bossSelectionPanel.SetActive(false);
        SetLevel();   
        StoryBoardPanels.SetActive(true);
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
    private void SetBossCards()
    {
        Debug.Log("Setting Boss Cards" + currentBoss);
        switch (currentBoss)
        {
            case 0:
                OnBossCards?.Invoke(bossGammaCards);
                Debug.Log("bossGammaCards");
                break;

            case 1:
                OnBossCards?.Invoke(bossBetaCards);
                Debug.Log("bossBetaCards");
                FirebaseManager.Instance.SaveBadge(1);
                //sendbadge to leaderBoard
                break;

            case 2:
                OnBossCards?.Invoke(bossAlphaCards);
                Debug.Log("bossBetaCards");
                FirebaseManager.Instance.SaveBadge(2);
                //sendbadgetoleaderboard
                break;
            case 3:
                OnBossCards?.Invoke(boss4Cards);
                //sendbadgetoleaderboard
                break;
            case 4:
                OnBossCards?.Invoke(boss5Cards);
                //sendbadgetoleaderboard
                break;
            case 5:
                OnBossCards?.Invoke(boss6Cards);
                //sendbadgetoleaderboard
                break;

            default:
                OnBossCards?.Invoke(defaultCards);
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
