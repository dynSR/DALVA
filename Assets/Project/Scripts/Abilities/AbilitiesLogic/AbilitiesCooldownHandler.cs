using System.Collections.Generic;
using UnityEngine;

public class AbilitiesCooldownHandler : MonoBehaviour
{
    public delegate void AbilityUsedAction(Ability abilityUsed);
    public static event AbilityUsedAction OnAbitilityUsed;

    [SerializeField] private List<AbilityCooldownData> allAbilitiesOnCooldown = new List<AbilityCooldownData>();


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

    private void Update()
    {
        ApplyAbilityCooldown();
        CheckForExpiredAbilityCooldown();
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

}
