using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class Zoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Vector3 originalScale;
    public float zoomFactor = 1.2f;
    public float zoomSpeed = 10f;

    private Vector3 targetScale;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * zoomSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * zoomFactor;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse Clicked now");
        UIAnimationUtility.ShakePosition(gameObject.GetComponent<RectTransform>(), new Vector3(2, 10, 1), 0.5f, 10, 90, Ease.InOutBounce);
        var Damage = GetComponent<CardDisplay>().Card.value1;
        GameManager.instance.PlayerAttack(Damage);

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}
