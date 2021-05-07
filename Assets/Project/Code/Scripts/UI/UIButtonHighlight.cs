using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color pressedColor;

    private Button myButton;

    void Start()
    {
        myButton = GetComponent<Button>();
    }

    void Update()
    {
        if (!myButton.IsInteractable()) ChangeTextColor(normalColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeTextColor(pressedColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeTextColor(highlightColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeTextColor(normalColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ChangeTextColor(highlightColor);
    }

    void ChangeTextColor(Color colorToAssign)
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
}
