using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PanelTriggerToggle : MonoBehaviour
{
    public RectTransform panel;            //  UI panel
    public float duration = 0.5f;          // Animation duration
    //public AudioSource audioSource;        // Audio source to play sound
    //public AudioClip openSound;
    //public AudioClip closeSound;

    private Vector2 shownPos;
    private Vector2 hiddenPos;
    private bool isOpen = false;
    private CanvasGroup canvasGroup;

    //void Start()
    //{
    //    float quarterScreen = Screen.width * 0.25f;
    //    float panelWidth = panel.rect.width;

    //    shownPos = new Vector2(quarterScreen - panelWidth / 2f, panel.anchoredPosition.y);
    //    hiddenPos = new Vector2(-panelWidth, panel.anchoredPosition.y);

    //    panel.anchoredPosition = hiddenPos;

    //    canvasGroup = panel.GetComponent<CanvasGroup>();
    //    if (!canvasGroup)
    //        canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();

    //    canvasGroup.alpha = 0f;
    //}

    public void TogglePanel()
    {
        //panel.DOKill(); // Kill any running tweens
        Debug.Log("Clicking on button");
        if (isOpen)
        {
            // Hide
            UIAnimationUtility.MoveUIElement(panel.GetComponent<RectTransform>(), -500, 0.5f, Ease.InOutFlash);

            //panel.DOAnchorPos(hiddenPos, duration).SetEase(Ease.InOutQuad);
            //canvasGroup.DOFade(0f, duration);
            //if (closeSound) audioSource?.PlayOneShot(closeSound);
        }
        else
        {
            // Show
            UIAnimationUtility.MoveUIElement(panel.GetComponent<RectTransform>(), 500, 0.5f, Ease.InOutFlash);

            //if (openSound) audioSource?.PlayOneShot(openSound);
        }

        isOpen = !isOpen;
    }
}
