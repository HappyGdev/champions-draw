using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UiItemSpawner : MonoBehaviour
{
    public static UiItemSpawner Instance;   
    public GameObject itemPrefab;            // UI prefab (must be a RectTransform)
    public Transform layoutGroupParent;      // Parent with HorizontalLayoutGroup

    public Transform BosslayoutGroupParent;      // Parent with HorizontalLayoutGroup
    public Transform PlayerlayoutGroupParent;      // Parent with HorizontalLayoutGroup

    public List<GameObject> PlayerInventory = new List<GameObject>();
    public List<GameObject> BossInventory = new List<GameObject>();

    public List<GameObject> FightcardInventory= new List<GameObject>();

    private int use3Cards;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;    
    }

    public void SpawnItem(Card chosencard, bool isboss)
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

    public void CreateCard(GameObject itemPF, Transform LayoutGroup, Card Chosen, bool isZoomable, bool isBoss)
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
        else
        {
            FightcardInventory.Add(CCard);
        }

        if (isBoss)
        {
            BossInventory.Add(CCard);
        }
    }
    public void DeleteCardFromInventory(Card crd)
    {
        for (int i = FightcardInventory.Count - 1; i >= 0; i--)
        {
            var fcard = FightcardInventory[i];

            if (fcard == null)
            {
                FightcardInventory.RemoveAt(i); // clean up destroyed references
                continue;
            }

            var cardDisplay = fcard.GetComponent<CardDisplay>();
            if (cardDisplay != null && cardDisplay.Card.name == crd.name)
            {
                fcard.SetActive(false); // or Destroy(fcard) if needed
                break; // stop after finding the match
            }
        }
    }

    public void DestroyAllButOne(Card keepCard)
    {
        StartCoroutine(SwapAndRoll(keepCard));
    }
    IEnumerator SwapAndRoll(Card keepCard)
    {
        // کپی از لیست برای جلوگیری از تغییر همزمان
        var objectsToRemove = new List<GameObject>();

        foreach (var obj in PlayerInventory)
        {
            if (keepCard.name != obj.GetComponent<CardDisplay>().Card.name)
            {
                obj.GetComponent<Zoom>().enabled = false;
                objectsToRemove.Add(obj);  // اضافه به لیست حذف
            }
            else
            {
                obj.GetComponent<Zoom>().enabled = false;
            }

            yield return new WaitForSeconds(0.3f);
        }

        // حذف واقعی از لیست و Destroy
        foreach (var obj in objectsToRemove)
        {
            PlayerInventory.Remove(obj);  // حذف از لیست
            Destroy(obj);                 // نابودی گیم‌آبجکت
        }

        yield return new WaitForSeconds(0.2f);
        GameManager.instance.ReplaceTwoFightCardsButKeep(keepCard);
    }

    public void SwapCard(Card keepCard)
    {
        StartCoroutine(SwapTwoCard(keepCard));
    }
    IEnumerator SwapTwoCard(Card keepCard)
    {
        List<Card> cardsToSwap = new List<Card>();

        foreach (var obj in PlayerInventory)
        {
            Card currentCard = obj.GetComponent<CardDisplay>().Card;

            if (keepCard.name != currentCard.name)
            {
                cardsToSwap.Add(currentCard);
            }
            else
            {
                obj.GetComponent<Zoom>().enabled = false;
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (cardsToSwap.Count == 2)
        {
            var temp = cardsToSwap[0].value1;
            cardsToSwap[0].value1 = cardsToSwap[1].value1;
            cardsToSwap[1].value1 = temp;
        }
        else
        {
            Debug.LogWarning("Expected exactly 2 cards to swap, but found: " + cardsToSwap.Count);
        }

        yield return new WaitForSeconds(0.05f);
        GameManager.instance.ReplaceTwoCards();
    }



    public void ReplaceOneCardWithLowValue(Card keepCard, int value)
    {
        List<GameObject> removableCards = new List<GameObject>();

        // پیدا کردن کارت‌های غیر از keepCard در PlayerInventory
        foreach (var obj in PlayerInventory)
        {
            var display = obj.GetComponent<CardDisplay>();
            if (display != null && display.Card != keepCard)
            {
                removableCards.Add(obj);
            }
        }
        //disable cart for just act once
        foreach (var obj in PlayerInventory)
        {
            Debug.Log("Keep Crd name is " + keepCard.name);
            Debug.Log("select Crd name is " + obj.GetComponent<CardDisplay>().Card.name);
            if (keepCard.name == obj.GetComponent<CardDisplay>().Card.name)
            {
                obj.GetComponent<Zoom>().enabled = false;
            }
        }

        // اگر هیچ کارتی برای حذف نیست، خروج
        if (removableCards.Count == 0)
        {
            Debug.LogWarning("No removable cards found.");
            return;
        }

        // انتخاب رندوم یک کارت از بین قابل حذف‌ها
        int randIndex = UnityEngine.Random.Range(0, removableCards.Count);
        GameObject toRemove = removableCards[randIndex];

        PlayerInventory.Remove(toRemove);
        Destroy(toRemove);

        // حالا در PlayerFightcards به دنبال یک کارت با value1 < 5 بگرد
        foreach (Card c in GameManager.instance.PlayerFightcards)
        {
            if (c.value1 < value && c.actionType != CardActionType.empty)
            {
                SpawnFightCardItem(c); // کارت جدید اضافه شود به UI
                GameManager.onCardDisplay?.Invoke();
                return;
            }
        }

        Debug.LogWarning("No card with value1 > 5 found in PlayerFightcards.");
    }
    public void Choose3Card()
    {
        use3Cards = 3;
    }

    public void DestroyPlayerInventory(bool isBoosTurnSkip)
    {
        foreach (var item in PlayerInventory)
        {
            item.GetComponent<Zoom>().enabled = false;
            Destroy(item, 1f);
        }
        PlayerInventory.Clear();

        //Player Turn over Button Apear

        //implement use 3 moves in one logic
        if (use3Cards >= 1)
        {
            GameManager.instance.ChangeTurn();
            GameManager.instance.CheckBattleOutcome();
            UIManager.Instance.Player_turn_Over_button_On();
            use3Cards--;
            return;
        }
        else
        {
            if (isBoosTurnSkip)
            {
                GameManager.instance.ChangeTurn();
            }

            GameManager.instance.CheckBattleOutcome();

            UIManager.Instance.Player_turn_Over_button_On();
        }
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
        // UIManager.Instance.PlyerBossTurn(1);
        yield return new WaitForSeconds(2f);
        GameManager.instance.TurnLoop();
    }
    //public void BossAttack()
    //{
    //    if (GameManager.instance.gameOver)
    //        return;

    //    StartCoroutine(AttackFromBoss());

    //}
    //IEnumerator AttackFromBoss()
    //{
    //    //yield return new WaitForSeconds(2f);
    //    int ranindex = UnityEngine.Random.Range(0, BossInventory.Count);
    //    foreach (var item in BossInventory)
    //    {
    //        item.SetActive(false);
    //    }
    //    yield return new WaitForSeconds(0.2f);
    //    BossInventory[ranindex].SetActive(true);

    //    UIAnimationUtility.ShakeScale(BossInventory[ranindex].GetComponent<RectTransform>(), new Vector3(.2f, .8f, .2f), 0.5f, 10, 90, Ease.InOutBounce);
    //    UIAnimationUtility.ShakePosition(BossInventory[ranindex].GetComponent<RectTransform>(), new Vector3(1, 10, 1), 0.5f, 10, 90, Ease.InOutBounce);

    //    var bossdamage = BossInventory[ranindex].GetComponent<CardDisplay>().Card.value1;
    //    GameManager.instance.BossAttackPlayer(bossdamage);
    //}

}