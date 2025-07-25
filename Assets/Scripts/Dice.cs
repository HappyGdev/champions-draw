using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DG.Tweening.Core.Easing;
using UnityEngine.EventSystems;
using System.Collections;

public class Dice : MonoBehaviour, IPointerClickHandler
{
    public Sprite[] diceFaces;        // Assign 6 dice face sprites in the Inspector
    public Image diceImage;           // Assign the UI Image component
    //public GameManager gameManager;   // Reference to GameManager
    public float rollTime = 1.0f;     // Total animation time
    public float frameRate = 0.1f;    // Time between frames
    private AudioSource audioSource;
    private bool isRolling = false;

    private void Awake()
    {    audioSource = GetComponent<AudioSource>();  }

    //Click on Dice
    public void OnPointerClick(PointerEventData eventData)
    {
        //If Player is Moving Dont Roll Dice
        if (GameManager.instance.isMoving)
            return;

        if (!isRolling && GameManager.instance.remainingMoves > 0)
        {
            //Add some Dice Animation
            UIAnimationUtility.PunchRotation(diceImage.rectTransform, new Vector3(20,20,180), 0.5f, 10, 90, Ease.InOutFlash);

            // play dice sound
            if (!audioSource.isPlaying) {
                audioSource.Play();
            }

            GameManager.instance.SetSampleCardNull();
            //Start dice Logic
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

        //Call Game Manager Move Player
        GameManager.instance.MovePlayer(finalNumber);

        isRolling = false;
    }
}
