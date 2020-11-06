using System.Collections.Generic;
using UnityEngine;

public class CooldownHandler : MonoBehaviour
{
    public delegate void StatusEffectAction(StatusEffect statusEffect);
    public static event StatusEffectAction OnAddingStatusEffect;

    public delegate void AbilityUsedAction(Ability abilityUsed);
    public static event AbilityUsedAction OnAbitilityUsed;

    private bool IsThereMoreThanOneStatusEffectApplied => allStatusEffectApplied.Count > 0;

    [SerializeField] private List<AbilityCooldownData> allAbilitiesOnCooldown = new List<AbilityCooldownData>();

    [SerializeField] private List<StatusEffectDurationData> allStatusEffectApplied = new List<StatusEffectDurationData>();

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

        ApplyStatusEffectDurationOverTime();
        CheckForExpiredStatusEffectDuration();
    }

    #region Abilities Cooldown Handler Section
    public void PutAbilityOnCooldown(Ability ability)
    {
        OnAbitilityUsed?.Invoke(ability);
        allAbilitiesOnCooldown.Add(new AbilityCooldownData(ability, ability.AbilityCooldown));
    }

    private void ApplyAbilityCooldown()
    {
        for (int i = 0; i < allAbilitiesOnCooldown.Count; i++)
        {
            allAbilitiesOnCooldown[i].cooldown -= Time.deltaTime;
        }
    }

    private void CheckForExpiredAbilityCooldown()
    {
        for (int i = allAbilitiesOnCooldown.Count - 1; i >= 0; i--)
        {
            if (allAbilitiesOnCooldown[i].cooldown <= 0)
            {
                allAbilitiesOnCooldown.RemoveAt(i);
            }
        }
    }

    public bool IsAbilityOnCooldown(Ability ability)
    {
        foreach (AbilityCooldownData cooldownData in allAbilitiesOnCooldown)
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
    public void ApplyNewStatusEffectDuration(StatusEffect newStatusEffect)
    {
        //OnAddingStatusEffect?.Invoke(newStatusEffect);
        allStatusEffectApplied.Add(new StatusEffectDurationData(newStatusEffect, newStatusEffect.StatusEffectDuration));
    }

    private void ApplyStatusEffectDurationOverTime()
    {
        for (int i = 0; i < allStatusEffectApplied.Count; i++)
        {
            allStatusEffectApplied[i].duration -= Time.deltaTime;
        }
    }

    public bool AreThereSimilarExistingStatusEffectApplied(StatusEffect newStatusEffectApplied)
    {
        for (int i = 0; i <= allStatusEffectApplied.Count; i++)
        {
            if (IsThereMoreThanOneStatusEffectApplied && allStatusEffectApplied[i].statusEffect.TypeOfEffect == newStatusEffectApplied.TypeOfEffect && allStatusEffectApplied[i].statusEffect.StatusEffectName != newStatusEffectApplied.StatusEffectName)
            {
                Debug.Log(newStatusEffectApplied.StatusEffectName + " is the same that " + allStatusEffectApplied[i].statusEffect.StatusEffectName);
                return true;
            }
        }

        return false;
    }

    public void RemoveStatusEffectOfSameTypeThatHasAlreadyBeenApplied()
    {
        for (int i = 0; i <= allStatusEffectApplied.Count; i++)
        {
            allStatusEffectApplied[i].statusEffect.RemoveStatusEffect();
            allStatusEffectApplied[i].statusEffect.StatusEffectContainer.DestroyContainer();
            allStatusEffectApplied.Remove(allStatusEffectApplied[i]);
        }
    }

    private void CheckForExpiredStatusEffectDuration()
    {
        for (int i = allStatusEffectApplied.Count - 1; i >= 0; i--)
        {
            if (allStatusEffectApplied[i].duration <= 0)
            {
                allStatusEffectApplied[i].statusEffect.RemoveStatusEffect();
                allStatusEffectApplied.RemoveAt(i);
            }
        }
    }

    public bool IsDurationOfStatusEffectAlreadyApplied(StatusEffect statusEffect)
    {
        foreach (StatusEffectDurationData durationData in allStatusEffectApplied)
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
