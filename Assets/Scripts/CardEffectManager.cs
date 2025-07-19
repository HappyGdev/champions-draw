using System.Collections;
using System;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    public static CardEffectManager Instance;

    public int bossPoisendRound;
    public int dmgBoost;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        bossPoisendRound = 0;   
        dmgBoost = 0;
    }
    public int CurrentBoost()
    {
        return dmgBoost;
    }
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

            default:
                Debug.LogWarning("Unknown MultiActionType");
                break;
        }
    }
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

    private IEnumerator TryAddCardFromField(Card crd,int value)
    {
        if (crd != null)
        { UiItemSpawner.Instance.ReplaceOneCardWithLowValue(crd, value); }
        yield return new WaitForSeconds(1f);
    }

}
