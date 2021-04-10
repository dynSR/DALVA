using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour
{
    public delegate void StatusEffectApplicationHandler (StatusEffect statusEffectApplied);
    public event StatusEffectApplicationHandler OnApplyingStatusEffect;

    [SerializeField] private List<StatusEffectDurationData> appliedStatusEffects = new List<StatusEffectDurationData>();

    [System.Serializable]
    public class StatusEffectDurationData
    {
        public StatusEffect statusEffect;
        public float duration;

        public StatusEffectDurationData(StatusEffect statusEffect, float duration)
        {
            this.statusEffect = statusEffect;
            this.duration = duration;
        }
    }

    void Update() => ProcessDuration();

    private void LateUpdate() => CheckForExpiredStatusEffect();

    #region Applying an effect
    public void AddNewEffect(StatusEffect newStatusEffect)
    {
        appliedStatusEffects.Add(new StatusEffectDurationData(newStatusEffect, newStatusEffect.StatusEffectDuration));

        OnApplyingStatusEffect?.Invoke(newStatusEffect);
    }

    public void ResetCooldown(StatusEffect newStatusEffect)
    {
        foreach (StatusEffectDurationData durationData in appliedStatusEffects)
        {
            if (durationData.statusEffect == newStatusEffect)
            {
                durationData.duration = newStatusEffect.StatusEffectDuration;
                OnApplyingStatusEffect?.Invoke(newStatusEffect);
            }
        }
    }

    public bool IsEffectAlreadyApplied(StatusEffect statusEffect)
    {
        foreach (StatusEffectDurationData durationData in appliedStatusEffects)
        {
            if (durationData.statusEffect.StatusEffectId == statusEffect.StatusEffectId)
            {
                Debug.Log(statusEffect.StatusEffectName + " will remain for " + durationData.duration.ToString("0.0") + " seconds before expiring!");
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Status Effect Duration Handler Section
    private void ProcessDuration()
    {
        for (int i = 0; i < appliedStatusEffects.Count; i++)
        {
            appliedStatusEffects[i].duration -= Time.deltaTime;
        }
    }

    private void RemoveEffectFromStatusEffectHandler(StatusEffect newStatusEffect)
    {
        for (int i = appliedStatusEffects.Count - 1; i >= 0; i--)
        {
            if (appliedStatusEffects[i].statusEffect == newStatusEffect)
            {
                appliedStatusEffects[i].statusEffect.RemoveEffect(transform);
                appliedStatusEffects.RemoveAt(i);
            }
        }
    }

    private void CheckForExpiredStatusEffect()
    {
        for (int i = appliedStatusEffects.Count - 1; i >= 0; i--)
        {
            if (appliedStatusEffects[i].duration <= 0)
            {
                appliedStatusEffects[i].statusEffect.RemoveEffect(transform);
                appliedStatusEffects.RemoveAt(i);
            }
        }
    }
    #endregion
}