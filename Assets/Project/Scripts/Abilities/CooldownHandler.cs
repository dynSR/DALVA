using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownHandler : MonoBehaviour
{
    public delegate void StatusEffectAction(StatusEffect statusEffect);
    public static event StatusEffectAction OnAddingStatusEffect;
    public static event StatusEffectAction OnRemovingStatusEffect;

    public delegate void AbilityUsedAction(Ability abilityUsed);
    public static event AbilityUsedAction OnAbitilityUsed;

    private bool IsThereMoreThanOneStatusEffectApplied => statusEffectApplied.Count > 0;

    [SerializeField] private List<AbilityCooldownData> abilitiesOnCooldown = new List<AbilityCooldownData>();

    [SerializeField] private List<StatusEffectDurationData> statusEffectApplied = new List<StatusEffectDurationData>();

    [System.Serializable]
    private class AbilityCooldownData
    {
        public Ability ability;
        public float cooldown; 

        public AbilityCooldownData(Ability ability, float cooldown)
        {
            this.ability = ability;
            this.cooldown = cooldown;
        }
    }

    [System.Serializable]
    private class StatusEffectDurationData
    {
        public StatusEffect statusEffect;
        public float duration;

        public StatusEffectDurationData(StatusEffect statusEffect, float duration)
        {
            this.statusEffect = statusEffect;
            this.duration = duration;
        }
    }

    private void Update()
    {
        ApplyAbilityCooldown();
        CheckForExpiredAbilityCooldown();

        ApplyStatusEffectDuration();
        CheckForExpiredStatusEffectDuration();
    }

    #region Abilities Cooldown Handler Section
    public void PutAbilityOnCooldown(Ability ability)
    {
        OnAbitilityUsed?.Invoke(ability);
        abilitiesOnCooldown.Add(new AbilityCooldownData(ability, ability.AbilityCooldown));
    }

    private void ApplyAbilityCooldown()
    {
        for (int i = 0; i < abilitiesOnCooldown.Count; i++)
        {
            abilitiesOnCooldown[i].cooldown -= Time.deltaTime;
        }
    }

    private void CheckForExpiredAbilityCooldown()
    {
        for (int i = abilitiesOnCooldown.Count - 1; i >= 0; i--)
        {
            if (abilitiesOnCooldown[i].cooldown <= 0)
            {
                abilitiesOnCooldown.RemoveAt(i);
            }
        }
    }

    public bool IsAbilityOnCooldown(Ability ability)
    {
        foreach (AbilityCooldownData cooldownData in abilitiesOnCooldown)
        {
            if (cooldownData.ability == ability)
            {
                Debug.Log(ability.AbilityName + " is on cooldown for " + cooldownData.cooldown.ToString("0.0") + " seconds");
                return true;
            }
        }
        
        return false;
    }
    #endregion

    #region Status Effect Duration Handler Section
    public void ApplyStatusEffectDuration(StatusEffect statusEffect)
    {
        OnAddingStatusEffect?.Invoke(statusEffect);

        statusEffectApplied.Add(new StatusEffectDurationData(statusEffect, statusEffect.StatusEffectDuration));
    }

    private void ApplyStatusEffectDuration()
    {
        for (int i = 0; i < statusEffectApplied.Count; i++)
        {
            statusEffectApplied[i].duration -= Time.deltaTime;
        }
    }

    public bool CheckForSimilarExistingStatusEffect(StatusEffect statusEffect)
    {
        //for (int i = 0; i < statusEffectApplied.Count; i++)
        //{
        //    if (IsThereMoreThanOneStatusEffectApplied && statusEffectApplied[i].statusEffect.TypeOfEffect == statusEffect.TypeOfEffect && statusEffectApplied[i].statusEffect.StatusEffectName != statusEffect.StatusEffectName)
        //    {
        //        Debug.Log(statusEffect.StatusEffectName + " is the same that " + statusEffectApplied[i].statusEffect.StatusEffectName);
        //        statusEffectApplied[i].statusEffect.RemoveStatusEffect();
        //    }
        //}

        foreach (StatusEffectDurationData durationData in statusEffectApplied)
        {
            if (IsThereMoreThanOneStatusEffectApplied && durationData.statusEffect.TypeOfEffect == statusEffect.TypeOfEffect && durationData.statusEffect.StatusEffectName != statusEffect.StatusEffectName)
            {
                Debug.Log(statusEffect.StatusEffectName + " is the same that " + durationData.statusEffect.StatusEffectName);

                durationData.statusEffect.RemoveStatusEffect();
                durationData.statusEffect.StatusEffectContainer.DestroyContainer();
                statusEffectApplied.Remove(durationData);

                return true;
            }
        }

        return false;
    }

    private void CheckForExpiredStatusEffectDuration()
    {
        for (int i = statusEffectApplied.Count - 1; i >= 0; i--)
        {
            if (statusEffectApplied[i].duration <= 0)
            {
                statusEffectApplied[i].statusEffect.RemoveStatusEffect();
                statusEffectApplied.RemoveAt(i);
            }
        }
    }

    public bool IsDurationOfStatusEffectApplied(StatusEffect statusEffect)
    {
        foreach (StatusEffectDurationData durationData in statusEffectApplied)
        {
            if (durationData.statusEffect == statusEffect)
            {
                Debug.Log(statusEffect.StatusEffectName + " will remain for " + durationData.duration.ToString("0.0") + " seconds before expiring!");
                return true;
            }
        }

        return false;
    }
    #endregion
}
