using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectCooldownHandler : MonoBehaviour
{
    public delegate void StatusEffectAction(StatusEffect statusEffect);
    public static event StatusEffectAction OnAddingStatusEffect;
    public static event StatusEffectAction OnRemovingStatusEffect;

    [SerializeField] private List<CooldownData> statusEffectOnCooldown = new List<CooldownData>();

    [System.Serializable]
    private class CooldownData
    {
        public StatusEffect statusEffect;
        public float buffDuration;

        public CooldownData(StatusEffect statusEffect, float duration)
        {
            this.statusEffect = statusEffect;
            this.buffDuration = duration;
        }
    }

    private void Update()
    {
        for (int i = 0; i < statusEffectOnCooldown.Count; i++)
        {
            statusEffectOnCooldown[i].buffDuration -= Time.deltaTime;
        }

        RemoveStatusEffect();
    }

    public void ApplyStatusEffectDuration(StatusEffect statusEffect)
    {
        OnAddingStatusEffect?.Invoke(statusEffect);

        statusEffectOnCooldown.Add(new CooldownData(statusEffect, statusEffect.StatusEffectDuration));
        //Add UI
    }

    private void RemoveStatusEffect()
    {
        for (int i = statusEffectOnCooldown.Count - 1; i >= 0; i--)
        {
            if (statusEffectOnCooldown[i].buffDuration <= 0)
            {
                OnRemovingStatusEffect?.Invoke(statusEffectOnCooldown[i].statusEffect);

                statusEffectOnCooldown[i].statusEffect.RemoveStatusEffect();
                statusEffectOnCooldown.RemoveAt(i);
            }
        }
    }

    //public bool IsOnCooldown(StatusEffect statusEffect)
    //{
    //    foreach (CooldownData cooldownData in statusEffectOnCooldown)
    //    {
    //        if (cooldownData.statusEffect == statusEffect)
    //        {
    //            Debug.Log(statusEffect.StatusEffectName + "is on cooldown for " + cooldownData.buffDuration.ToString("0.0") + " seconds");
    //            return true;
    //        }
    //    }

    //    return false;
    //}
}
