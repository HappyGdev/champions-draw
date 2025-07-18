using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectManager : MonoBehaviour
{
    public static CardEffectManager Instance;

    private bool isBossTurnSkipped = false;
    private bool damageBoostActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ApplyCardEffect(Card card)
    {
        switch (card.actionType)
        {
            case CardActionType.Attack:
                int dmg = card.value1;   ////damageBoostActive ? card.value2 + 2 : card.value2;
                HealthBar.instance.BossTakeDamage(dmg);
                //GameManager.instance.BossAttackPlayer(dmg);
                break;

            case CardActionType.Heal:
                //HealPlayer(card.value3);
                int hlth = card.value1;   
                HealthBar.instance.PlayerTakeDamage(-hlth);
                break;

            case CardActionType.Multi:
                HandleMultiAction(card);
                break;

            case CardActionType.empty:
                Debug.Log("Empty card");
                break;
        }
    }

    //private void HealPlayer(int amount)
    //{
    //    var hb = HealthBar.instance;
    //    hb.PlayercurrentHealth = Mathf.Min(hb.PlayercurrentHealth + amount, hb.maxPlayerHealth);
    //    hb.UpdatePlayerHealthUI();
    //}

    private void HandleMultiAction(Card card)
    {
        switch (card.multiActionType)
        {
            case MultiActionType.AttackTwice:
                StartCoroutine(AttackTwiceRoutine());
                break;

            case MultiActionType.SwapValues:
                Debug.Log("Swapping values of two cards (not implemented).");
                break;

            case MultiActionType.BossStun:
                isBossTurnSkipped = true;
                Debug.Log("Boss will skip next turn.");
                break;

            case MultiActionType.RollAndSwap:
                StartCoroutine(RollAgainAndSwapRoutine());
                break;

            case MultiActionType.DiscardAndAdd5:
                TryAddCardFromField(maxValue: 5);
                break;

            case MultiActionType.DiscardAndAdd7:
                TryAddCardFromField(maxValue: 7);
                break;

            case MultiActionType.PoisonBoss:
                StartCoroutine(PoisonBossRoutine());
                break;

            case MultiActionType.BoostDamage:
                damageBoostActive = true;
                Debug.Log("Next attacks deal +2 damage.");
                break;

            default:
                Debug.LogWarning("Unknown MultiActionType");
                break;
        }
    }

    private IEnumerator AttackTwiceRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.BossAttackPlayer(10);
        yield return new WaitForSeconds(0.3f);
        GameManager.instance.BossAttackPlayer(10);
    }

    private IEnumerator PoisonBossRoutine()
    {
        int poisonTurns = 3;
        int poisonDamage = 5;

        for (int i = 0; i < poisonTurns; i++)
        {
            Debug.Log($"Poison tick {i + 1}: -{poisonDamage} HP");
            HealthBar.instance.BossTakeDamage(poisonDamage);
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator RollAgainAndSwapRoutine()
    {
        Debug.Log("Player rolls again and swaps a card (to be implemented).");
        // تو این قسمت باید از GameManager تابعی برای roll مجدد صدا بزنی
        yield return null;
    }

    private void TryAddCardFromField(int maxValue)
    {
        Debug.Log($"Try to add card from field with value <= {maxValue} (to be implemented).");
        // انتخاب کارت از روی زمین + حذف یکی از کارت‌های دست فعلی
    }

    public bool ShouldSkipBossTurn()
    {
        if (isBossTurnSkipped)
        {
            isBossTurnSkipped = false;
            return true;
        }
        return false;
    }

    public void ResetDamageBoost()
    {
        damageBoostActive = false;
    }
}
