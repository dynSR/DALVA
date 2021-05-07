using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatTooltip : UIButtonWithTooltip
{
    [SerializeField] private StatType[] statType;
    [SerializeField] private TextMeshProUGUI detailedStatText;

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
            detailedStatText.gameObject.SetActive(true);
            detailedStatText.SetText(
                " ( "
                + $"<color=#ffffffff>{ LeftSectionTooltip.CharacterStats.GetStat(statType[0]).Value }</color>"
                //LeftSectionTooltip.CharacterStats.GetStat(statType[0]).Value.ToString("0")
                + " + "
                + $"<color=#5dade2>{ LeftSectionTooltip.CharacterStats.GetStat(statType[1]).Value }</color>"
                +  " ) ");
        }  
    }
}