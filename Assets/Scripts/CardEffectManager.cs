using System.Collections;
using System;
using UnityEngine;
using DG.Tweening;

public class CardEffectManager : MonoBehaviour
{
    public static CardEffectManager Instance;

    private int  bossPoisendRound;
    private int  dmgBoost;
    private int  attack3X;
    private bool isAllowTocontiunueTurn;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //Initialize
        bossPoisendRound = 0;   
        dmgBoost         = 0;
    }


    #region Player Power_Up
    //Player Card Effect
    public void ApplyCardEffect(Card card)
    {
        switch (card.actionType)
        {
            case CardActionType.Attack:
                int dmg = card.value1;   
                HealthBar.instance.BossTakeDamage(dmg + dmgBoost);

                GameManager.instance.SendEndAction(false);
                break;

            case CardActionType.Heal:
                int hlth = card.value1;   
                HealthBar.instance.PlayerTakeDamage(-hlth);
                GameManager.instance.SendEndAction(false);
                break;

            case CardActionType.Multi:
                HandleMultiAction(card);
                break;

            case CardActionType.empty:
                Debug.Log("Empty card");
                break;
        }
    }

    private void HandleMultiAction(Card card)
    {
        switch (card.multiActionType)
        {
            case MultiActionType.AttackTwice:
                StartCoroutine(AttackTwiceRoutine(card));
                break;

            case MultiActionType.SwapValues:
                StartCoroutine(SwapValue(card));
                break;

            case MultiActionType.BossStun:
                HealthBar.instance.BossTakeDamage(card.value1 + dmgBoost);
                GameManager.instance.SendEndAction(true);
                break;

            case MultiActionType.RollAndSwap:
                StartCoroutine(RollAgainAndSwapRoutine(card));
                break;

            case MultiActionType.DiscardAndAdd5:
                StartCoroutine(TryAddCardFromField(card,5));
                break;

            case MultiActionType.DiscardAndAdd7:
                StartCoroutine(TryAddCardFromField(card, 7));
                break;

            case MultiActionType.PoisonBoss:
                StartCoroutine(PoisonBossRoutine(card));
                break;

            case MultiActionType.BoostDamage:
                dmgBoost += 2;
                UIManager.Instance.ShowDamageBoost();
                HealthBar.instance.BossTakeDamage(card.value1 + dmgBoost);
                GameManager.instance.SendEndAction(false);
                break;

            case MultiActionType.doubleHealingLess50:
                if (HealthBar.instance.PlayercurrentHealth <= 50)
                {
                    int hlth = card.value1;
                    HealthBar.instance.PlayerTakeDamage(-hlth * 2);
                    GameManager.instance.SendEndAction(false);
                }
                else
                {
                    int hlth = card.value1;
                    HealthBar.instance.PlayerTakeDamage(-hlth);
                    GameManager.instance.SendEndAction(false);
                }
                break;

            case MultiActionType.ReduceDamageNextTurn:
                StartCoroutine(NextroundMinus5(card));
                GameManager.instance.SendEndAction(false);
                break;

            case MultiActionType.doubleDamageRound:
                StartCoroutine(DoubleEachCard());
                break;

            case MultiActionType.Select3Card:
                StartCoroutine(Use3Card());
                break;

            default:
                Debug.LogWarning("Unknown MultiActionType");
                break;
        }
    }

    // Coroutine Sections
    public int CurrentBoost() { return dmgBoost; }
    private IEnumerator AttackTwiceRoutine(Card crd)
    {
        HealthBar.instance.BossTakeDamage(crd.value1 + dmgBoost);
        yield return new WaitForSeconds(0.5f);
        HealthBar.instance.BossTakeDamage(crd.value1 + dmgBoost);
        GameManager.instance.SendEndAction(false);
    }
    private IEnumerator SwapValue(Card crd)
    {
        if (crd != null)
        {UiItemSpawner.Instance.SwapCard(crd);}
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator PoisonBossRoutine(Card crd)
    {
        bossPoisendRound += 3;
        HealthBar.instance.BossTakeDamage(crd.value1 + dmgBoost);
        HealthBar.instance.BossPoisonHelathbar();
        yield return new WaitForSeconds(0.5f);
        //Apply poison manuelly for first time
        HealthBar.instance.BossTakeDamage(5 + dmgBoost);
        GameManager.instance.PoisendBoss(bossPoisendRound);
        yield return new WaitForSeconds(1f);
        GameManager.instance.SendEndAction(false);
    }
    private IEnumerator RollAgainAndSwapRoutine(Card crd)
    {
        if (crd != null)
        {UiItemSpawner.Instance.DestroyAllButOne(crd);}
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator NextroundMinus5(Card crd)
    {
        HealthBar.instance.BossTakeDamage(crd.value1 + dmgBoost);
        GameManager.instance.ProtectPlayer();
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator DoubleEachCard()
    {
        HealthBar.instance.DoubleDamageRound();
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator TryAddCardFromField(Card crd,int value)
    {
        if (crd != null)
        { UiItemSpawner.Instance.ReplaceOneCardWithLowValue(crd, value); }
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator Use3Card()
    {
        UiItemSpawner.Instance.Choose3Card();
        yield return new WaitForSeconds(1f);
    }
    // end of Coroutines <summary>
 
    #endregion

    #region Boss Power Up
    public void BossCardEffect()      {BossCardsAction(); }
    private void BossCardsAction()    {StartCoroutine(AttackFromBoss());}
    IEnumerator AttackFromBoss()
    {
        int ranindex = UnityEngine.Random.Range(0, UiItemSpawner.Instance.BossInventory.Count);
        foreach (var item in UiItemSpawner.Instance.BossInventory)
        {
            item.SetActive(false);
        }
        yield return new WaitForSeconds(0.2f);
        UiItemSpawner.Instance.BossInventory[ranindex].SetActive(true);

        UIAnimationUtility.ShakeScale(UiItemSpawner.Instance.BossInventory[ranindex].GetComponent<RectTransform>(), new Vector3(.2f, .8f, .2f), 0.5f, 10, 90, Ease.InOutBounce);
        UIAnimationUtility.ShakePosition(UiItemSpawner.Instance.BossInventory[ranindex].GetComponent<RectTransform>(), new Vector3(1, 10, 1), 0.5f, 10, 90, Ease.InOutBounce);

        var bosscard = UiItemSpawner.Instance.BossInventory[ranindex].GetComponent<CardDisplay>().Card;
        DecideBossAttack(bosscard);
    }

    public void DecideBossAttack(Card crd)
    {
        switch (crd.actionType)
        {
            case CardActionType.Attack:
                GameManager.instance.BossAttackPlayer(crd.value1, false);
                break;

            case CardActionType.Heal:
                HealthBar.instance.BossTakeDamage(-crd.value1);
                StartCoroutine(ChangeTurn());
                break;

            case CardActionType.Multi:
                HandleBossMultiAction(crd);
                break;

            case CardActionType.empty:
                break;

            default:
                break;
        }
    }
    private void HandleBossMultiAction(Card card)
    {
        switch (card.multiActionType)
        {
            case MultiActionType.None:
                break;

            // for boss valu3 means the attack round forexample 3 means attack 3 times
            case MultiActionType.AttackTwice:
                var attackRound = card.value3;
                StartCoroutine(BossMultipleAtatck(card, attackRound));
                break;

            //just used same name but its player stun not boss stun we could use seprate class but for 1 variable seems not neccessary
            case MultiActionType.BossStun:
                GameManager.instance.BossAttackPlayer(card.value1, true);
                break;

            case MultiActionType.PoisonBoss:
                break;

            case MultiActionType.BoostDamage:
                break;

            default:
                break;
        }
    }
    private IEnumerator BossMultipleAtatck(Card crd, int round)
    {
        for (int i = 0; i < round; i++)
        {
            GameManager.instance.BossAttackPlayer(crd.value1, false);
            yield return new WaitForSeconds(0.4f);
        }
    }
    private IEnumerator ChangeTurn()
    {
        yield return new WaitForSeconds(1f);
        UiItemSpawner.Instance.DestroyBossInventory();
        // Old Logic With Button 
        ///UIManager.Instance.Player_turn_Over_button_On();
        // New Logic
        UiItemSpawner.Instance.Player_Turn_Over_button();

    }

    #endregion
}
