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

    [SerializeField] private Ability ability;
    [SerializeField] private GameObject rangeDisplayer;
    [SerializeField] private List<GameObject> abilitVFXToActivate;
    private bool canBeUsed = true;
    private bool characterIsTryingToCast = false;
    protected Vector3 CastLocation = Vector3.zero;

    float DistanceFromCastingPosition => Vector3.Distance(transform.position, CastLocation);
    private bool IsInRangeToCast => DistanceFromCastingPosition <= Ability.AbilityRange;

    [SerializeField] private bool normalCast = true;
    [SerializeField] private bool fastCastWithIndication = false;
    [SerializeField] private bool smartCast = false;

    [Header("Debug")]
    [SerializeField] private Color gizmosColor;

    public Ability Ability { get => ability; }
    public bool CanBeUsed { get => canBeUsed; set => canBeUsed = value; }
    public AbilityEffect UsedEffectIndex { get; set; }
    public List<GameObject> AbilityVFXToActivate { get => abilitVFXToActivate; }

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
            Controller.HandleCharacterRotation(transform, hit.point, Controller.RotateVelocity, Controller.RotationSpeed);
        }
    }
    #endregion

    #region Handling ability casting
    private void CastWhenInRange()
    {
        if (characterIsTryingToCast && !Controller.IsCasting && Ability.AbilityRange == 0) 
        {
            Debug.Log("Ability has a equal to 0");
            StartCoroutine(ProcessCasting(Ability.AbilityCastingTime));
            characterIsTryingToCast = false;
            return; 
        }

        //Compare character position with cast position and if ability range is > move and cast else just cast
        if (characterIsTryingToCast && !Controller.IsCasting && IsInRangeToCast)
        {
            Debug.Log("Close enough, can cast now !");
            StartCoroutine(ProcessCasting(Ability.AbilityCastingTime));
            characterIsTryingToCast = false;
        }
    }

    private IEnumerator ProcessCasting(float castDuration)
    {
        Controller.IsCasting = true;
        Controller.CanMove = false;

        if (castDuration > 0) Controller.Agent.ResetPath();

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

    #region Ability Behaviours 
    protected void AbilityBuff(EntityStats Stat, StatType type, float flatValue, object source, float percentageValue = 0f)
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

    private Vector3 GetCursorPosition(Ray ray)
    {
        Vector3 cursorPosition = Vector3.zero;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            cursorPosition = hit.point;
        }

        return cursorPosition;
    }

    protected void PlayAbilityAnimation(string animationName, bool resetAutoAttack = false)
    {
        if (resetAutoAttack)
            Interactions.ResetInteractionState();

        Controller.CharacterAnimator.SetBool(animationName, true);
    }

    protected void ResetAbilityAnimation(string animationName)
    {
        Controller.CharacterAnimator.SetBool(animationName, false);
    }
    protected void ApplyAbilityAtLocation(Vector3 pos, GameObject applicationInstance)
    {
        Instantiate(applicationInstance, pos, Quaternion.identity);
    }
    #endregion

    #region VFX
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

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;

        if(Ability.AbilityRange > 0f)
            Gizmos.DrawWireSphere(transform.position, Ability.AbilityRange);
    }
}