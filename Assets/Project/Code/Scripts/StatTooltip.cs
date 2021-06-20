using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatTooltip : UIButtonWithTooltip
{
    [SerializeField] private StatType[] statType;
    [SerializeField] private TextMeshProUGUI detailedStatText;

    public Color firstStatColor = Color.white;
    public Color secondStatColor = Color.white;

    #region Refs
    private LeftSectionTooltip LeftSectionTooltip => GetComponentInParent<LeftSectionTooltip>();
    #endregion

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        SetDetailedStatText();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    private void SetDetailedStatText()
    {
        if (detailedStatText != null)
        {
            ColorUtility.ToHtmlStringRGBA(firstStatColor);

            detailedStatText.gameObject.SetActive(true);

            detailedStatText.text = 
                string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(firstStatColor), LeftSectionTooltip.CharacterStats.GetStat(statType[0]).Value)
                + 
                    " + "
                +
                 string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(secondStatColor), LeftSectionTooltip.CharacterStats.GetStat(statType[1]).Value);

            /*detailedStatText.SetText(
            " ( "
                + $"<color=color>{ LeftSectionTooltip.CharacterStats.GetStat(statType[0]).Value }</color>"
                //LeftSectionTooltip.CharacterStats.GetStat(statType[0]).Value.ToString("0")
                + " + "
                + $"<color=#5dade2>{ LeftSectionTooltip.CharacterStats.GetStat(statType[1]).Value }</color>"
                +  " ) ");*/
        }  
    }
}