using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHandler : MonoBehaviour
{
    public delegate void StatusEffectApplication(StatusEffectSystem statusEffectApplied);
    public static event StatusEffectApplication OnApplyingStatusEffectEvent;

    [SerializeField] private List<StatusEffectDurationData> statusEffectApplied = new List<StatusEffectDurationData>();

    [System.Serializable]
    private class StatusEffectDurationData
    {
        public StatusEffectSystem statusEffect;
        public float duration;

        public StatusEffectDurationData(StatusEffectSystem statusEffect, float duration)
        {
            this.statusEffect = statusEffect;
            this.duration = duration;
        }
    }

    void Update()
    {
        ProcessDuration();
        HandleExpiredEffect();
    }

    #region Applying an effect
    public void AddNewEffect(StatusEffectSystem newStatusEffect)
    {
        statusEffectApplied.Add(new StatusEffectDurationData(newStatusEffect, newStatusEffect.StatusEffect.StatusEffectDuration));

        OnApplyingStatusEffectEvent?.Invoke(newStatusEffect);
    }

    public bool IsEffectAlreadyApplied(StatusEffectSystem statusEffect)
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