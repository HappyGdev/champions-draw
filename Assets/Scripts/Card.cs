using UnityEngine;

[CreateAssetMenu(fileName = "New Card",menuName ="Cards" )]
public class Card : ScriptableObject
{
    public new string name;
    public string description;

    public Sprite artwork;
    public Sprite type;


    public int value1;  //mana
    public int value2;  //attack
    public int value3;  //health

}
