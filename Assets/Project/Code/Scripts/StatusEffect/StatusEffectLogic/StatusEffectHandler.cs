using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour
{
    public delegate void StatusEffectApplicationHandler (StatusEffectLogic statusEffectApplied);
    public static event StatusEffectApplicationHandler OnApplyingStatusEffect;

    [SerializeField] private List<StatusEffectDurationData> statusEffectApplied = new List<StatusEffectDurationData>();

    [System.Serializable]
    public class StatusEffectDurationData
    {
        public StatusEffectLogic statusEffect;
        public float duration;

        public StatusEffectDurationData(StatusEffectLogic statusEffect, float duration)
        {
            this.statusEffect = statusEffect;
            this.duration = duration;
        }
    }

    void Update()
    {
        ProcessDuration();
        //HandleExpiredEffect();
    }

    #region Applying an effect
    public void AddNewEffect(StatusEffectLogic newStatusEffect)
    {
        statusEffectApplied.Add(new StatusEffectDurationData(newStatusEffect, newStatusEffect.StatusEffect.StatusEffectDuration));

        OnApplyingStatusEffect?.Invoke(newStatusEffect);
    }

    public void ResetCooldown(StatusEffectLogic newStatusEffect)
    {
        foreach (StatusEffectDurationData durationData in statusEffectApplied)
        {
            if (durationData.statusEffect == newStatusEffect)
            {
                durationData.duration = newStatusEffect.StatusEffect.StatusEffectDuration;
                OnApplyingStatusEffect?.Invoke(newStatusEffect);
            }
        }
    }

    public StatusEffectDurationData GetEffect(StatusEffect effect)
    {
        StatusEffectDurationData data = null;

        for (int i = statusEffectApplied.Count - 1; i >= 0; i--)
        {
            if (statusEffectApplied[i].statusEffect == effect)
            {
                data = statusEffectApplied[i];
            }
        }

        return data;
    }

    public bool IsEffectAlreadyApplied(StatusEffectLogic statusEffect)
    {
        foreach (StatusEffectDurationData durationData in statusEffectApplied)
        {
            if (durationData.statusEffect.StatusEffect.StatusEffectId == statusEffect.StatusEffect.StatusEffectId)
            {
                Debug.Log(statusEffect.StatusEffect.StatusEffectName + " will remain for " + durationData.duration.ToString("0.0") + " seconds before expiring!");
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Status Effect Duration Handler Section
    private void ProcessDuration()
    {
        for (int i = 0; i < statusEffectApplied.Count; i++)
        {
            statusEffectApplied[i].duration -= Time.deltaTime;
        }
    }

    public void RemoveEffectFromStatusEffectHandler(StatusEffectLogic newStatusEffect)
    {
        for (int i = statusEffectApplied.Count - 1; i >= 0; i--)
        {
            if (statusEffectApplied[i].statusEffect == newStatusEffect)
            {
                statusEffectApplied[i].statusEffect.RemoveEffect();
                statusEffectApplied.RemoveAt(i);
            }
        }
    }

    private void HandleExpiredEffect()
    {
        for (int i = statusEffectApplied.Count - 1; i >= 0; i--)
        {
            if (statusEffectApplied[i].duration <= 0)
            {
                statusEffectApplied[i].statusEffect.RemoveEffect();
                statusEffectApplied.RemoveAt(i);
            }
        }
    }
    #endregion
}