using System.Collections.Generic;
using UnityEngine;

public class AbilitiesCooldownHandler : MonoBehaviour
{
    public delegate void AbilityUsedAction(AbilityLogic abilityUsed);
    public static event AbilityUsedAction OnAbitilityUsed;

    [SerializeField] private List<AbilityCooldownData> allAbilitiesOnCooldown = new List<AbilityCooldownData>();

    [System.Serializable]
    private class AbilityCooldownData
    {
        public AbilityLogic ability;
        public float cooldown;

        public AbilityCooldownData(AbilityLogic ability, float cooldown)
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
    public void PutAbilityOnCooldown(AbilityLogic ability)
    {
        OnAbitilityUsed?.Invoke(ability);
        allAbilitiesOnCooldown.Add(new AbilityCooldownData(ability, ability.Ability.AbilityCooldown));
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
        for (int i = 0; i < allAbilitiesOnCooldown.Count; i++)
        {
            if (allAbilitiesOnCooldown[i].cooldown <= 0)
            {
                if (!allAbilitiesOnCooldown[i].ability.CanBeUsed)
                    allAbilitiesOnCooldown[i].ability.CanBeUsed = true;

                allAbilitiesOnCooldown.RemoveAt(i);
            }
        }

        //for (int i = allAbilitiesOnCooldown.Count - 1; i >= 0; i--)
        //{
        //    if (allAbilitiesOnCooldown[i].cooldown <= 0)
        //    {
        //        if (!allAbilitiesOnCooldown[i].ability.CanBeUsed)
        //            allAbilitiesOnCooldown[i].ability.CanBeUsed = true;

        //        allAbilitiesOnCooldown.RemoveAt(i);
        //    }
        //}
    }

    public bool IsAbilityOnCooldown(AbilityLogic ability)
    {
        for (int i = allAbilitiesOnCooldown.Count - 1; i >= 0; i--)
        {
            if (allAbilitiesOnCooldown[i].ability == ability)
            {
                //Debug.Log(ability.Ability.AbilityName + " is on cooldown for " + allAbilitiesOnCooldown[i].cooldown.ToString("0.0") + " seconds");
                return true;
            }
        }

        return false;
    }
    #endregion

}
