using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("STATUS EFFECT PARAMETERS")]
    [SerializeField] private Transform statusEffectLayoutGroup;
    [SerializeField] private GameObject statusEffectGameObject;

    public static UIManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        CooldownHandler.OnAddingStatusEffect -= UpdateStatusEffectUI;
    }

    void Start()
    {
        CooldownHandler.OnAddingStatusEffect += UpdateStatusEffectUI;
    }

    void UpdateStatusEffectUI(StatusEffect statusEffect)
    {
        GameObject statusEffectFeedbackInstance = Instantiate(statusEffectGameObject) as GameObject;
        statusEffectFeedbackInstance.transform.SetParent(statusEffectLayoutGroup);

        statusEffectFeedbackInstance.GetComponent<StatusEffectContainer>().StatusEffect = statusEffect;
        statusEffect.StatusEffectContainer = statusEffectFeedbackInstance.GetComponent<StatusEffectContainer>();
    }
}
