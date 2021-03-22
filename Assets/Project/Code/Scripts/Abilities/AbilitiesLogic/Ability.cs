using UnityEngine;
using Photon.Pun;
using System.Collections;

[RequireComponent(typeof(AbilitiesCooldownHandler))]
public abstract class Ability : MonoBehaviourPun
{
    [Header("INFORMATIONS")]
    [SerializeField] private string abilityName;
    [SerializeField] private string abilityDescription;
    [SerializeField] private KeyCode abilityKey;
    [SerializeField] private Sprite abilityIcon;
    [SerializeField] private GameObject abilityPrefab;

    #region Refs
    private CharacterStat Stats => GetComponent<CharacterStat>();
    private CharacterController Controller => GetComponent<CharacterController>();
    public InteractionSystem TargetHandler => GetComponent<InteractionSystem>();
    private AbilitiesCooldownHandler AbilitiesCooldownHandler => GetComponent<AbilitiesCooldownHandler>();
    #endregion

    [Header("ATTRIBUTES VALUE")]
    [SerializeField] private float abilityCooldown = 0f;
    [SerializeField] private float abilityDamage = 0f;
    [SerializeField] private float abilityRange = 0f;//Gestion de la range à ajouter -!-
    [SerializeField] private float abilityAreaOfEffect = 0f;
    [SerializeField] private float abilityCastingTime = 0f;
    [SerializeField] private float abilityEffectDuration = 0f;
    #region Public refs
    public string AbilityDescription { get => abilityDescription; }
    public string AbilityName { get => abilityName; }
    public GameObject AbilityPrefab { get => abilityPrefab; }

    public float AbilityCooldown { get => abilityCooldown; set => abilityCooldown = value; }
    public float AbilityDamage { get => abilityDamage; set => abilityDamage = value; }
    public float AbilityRange { get => abilityRange; }
    public float AbilityAreaOfEffect { get => abilityAreaOfEffect; }
    public Sprite AbilityIcon { get => abilityIcon; }
    public KeyCode AbilityKey { get => abilityKey; }
    #endregion

    protected abstract void Cast();

    protected virtual void Update()
    {
        if (GameObject.Find("GameNetworkManager") != null && !photonView.IsMine && PhotonNetwork.IsConnected || Stats.IsDead) { return; }

        if (Input.GetKeyDown(AbilityKey))
        {
            if (AbilitiesCooldownHandler.IsAbilityOnCooldown(this)) return;

            TargetHandler.Target = null;

            AdjustCharacterPositioning();

            if(!Controller.IsCasting)
                StartCoroutine(ProcessCastingTime(abilityCastingTime));
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

        StartCoroutine(PutAbilityOnCooldown(abilityEffectDuration));

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