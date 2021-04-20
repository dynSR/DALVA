using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour
{
    public delegate void StatusEffectApplicationHandler (StatusEffect statusEffectApplied);
    public event StatusEffectApplicationHandler OnApplyingStatusEffect;

    [SerializeField] private List<StatusEffectDurationData> appliedStatusEffects = new List<StatusEffectDurationData>();
    public List<StatusEffectDurationData> AppliedStatusEffects { get => appliedStatusEffects; }

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
        OnApplyingStatusEffect?.Invoke(newStatusEffect);

        AppliedStatusEffects.Add(new StatusEffectDurationData(newStatusEffect, newStatusEffect.StatusEffectDuration));
    }

    public void ResetCooldown(StatusEffect newStatusEffect)
    {
        foreach (StatusEffectDurationData durationData in AppliedStatusEffects)
        {
            if (durationData.statusEffect.StatusEffectId == newStatusEffect.StatusEffectId)
            {
                durationData.duration = newStatusEffect.StatusEffectDuration;
                OnApplyingStatusEffect?.Invoke(newStatusEffect);
            }
        }
    }

    public bool IsEffectAlreadyApplied(StatusEffect statusEffect)
    {
        foreach (StatusEffectDurationData durationData in AppliedStatusEffects)
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
        for (int i = 0; i < AppliedStatusEffects.Count; i++)
        {
            AppliedStatusEffects[i].duration -= Time.deltaTime;
        }
    }

    private void CheckForExpiredStatusEffect()
    {
        for (int i = AppliedStatusEffects.Count - 1; i >= 0; i--)
        {
            if (AppliedStatusEffects[i].duration <= 0)
            {
                AppliedStatusEffects[i].statusEffect.RemoveEffect(transform);
                AppliedStatusEffects.RemoveAt(i);
            }
        }
    }
    #endregion
}