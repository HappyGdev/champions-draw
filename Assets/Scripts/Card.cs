using UnityEngine;
public enum Packs
{
    Starter, 
    Booster1, 
    Booster2
}
public enum Rarity
{
    C,//Common
    UC,// Uncommon
    R,// Rare
    UR,// Ultra_Rare
    SR//Secret_Rare
}
public enum MultiActionType
{
    None,
    AttackTwice,     // 2x
    SwapValues,      // SW
    BossStun,        // ST
    RollAndSwap,     // RA
    DiscardAndAdd5,  // D+5
    DiscardAndAdd7,  // D+7
    PoisonBoss,      // PO
    BoostDamage,      // A+2
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
    public new string name;
    public string description;

    public Sprite artwork;
    public Sprite type;

    public bool isBoss;
    public bool isPlayerInventory;
    public bool isDefenseDown;

    public int value1;  // mana
    public int value2;  // attack
    public int value3;  // health

    public CardActionType actionType; // Add this line for enum usage
    public MultiActionType multiActionType;  // مخصوص کارت‌های Multi
    public Packs cardPack;  // مخصوص کارت‌های Multi
    public Rarity rarity;   // برای rare

}
