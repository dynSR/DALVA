using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShieldable
{
    void ApplyShieldOnTarget(Transform target, float shieldValue, float shieldEffectiveness, bool comesFromAnEffect = false);
}