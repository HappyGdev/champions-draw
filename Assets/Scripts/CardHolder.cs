using System.Collections.Generic;
using UnityEngine;

public class CardHolder : MonoBehaviour
{
    private void OnEnable()
    {
        StoryManagere.OnBossCards += SetBossObjects;
    }
    private void OnDisable()
    {
        StoryManagere.OnBossCards -= SetBossObjects;
    }
    //  public List<Card> CardHold = new List<Card>();    
    public List<Card> BossCard = new List<Card>();

    public List<Card> StarterCards = new List<Card>();
    public List<Card> BoosterPack1 = new List<Card>();
    public List<Card> BoosterPack2= new List<Card>();

    public List<Card> PlayerAvaiableCards = new List<Card>();

    public List<Card> allCardsInGame =new List<Card>();

    public void UpdatePlayerAvailableCards(List<string> newCards)
    {
        PlayerAvaiableCards.Clear();

        foreach(string newCard in newCards)
        {
            PlayerAvaiableCards.Add(Card.GetFromID(newCard));
        }
    }
    public void SetBossObjects(List<Card> bossList)
    {
        BossCard.Clear();             // clear old data
        BossCard.AddRange(bossList);  // copy all from boss list
    }

}
