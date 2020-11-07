using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [Header("STATUS EFFECT PARAMETERS")]
    [SerializeField] private Transform statusEffectLayoutGroup;
    [SerializeField] private GameObject statusEffectGameObject;

    private void OnDestroy()
    {
        StatusEffectHandler.OnAddingStatusEffect -= UpdateStatusEffectUI;
    }

    void Start()
    {
        StatusEffectHandler.OnAddingStatusEffect += UpdateStatusEffectUI;
    }

    public void UpdateStatusEffectUI(StatusEffect statusEffect)
    {
        GameObject statusEffectFeedbackInstance = Instantiate(statusEffectGameObject) as GameObject;
        statusEffectFeedbackInstance.transform.SetParent(statusEffectLayoutGroup);

        statusEffectFeedbackInstance.GetComponent<StatusEffectContainer>().ContainedStatusEffect = statusEffect;
        statusEffect.StatusEffectContainer = statusEffectFeedbackInstance.GetComponent<StatusEffectContainer>();
    }
}
