using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Dice : MonoBehaviour, IPointerClickHandler
{
    public Sprite[] diceFaces;        // Assign 6 dice face sprites in the Inspector
    public Image diceImage;           // Assign the UI Image component
    public GameManager gameManager;   // Reference to GameManager
    public float rollTime = 1.0f;     // Total animation time
    public float frameRate = 0.1f;    // Time between frames

    private bool isRolling = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isRolling && gameManager.remainingMoves > 0)
        {
            StartCoroutine(RollDice());
        }
    }

    private IEnumerator RollDice()
    {
        isRolling = true;
        float elapsed = 0f;

        while (elapsed < rollTime)
        {
            int tempNumber = Random.Range(0, diceFaces.Length);
            diceImage.sprite = diceFaces[tempNumber];
            yield return new WaitForSeconds(frameRate);
            elapsed += frameRate;
        }

        int finalNumber = Random.Range(1, 7); // 1 to 6
        diceImage.sprite = diceFaces[finalNumber - 1];
        gameManager.MovePlayer(finalNumber);

        isRolling = false;
    }
}
