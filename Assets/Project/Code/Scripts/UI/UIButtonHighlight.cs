using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color pressedColor;

    public GameObject myBorder;

    private Button myButton;

    public bool isAMainMenuButton;

    public Color HighlightColor { get => highlightColor; }
    public Color NormalColor { get => normalColor; }

    private void OnEnable()
    {
        HideBorder();
        ChangeTextColor(NormalColor);
    }

    void Start()
    {
        myButton = GetComponent<Button>();
    }

    void LateUpdate()
    {
        //J'en ai besoin pour les boutons du shop
        if (!myButton.IsInteractable())
        {
            ChangeTextColor(NormalColor);
            if(!isAMainMenuButton) HideBorder();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Click on the button", transform);

        if (myButton.IsInteractable())
            ChangeTextColor(pressedColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("In the button", transform);

        if (myButton.IsInteractable())
            ChangeTextColor(HighlightColor);

        if (isAMainMenuButton && !myButton.IsInteractable()) return;

        if (myButton.IsInteractable())
            DisplayBorder();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Outside of the button", transform);

        ChangeTextColor(NormalColor);

        if (isAMainMenuButton && !myButton.IsInteractable()) return;

        if (myButton.IsInteractable())
            HideBorder();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ChangeTextColor(HighlightColor);
    }

    public void ChangeTextColor(Color colorToAssign)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            TextMeshProUGUI text = transform.GetChild(i).GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                transform.GetChild(i).GetComponent<TextMeshProUGUI>().color = colorToAssign;
            }
        }
    }

    public void DisplayBorder()
    {
        if (myBorder != null && !myBorder.activeInHierarchy) myBorder.SetActive(true);
    }

    public void HideBorder()
    {
        if (myBorder != null && myBorder.activeInHierarchy) myBorder.SetActive(false);
    }
}
