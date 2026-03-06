using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPressAnimation : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    [Header("Scale Settings")]
    public float pressedScale = 0.92f;
    public float overshootScale = 1.03f;
    public float animationSpeed = 25f;

    private Vector3 originalScale;
    private Vector3 currentTarget;

    void Awake()
    {
        originalScale = transform.localScale;
        currentTarget = originalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            currentTarget,
            Time.unscaledDeltaTime * animationSpeed
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        currentTarget = originalScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(Overshoot());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetScale();
    }

    private System.Collections.IEnumerator Overshoot()
    {
        currentTarget = originalScale * overshootScale;
        yield return new WaitForSecondsRealtime(0.04f);
        ResetScale();
    }

    private void ResetScale()
    {
        currentTarget = originalScale;
    }

    void OnDisable()
    {
        transform.localScale = originalScale;
    }
}
