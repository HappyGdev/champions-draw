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
    [Space]
    [Header ("Boss Cards")]
    public List<Card> bossGammaCards;
    public List<Card> bossBetaCards;
    public List<Card> bossAlphaCards;
    public List <Card> defaultCards;
    [Space]
    [Header("Panels")]
    public GameObject bossSelectionPanel;
    [SerializeField] private int currentBoss;


    private void Awake()
    {
        if(instance == null) { instance = this; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // ContinueButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ContinueButton()
    {
        LoadBoss();
        StartCoroutine(ContinueGame());
    }

    IEnumerator ContinueGame()
    {
        bossSelectionPanel.SetActive(true);
        SetBossUI();
        SetBossCards();
        yield return new WaitForSeconds(2f);
        bossSelectionPanel.SetActive(false);
        GameManager.instance.MainPanel.SetActive(true);
    }
    private void LoadBoss()
    {
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
                break;

            case 2:
                OnBossCards?.Invoke(bossAlphaCards);
                Debug.Log("bossBetaCards");
                break;

            default:
                OnBossCards?.Invoke(defaultCards);
                Debug.Log("this is Default card");
                break;


        }
    }
}
