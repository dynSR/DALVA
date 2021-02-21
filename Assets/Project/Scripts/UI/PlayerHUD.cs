using Photon.Pun;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [Header("STATUS EFFECT PARAMETERS")]
    [SerializeField] private Transform statusEffectLayoutGroup;
    [SerializeField] private GameObject statusEffectGameObject;

    void OnEnable()
    {
        StatusEffectHandler.OnApplyingStatusEffect += UpdateStatusEffectUI;
    }

    void OnDisable()
    {
        StatusEffectHandler.OnApplyingStatusEffect -= UpdateStatusEffectUI;
    }

    public void UpdateStatusEffectUI(StatusEffect statusEffect)
    {
        GameObject statusEffectFeedbackInstance = Instantiate(statusEffectGameObject) as GameObject;
        statusEffectFeedbackInstance.transform.SetParent(statusEffectLayoutGroup);

        statusEffectFeedbackInstance.GetComponent<StatusEffectContainer>().ContainedStatusEffect = statusEffect;
        statusEffect.StatusEffectContainer = statusEffectFeedbackInstance.GetComponent<StatusEffectContainer>();
    }
}