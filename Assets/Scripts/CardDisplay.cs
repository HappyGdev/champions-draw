using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public Card Card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public Image artworkImage;
    public Image ArtworkType;

    public TextMeshProUGUI Value1;
    public TextMeshProUGUI Value2;
    public TextMeshProUGUI Value3;
    //public TextMeshProUGUI cardPoint_txt;

    public int CardIndex;   // ایندکس کارت در PlayerFightcards



    public void DisplayCard()
    {
        
        //if (Card == null)
        //{
        //    Debug.LogError("Card is NULL on CardDisplay! Cannot display.");
        //    Debug.Log(this.gameObject.name);
        //    return;
        //}
        nameText.text = Card.name;
        descriptionText.text = Card.description;

        artworkImage.sprite = Card.artwork;
        ArtworkType.sprite = Card.type;

        Value1.text=Card.value1.ToString();
        Value2.text=Card.value2.ToString();
        Value3.text=Card.value3.ToString();
        //cardPoint_txt.text = Card.cardScore.ToString();
    }

    private void OnEnable()
    {
        GameManager.onCardDisplay += DisplayCard;
        MiniGameManager.onCardDisplay += DisplayCard;

    }
    private void OnDisable()
    {
        GameManager.onCardDisplay -= DisplayCard;
        MiniGameManager.onCardDisplay -= DisplayCard;

    }

}
