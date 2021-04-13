using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityButton : InteractiveButton
{
    [SerializeField] private Ability buttonAbility;
    public Ability ButtonAbility { get => buttonAbility; }

    void Start() => SetButton(ButtonAbility.AbilityIcon, ButtonAbility.AbilityCost);

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        //PlayerShop.BuyItem(ButtonAbility);
    }
}
