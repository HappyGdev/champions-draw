using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;            // UI prefab (must be a RectTransform)
    public Transform layoutGroupParent;      // Parent with HorizontalLayoutGroup

    public Transform BosslayoutGroupParent;      // Parent with HorizontalLayoutGroup
    public Transform PlayerlayoutGroupParent;      // Parent with HorizontalLayoutGroup

    public List<GameObject> PlayerInventory = new List<GameObject>();
    public List<GameObject> BossInventory = new List<GameObject>();


    public void SpawnItem(Card chosencard,bool isboss)
    {
        if (itemPrefab == null || layoutGroupParent == null)
        {
            Debug.LogWarning("Prefab or LayoutGroup not assigned.");
            return;
        }

        if (isboss)
        {
            CreateCard(itemPrefab, BosslayoutGroupParent, chosencard, false, true);
        }
        else
        {
            CreateCard(itemPrefab, layoutGroupParent, chosencard, false, false);
        }

    }
    public void SpawnFightCardItem(Card chosencard)
    {
        if (itemPrefab == null || layoutGroupParent == null)
        {
            Debug.LogWarning("Prefab or LayoutGroup not assigned.");
            return;
        }

        CreateCard(itemPrefab, PlayerlayoutGroupParent, chosencard, true, false);
    }

    public void CreateCard(GameObject itemPF,Transform LayoutGroup,Card Chosen,bool isZoomable,bool isBoss)
    {
        var CCard = Instantiate(itemPF, LayoutGroup);
        CCard.GetComponent<CardDisplay>().Card = Chosen;
        CCard.GetComponent<CardDisplay>().Card.name = Chosen.name;
        CCard.GetComponent<CardDisplay>().Card.type = Chosen.type;
        CCard.GetComponent<CardDisplay>().Card.actionType = Chosen.actionType;
        CCard.GetComponent<CardDisplay>().Card.artwork = Chosen.artwork;
        CCard.GetComponent<CardDisplay>().Card.value1 = Chosen.value1;
        CCard.GetComponent<CardDisplay>().Card.value2 = Chosen.value2;
        CCard.GetComponent<CardDisplay>().Card.value3 = Chosen.value3;

        if (isZoomable)
        {
            CCard.GetComponent<Zoom>().enabled = true;
            CCard.GetComponent<BoxCollider2D>().enabled = true; 
            PlayerInventory.Add(CCard); 
        }

        if (isBoss)
        {
            BossInventory.Add(CCard);
        }
    }

    public void DestroyPlayerInventory()
    {
        foreach (var item in PlayerInventory)
        {
            item.GetComponent<Zoom>().enabled=false;   
            Destroy(item,1f);
        }
        PlayerInventory.Clear();

        //Player Turn over Button Apear
        UIManager.Instance.Player_turn_Over_button_On();
    }
    public void Player_Turn_Over_button()
    {
        //Now its Boss Turn
        StartCoroutine(ContinueTurn());
    }
    public void DestroyBossInventory()
    {
        foreach (var item in BossInventory)
        {
            Destroy(item);
        }
        BossInventory.Clear();
    }
    IEnumerator ContinueTurn()
    {
        yield return new WaitForSeconds(2f);
        GameManager.instance.TurnLoop();
    }
    public void BossAttack()
    {
        if (GameManager.instance.gameOver)
            return;

        StartCoroutine(AttackFromBoss());

    }
    IEnumerator AttackFromBoss()
    {
        yield return new WaitForSeconds(2f);
        int ranindex = UnityEngine.Random.Range(0, BossInventory.Count);

        UIAnimationUtility.ShakePosition(BossInventory[ranindex].GetComponent<RectTransform>(), new Vector3(2, 10, 1), 0.5f, 10, 90, Ease.InOutBounce);
        var bossdamage = BossInventory[ranindex].GetComponent<CardDisplay>().Card.value1;
        GameManager.instance.BossAttackPlayer(bossdamage);
    }

    private void OnEnable()
    {
        GameManager.onDestroyPlayedCard += DestroyPlayerInventory;
        GameManager.onBossAttackTurn += BossAttack;
        GameManager.onDestroyBosscard += DestroyBossInventory;    
    }
    private void OnDisable()
    {
        GameManager.onDestroyPlayedCard -= DestroyPlayerInventory;
        GameManager.onBossAttackTurn -= BossAttack;
        GameManager.onDestroyBosscard -= DestroyBossInventory;
    }
}
