using UnityEngine;

public enum CardActionType
{
    Attack,
    Heal,
    Multi,
}

[CreateAssetMenu(fileName = "New Card", menuName = "Cards")]
public class Card : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite artwork;
    public Sprite type;

    public bool isBoss;

    public int value1;  // mana
    public int value2;  // attack
    public int value3;  // health

    public CardActionType actionType; // Add this line for enum usage
}
