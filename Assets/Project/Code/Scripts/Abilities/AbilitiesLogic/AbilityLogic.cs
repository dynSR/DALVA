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
[RequireComponent(typeof(ThrowingAbilityProjectile))]
[RequireComponent(typeof(DashLogic))]
public abstract class AbilityLogic : MonoBehaviourPun
{
    #region Refs
    public Transform Player => GetComponent<Transform>();
    protected EntityStats Stats => GetComponent<EntityStats>();
    protected CharacterController Controller => GetComponent<CharacterController>();
    protected InteractionSystem Interactions => GetComponent<InteractionSystem>();
    protected AbilitiesCooldownHandler AbilitiesCooldownHandler => GetComponent<AbilitiesCooldownHandler>();
    protected ThrowingAbilityProjectile ThrowingProjectile => GetComponent<ThrowingAbilityProjectile>();
    protected DashLogic DashLogic => GetComponent<DashLogic>();
    #endregion

    [Header("USED ABILITY")]
    [SerializeField] private Ability ability;
    [SerializeField] private AbilityEffect usedEffectIndex = AbilityEffect.I;

    [Header("PREVISUALISATION")]
    [SerializeField] private GameObject rangeDisplayer;

    [Header("VFX")]
    [SerializeField] private List<GameObject> abilityVFXToActivate;
    private bool canBeUsed = true;
    private bool characterIsTryingToCast = false;
    public Vector3 CastLocation = Vector3.zero;

    float DistanceFromCastingPosition => Vector3.Distance(transform.position, CastLocation);
    private bool IsInRangeToCast => DistanceFromCastingPosition <= Ability.AbilityRange;

    [Header("CASTING TYPE")]
    [SerializeField] private bool normalCast = true;
    [SerializeField] private bool fastCastWithIndication = false;
    [SerializeField] private bool smartCast = false;

    [Header("DEBUG")]
    [SerializeField] private Color gizmosColor;

    public Ability Ability { get => ability; }
    public bool CanBeUsed { get => canBeUsed; set => canBeUsed = value; }
    public AbilityEffect UsedEffectIndex { get => usedEffectIndex; set => usedEffectIndex = value; }
    public List<GameObject> AbilityVFXToActivate { get => abilityVFXToActivate; }

    protected abstract void Cast();

    protected virtual void Update()
    {
        if (GameObject.Find("GameNetworkManager") != null && !photonView.IsMine && PhotonNetwork.IsConnected || Stats.IsDead) { return; }

        if (AbilitiesCooldownHandler.IsAbilityOnCooldown(this) || Controller.IsCasting || !CanBeUsed) return;

        if (UtilityClass.IsKeyPressed(Ability.AbilityKey))
        {
            if (normalCast || fastCastWithIndication)
            {
                if (rangeDisplayer != null && !rangeDisplayer.activeInHierarchy)
                {
                    rangeDisplayer.SetActive(true);
                }
            }
            else if (smartCast)
            {
                characterIsTryingToCast = true;

                AdjustCharacterPositioning();
                CastLocation = GetCursorPosition(UtilityClass.RayFromMainCameraToMousePosition());

                if (Ability.AbilityRange > 0 && !IsInRangeToCast)
                {
                    Debug.Log("Can't cast now, need to get closer !");
                    Controller.Agent.SetDestination(CastLocation);
                }
            }
        }

        if (normalCast && rangeDisplayer != null && rangeDisplayer.activeInHierarchy && UtilityClass.LeftClickIsPressed()
            || fastCastWithIndication && rangeDisplayer != null && rangeDisplayer.activeInHierarchy && UtilityClass.IsKeyUnpressed(Ability.AbilityKey))
        {
            characterIsTryingToCast = true;

            rangeDisplayer.SetActive(false);

            AdjustCharacterPositioning();
            CastLocation = GetCursorPosition(UtilityClass.RayFromMainCameraToMousePosition());

            if (Ability.AbilityRange > 0 && !IsInRangeToCast)
            {
                Debug.Log("Can't cast now, need to get closer !");
                Controller.Agent.SetDestination(CastLocation);
            }
        }

        CastWhenInRange();
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
            Controller.HandleCharacterRotationBeforeCasting(transform, hit.point, Controller.RotateVelocity, Controller.RotationSpeed);
        }
    }
    #endregion

    #region Get cursor Informations
    public static Vector3 GetCursorPosition(Ray ray)
    {
        Vector3 cursorPosition = Vector3.zero;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            cursorPosition = hit.point;
        }

        return cursorPosition;
    }
    #endregion

    #region Handling ability casting
    private void CastWhenInRange()
    {
        if (characterIsTryingToCast && !Controller.IsCasting && (IsInRangeToCast || Ability.AbilityRange == 0f))
        {
            //Debug.Log("Close enough, can cast now !");
            Controller.IsCasting = true;
            Controller.CanMove = false;
            Cast();
            StartCoroutine(PutAbilityOnCooldown(Ability.AbilityTimeToCast + Ability.AbilityDuration));
            characterIsTryingToCast = false;
        }
    }

    public IEnumerator PutAbilityOnCooldown(float delay)
    {
        //if delay == 0 it means that it is directly put in cooldoown it is mainly used with ability that do not use duration like auras, boosts, etc.
        //else it is gonna wait before puttin ability in cooldown
        yield return new WaitForSeconds(delay);
        
        AbilitiesCooldownHandler.PutAbilityOnCooldown(this);
    }
    #endregion

    protected void AbilityGivesABuff(EntityStats Stat, StatType type, float flatValue, object source, float percentageValue = 0f)
    {
        if (!CanBeUsed) return;

        if (flatValue != 0)
            Stat.GetStat(type).AddModifier(new StatModifier(flatValue, type, StatModType.Flat, source));

        if(percentageValue != 0)
            Stat.GetStat(type).AddModifier(new StatModifier(percentageValue, type, StatModType.PercentAdd, source));

        CanBeUsed = false;
    }

    protected void RemoveBuffGivenByAnAbility(StatType affectedStat, object source = null)
    {
        if (!CanBeUsed)
        {
            if (AbilitiesCooldownHandler.IsAbilityOnCooldown(this)) return;

            AbilitiesCooldownHandler.PutAbilityOnCooldown(this);
            Stats.GetStat(affectedStat).RemoveAllModifiersFromSource(source);
        }
    }

    public void ApplyAbilityEffectAtLocation(Vector3 pos, GameObject applicationInstance)
    {
        Instantiate(applicationInstance, pos, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(new Vector3(pos.x, 0.01f, pos.z), Ability.AbilityAreaOfEffect);

        foreach (Collider collider in colliders)
        {
            EntityStats targetStat = collider.GetComponent<EntityStats>();
            EntityDetection targetFound = collider.GetComponent<EntityDetection>();

            EntityStats characterStat = transform.GetComponent<EntityStats>();

            if (targetStat != null && !targetStat.IsDead
                && targetStat.EntityTeam != Stats.EntityTeam
                && collider.transform != transform
                && (targetFound.ThisTargetIsAPlayer(targetFound)
                || targetFound.ThisTargetIsAMonster(targetFound)
                || targetFound.ThisTargetIsAMinion(targetFound)))
            {
                collider.GetComponent<EntityStats>().TakeDamage
                    (transform,
                    targetStat.GetStat(StatType.PhysicalResistances).Value,
                    targetStat.GetStat(StatType.MagicalResistances).Value,
                    Ability.AbilityPhysicalDamage + (characterStat.GetStat(StatType.PhysicalPower).Value * Ability.AbilityPhysicalRatio),
                    Ability.AbilityMagicalDamage + (characterStat.GetStat(StatType.MagicalPower).Value * Ability.AbilityMagicalRatio),
                    characterStat.GetStat(StatType.CriticalStrikeChance).Value,
                    175f,
                    characterStat.GetStat(StatType.PhysicalPenetration).Value,
                    characterStat.GetStat(StatType.MagicalPenetration).Value);
            }
        }
    }

    #region Ability animation Play / Reset
    protected void PlayAbilityAnimation(string animationName, bool resetAutoAttack = false, bool lostTargetOnCast = false)
    {
        if (resetAutoAttack)
            Interactions.ResetInteractionState();

        if (lostTargetOnCast)
            Interactions.Target = null;

        Controller.CharacterAnimator.SetLayerWeight(2, 1);
        Controller.CharacterAnimator.SetBool(animationName, true);
    }
    #endregion

    #region VFX activation / desactivation
    protected void ActivateVFX(List<GameObject> effects)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects.Count > 0)
                effects[i].SetActive(true);
        }
    }

    protected void DeactivateVFX(List<GameObject> effects)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects.Count > 0)
                effects[i].SetActive(false);
        }
    }
    #endregion

    #region Debug Range
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;

        if(Ability.AbilityRange > 0f)
            Gizmos.DrawWireSphere(transform.position, Ability.AbilityRange);
    }
    #endregion
}