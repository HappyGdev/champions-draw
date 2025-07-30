using System.Collections.Generic;
using UnityEngine;

public class CardHolder : MonoBehaviour
{
    public List<Card> CardHold = new List<Card>();    
    public List<Card> BossCard = new List<Card>();

    public List<Card> StarterCards = new List<Card>();
    public List<Card> PlayerAvaiableCards = new List<Card>();

    List<Card> achievedCards = new List<Card>();

    public void UpdatePlayerAvailableCards(List<string> newCards)
    {
        PlayerAvaiableCards.Clear();
        PlayerAvaiableCards.AddRange(StarterCards);

        foreach(string newCard in newCards)
        {
            PlayerAvaiableCards.Add(Card.GetFromID(newCard));
        }
    }
}
