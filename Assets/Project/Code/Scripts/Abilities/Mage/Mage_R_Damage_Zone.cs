using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage_R_Damage_Zone : MonoBehaviour
{
    EntityStats UserStats { get; set; }
    public AbilityLogic AbilityUsed { get; set; }
    public bool CanMark { get; set; }

    public void SetZone(
        EntityStats userStats,
        AbilityLogic ability,
        bool canMark)
    {
        UserStats = userStats;
        AbilityUsed = ability;
        CanMark = canMark;
    }

    private void OnTriggerEnter(Collider other)
    {
        EntityStats targetStats = other.GetComponent<EntityStats>();

        if (targetStats != null && !targetStats.IsDead)
        {
            //Consume d'abord et applique ensuite si peut

            AbilityUsed.ApplyingDamageOnTarget(other);
        }
    }
}