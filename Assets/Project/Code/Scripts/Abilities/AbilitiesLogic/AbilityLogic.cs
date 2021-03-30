using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityEffect
{
    I,
    II,
    III,
    IV
}

[RequireComponent(typeof(AbilitiesCooldownHandler))]
public abstract class AbilityLogic : MonoBehaviourPun
{
    #region Refs
    public Transform Player => GetComponent<Transform>();
    protected CharacterStat Stats => GetComponent<CharacterStat>();
    protected CharacterController Controller => GetComponent<CharacterController>();
    protected InteractionSystem Interactions => GetComponent<InteractionSystem>();
    protected AbilitiesCooldownHandler AbilitiesCooldownHandler => GetComponent<AbilitiesCooldownHandler>();
    #endregion

    [SerializeField] private Ability ability;
    public Ability Ability { get => ability; }
    private bool canBeUsed = true;
    public bool CanBeUsed { get => canBeUsed; set => canBeUsed = value; }
    public AbilityEffect UsedEffectIndex { get; set; }

    protected abstract void Cast();

    protected virtual void Update()
    {
        if (GameObject.Find("GameNetworkManager") != null && !photonView.IsMine && PhotonNetwork.IsConnected || Stats.IsDead) { return; }

        if (Input.GetKeyDown(Ability.AbilityKey))
        {
            if (AbilitiesCooldownHandler.IsAbilityOnCooldown(this) || Controller.IsCasting || !CanBeUsed) return;

            //this reset needs to be used when the ability used cancel the attacking state
            //Interactions.Target = null;

            AdjustCharacterPositioning();
            StartCoroutine(ProcessCastingTime(Ability.AbilityCastingTime));
        }
    }

    #region Handle character position and rotation
    private void AdjustCharacterPositioning()
    {
        if (Ability.InstantCasting) return;

        TurnCharacterTowardsLaunchDirection();
        Controller.Agent.ResetPath();
    }

    private void TurnCharacterTowardsLaunchDirection()
    {
        if (Physics.Raycast(UtilityClass.RayFromMainCameraToMousePosition(), out RaycastHit hit, Mathf.Infinity))
        {
            Controller.HandleCharacterRotation(transform, hit.point, Controller.RotateVelocity, Controller.RotationSpeed);
        }
    }
    #endregion

    #region Handling ability casting
    private IEnumerator ProcessCastingTime(float castDuration)
    {
        //if castDuration == 0 it means that it is considered as an instant cast 
        //else it is gonna wait before casting the spell
        Controller.IsCasting = true;
        Controller.CanMove = false;

        yield return new WaitForSeconds(castDuration);
        Cast();

        if(Ability.AutomaticallyPutInCooldown)
            StartCoroutine(PutAbilityOnCooldown(Ability.AbilityDuration));

        yield return new WaitForSeconds(0.25f);

        Controller.IsCasting = false;
        Controller.CanMove = true;
    }

    private IEnumerator PutAbilityOnCooldown(float delay)
    {
        //if delay == 0 it means that it is directly put in cooldoown it is mainly used with ability that do not use duration like auras, boosts, etc.
        //else it is gonna wait before puttin ability in cooldown
        yield return new WaitForSeconds(delay);
        
        AbilitiesCooldownHandler.PutAbilityOnCooldown(this);
    }
    #endregion

    #region Buff 
    protected void AbilityBuff(CharacterStat Stat, StatType type, float flatValue, object source, float percentageValue = 0f)
    {
        if (!CanBeUsed) return;

        if (flatValue != 0)
            Stat.GetStat(type).AddModifier(new StatModifier(flatValue, type, StatModType.Flat, source));

        if(percentageValue != 0)
            Stat.GetStat(type).AddModifier(new StatModifier(percentageValue, type, StatModType.PercentAdd, source));

        CanBeUsed = false;
    }

    protected void RemoveAbilityBuff(StatType affectedStat, object source = null)
    {
        if (!CanBeUsed)
        {
            if (AbilitiesCooldownHandler.IsAbilityOnCooldown(this)) return;

            AbilitiesCooldownHandler.PutAbilityOnCooldown(this);
            Stats.GetStat(affectedStat).RemoveAllModifiersFromSource(source);
        }
    }

    protected void ActivateVFX(List<GameObject> effects)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].SetActive(true);
        }
    }

    protected void DeactivateVFX(List<GameObject> effects)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].SetActive(false);
        }
    }
    #endregion
}