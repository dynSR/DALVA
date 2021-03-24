using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AbilitiesCooldownHandler))]
public abstract class AbilityLogic : MonoBehaviourPun
{
    #region Refs
    private CharacterStat Stats => GetComponent<CharacterStat>();
    private CharacterController Controller => GetComponent<CharacterController>();
    public InteractionSystem TargetHandler => GetComponent<InteractionSystem>();
    private AbilitiesCooldownHandler AbilitiesCooldownHandler => GetComponent<AbilitiesCooldownHandler>();
    #endregion

    [SerializeField] private Ability ability;
    public Ability Ability { get => ability; }

    protected abstract void Cast();

    protected virtual void Update()
    {
        if (GameObject.Find("GameNetworkManager") != null && !photonView.IsMine && PhotonNetwork.IsConnected || Stats.IsDead) { return; }

        if (Input.GetKeyDown(Ability.AbilityKey))
        {
            if (AbilitiesCooldownHandler.IsAbilityOnCooldown(Ability)) return;

            TargetHandler.Target = null;

            AdjustCharacterPositioning();

            if (!Controller.IsCasting)
                StartCoroutine(ProcessCastingTime(Ability.AbilityCastingTime));
        }
    }

    private void AdjustCharacterPositioning()
    {
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

    private IEnumerator ProcessCastingTime(float castDuration)
    {
        //if castDuration == 0 it means that it is considered as an instant cast 
        //else it is gonna wait before casting the spell
        Controller.IsCasting = true;
        Controller.CanMove = false;

        yield return new WaitForSeconds(castDuration);
        Cast();

        StartCoroutine(PutAbilityOnCooldown(Ability.AbilityEffectDuration));

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
}
