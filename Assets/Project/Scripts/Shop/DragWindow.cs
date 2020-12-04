using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform DragWindowRectTransform => GetComponent<RectTransform>();

    [SerializeField] private Canvas playerHUD;
    private CanvasGroup DragWindowCanvasGroup => GetComponent<CanvasGroup>();

    private bool LeftClickIsPressed => Input.GetMouseButton(0);

    void Awake()
    {
        DragWindowCanvasGroup.alpha = 1f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (LeftClickIsPressed)
            DragWindowCanvasGroup.alpha = 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging Window");

        if(LeftClickIsPressed)
            DragWindowRectTransform.anchoredPosition += eventData.delta / playerHUD.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragWindowCanvasGroup.alpha = 1f;
    }
}
