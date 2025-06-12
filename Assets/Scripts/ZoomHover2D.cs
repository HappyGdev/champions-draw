using UnityEngine;

public class ZoomHover2D : MonoBehaviour
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

    void OnMouseEnter()
    {
        targetScale = originalScale * zoomFactor;
    }

    void OnMouseExit()
    {
        targetScale = originalScale;
    }
}
