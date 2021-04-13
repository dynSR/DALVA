using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(
        Transform character,
        float targetPhysicalResistances, 
        float targetMagicalResistances, 
        float characterPhysicalPower, 
        float characterMagicalPower, 
        float characterCriticalStrikeChance, 
        float characterCriticalStrikeMultiplier, 
        float characterPhysicalPenetration, 
        float characterMagicalPenetration,
        float characterIncomingDamageReduction);
}
