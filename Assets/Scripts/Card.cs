using System.Collections.Generic;
using UnityEngine;

public enum Packs
{
    Starter,
    Booster1,
    Booster2
}

public enum Rarity
{
    C,   // Common
    UC,  // Uncommon
    R,   // Rare
    UR,  // Ultra_Rare
    SR   // Secret_Rare
}

public enum MultiActionType
{
    None,
    AttackTwice,       // 2x
    SwapValues,        // SW
    BossStun,          // ST
    RollAndSwap,       // RA
    DiscardAndAdd5,    // D+5
    DiscardAndAdd7,    // D+7
    PoisonBoss,        // PO
    BoostDamage,       // A+2
    Select3Card,
    doubleDamageRound,
    ReduceDamageNextTurn,
    doubleHealingLess50,
}

public enum CardActionType
{
    Attack,
    Heal,
    Multi,
    empty
}

[CreateAssetMenu(fileName = "New Card", menuName = "Cards")]
public class Card : ScriptableObject
{
    public string cardId;
    public new string name;
    public string description;
    public Sprite artwork;
    public Sprite type;
    public bool isBoss;
    public bool isPlayerInventory;
    public bool isDefenseDown;

    public int value1; // mana
    public int value2; // attack
    public int value3; // health

    public int cardScore; // Score for scoreboard

    public CardActionType actionType;
    public MultiActionType multiActionType;
    public Packs cardPack;
    public Rarity rarity;

    static Dictionary<string, Card> itemLookupCache;

    public static Card GetFromID(string id)
    {
        itemLookupCache = null;

        if (itemLookupCache == null)
        {
            itemLookupCache = new Dictionary<string, Card>();
            var cardList = Resources.LoadAll<Card>("");

            foreach (var card in cardList)
            {
                if (card == null)
                    continue;

                if (itemLookupCache.ContainsKey(card.cardId))
                {
                    Debug.LogError(string.Format("Duplicate ID found for cards: {0} and {1}", itemLookupCache[card.cardId], card));
                    continue;
                }

                itemLookupCache[card.cardId] = card;
            }
        }

        if (id == null || !itemLookupCache.ContainsKey(id))
            return null;

        return itemLookupCache[id];
    }

    [ContextMenu("Generate Id")]
    void GenerateId()
    {
        cardId = System.Guid.NewGuid().ToString();
    }

    public string GetCardID()
    {
        return cardId;
    }

    // ✅ Auto-calculate score based on rarity
    public void CalculateCardScoreFromRarity()
    {
        switch (rarity)
        {
            case Rarity.C:
                cardScore = 5 + value1;
                break;
            case Rarity.UC:
                cardScore = 10 + value1;
                break;
            case Rarity.R:
                cardScore = 15 + value1;
                break;
            case Rarity.UR:
                cardScore = value1 * 2;
                break;
            case Rarity.SR:
                cardScore = value1 * 3;
                break;
            default:
                cardScore = 0;
                break;
        }
    }

    public int GetCardScore()
    {
        CalculateCardScoreFromRarity();
        return cardScore;
    }

#if UNITY_EDITOR
    // ✅ Auto-update when you change fields in the Inspector
    private void OnValidate()
    {
        CalculateCardScoreFromRarity();
    }
#endif
}
