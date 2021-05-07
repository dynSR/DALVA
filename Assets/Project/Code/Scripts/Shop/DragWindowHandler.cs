using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DragWindowHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform DragWindowRectTransform => GetComponent<RectTransform>();

    [SerializeField] private Canvas playerHUD;
    private CanvasGroup DragWindowCanvasGroup => GetComponent<CanvasGroup>();

    private bool LeftClickIsHeld => Input.GetMouseButton(0);

    void Awake()
    {
        DragWindowCanvasGroup.alpha = 1f;
    }

    void Start () { } 

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (LeftClickIsHeld)
            DragWindowCanvasGroup.alpha = 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging Window");

        if(LeftClickIsHeld)
            DragWindowRectTransform.anchoredPosition += eventData.delta / playerHUD.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragWindowCanvasGroup.alpha = 1f;
    }
}
