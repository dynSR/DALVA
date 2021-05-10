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

    private void OnEnable()
    {
        HideBorder();
        ChangeTextColor(normalColor);
    }

    void Start()
    {
        myButton = GetComponent<Button>();
    }

    void Update()
    {
        if (!myButton.IsInteractable())
        {
            ChangeTextColor(normalColor);
            DisplayBorder();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeTextColor(pressedColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeTextColor(highlightColor);

        DisplayBorder();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeTextColor(normalColor);

        HideBorder();
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

    private void DisplayBorder()
    {
        if (myBorder != null) myBorder.SetActive(true);
    }

    public void HideBorder()
    {
        if (myBorder != null) myBorder.SetActive(false);
    }
}
