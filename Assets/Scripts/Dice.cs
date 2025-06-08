using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dice : MonoBehaviour, IPointerClickHandler
{
    public Sprite[] diceFaces; // Assign 6 dice face sprites in the inspector
    public Image diceImage;    // Assign the UI Image component
    public GameManager gameManager; // Assign the GameManager

    public void OnPointerClick(PointerEventData eventData)
    {
        RollDice();
    }

    public void RollDice()
    {
        if (gameManager.remainingMoves <= 0) return;

        int rolledNumber = Random.Range(1, 7);
        diceImage.sprite = diceFaces[rolledNumber - 1];
        gameManager.MovePlayer(rolledNumber);
    }
}
