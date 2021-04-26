using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public abstract class SelectIcon : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Refs
    protected PlayerHUDManager ShopWindow => GetComponentInParent<Transform>().GetComponentInParent<PlayerHUDManager>();
    protected Transform SelectionIcon => transform.GetChild(0).transform;
    #endregion

    [SerializeField] private GameObject tooltip;
    public bool IsSelected { get; set; }
    public GameObject Tooltip { get => tooltip; }

    void Awake()
    {
        SelectionIcon.gameObject.SetActive(false);
    }

    protected abstract void SetSelection();
    public abstract void ResetSelection();

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer in " + gameObject.name);

        if (!IsSelected)
            DisplayIcon();

        DisplayTooltip(Tooltip);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (ShopWindow.IsShopWindowOpen 
            && UtilityClass.LeftClickIsPressedOnUIElement(eventData))
        {
            if (!IsSelected)
            {
                Debug.Log("Pointer Click, toggle on");
                ToggleOn();
                SetSelection();
            }
            else if (IsSelected)
            {
                Debug.Log("Pointer Click, toggle off");
                ToggleOff();
                ResetSelection();
            }
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer out of " + gameObject.name);

        if (!IsSelected)
            HideIcon();

        HideTooltip(Tooltip);
    }

    #region Toggle On / Off Selection
    protected void ToggleOn()
    {
        Debug.Log("ICON WASNT SHOWN");
        DisplayIcon();
        IsSelected = true;
    }

    public void ToggleOff()
    {
        Debug.Log("ICON WAS ALREADY SHOWN");
        HideIcon();
        IsSelected = false;
    }
    #endregion

    #region Display / Hide Icon
    protected void DisplayIcon()
    {
        if (!SelectionIcon.gameObject.activeInHierarchy)
            SelectionIcon.gameObject.SetActive(true);
    }

    protected void HideIcon()
    {
        if (SelectionIcon.gameObject.activeInHierarchy)
            SelectionIcon.gameObject.SetActive(false);
    }

    void DisplayTooltip(GameObject tooltipObject)
    {
        if (!tooltipObject.activeInHierarchy)
            tooltipObject.SetActive(true);
    }

    public void HideTooltip(GameObject tooltipObject)
    {
        if(tooltipObject.activeInHierarchy)
            tooltipObject.SetActive(false);
    }
    #endregion
}