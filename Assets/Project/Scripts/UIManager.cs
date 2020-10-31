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

    void Start()
    {
        StatusEffectCooldownHandler.OnAddingStatusEffect += UpdateStatuesEffectUI;
        StatusEffectCooldownHandler.OnRemovingStatusEffect += RemoveStatuesEffectFromUI;
    }

    void Update()
    {
        
    }

    void UpdateStatuesEffectUI(StatusEffect statusEffect)
    {
        GameObject statusEffectFeedbackInstance = Instantiate(statusEffectGameObject);
        statusEffectFeedbackInstance.transform.SetParent(statusEffectLayoutGroup);
        statusEffectFeedbackInstance.GetComponent<StatusEffectContainer>().StatuesEffect = statusEffect;
        statusEffectFeedbackInstance.GetComponent<Image>().sprite = statusEffect.StatusEffectIcon;
    }
    void RemoveStatuesEffectFromUI(StatusEffect statusEffect)
    {
        int childcount = statusEffectLayoutGroup.childCount;
        Debug.Log(statusEffectLayoutGroup.childCount);

        for (int i = childcount - 1; i >= 0; i--)
        {
            if (statusEffectLayoutGroup.GetChild(i).GetComponent<StatusEffectContainer>().StatuesEffect == statusEffect)
            {
                Debug.Log("Destroy Expired StatusEffect");
                Destroy(statusEffectLayoutGroup.GetChild(i).gameObject);
            }
        }
    }
}
