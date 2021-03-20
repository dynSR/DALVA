using UnityEngine;
using Photon.Pun;
using System.Collections;

public enum AbilityType { Buff, Heal, Debuff, Projectile, CrowdControl, Movement, Shield } //A étoffer si besoin !

[RequireComponent(typeof(AbilitiesCooldownHandler))]
public abstract class Ability : MonoBehaviourPun
{
    [Header("CORE PARAMETERS")]
    [SerializeField] private string abilityName;
    [SerializeField] private string abilityDescription;
    [SerializeField] private AbilityType abilityType;
    [SerializeField] private KeyCode abilityKey;
    [SerializeField] private Sprite abilityIcon;
    [SerializeField] private GameObject abilityPrefab;

    #region Refs
    private CharacterStat Stats => GetComponent<CharacterStat>();
    private CharacterController Controller => GetComponent<CharacterController>();
    public InteractionSystem TargetHandler => GetComponent<InteractionSystem>();
    private AbilitiesCooldownHandler AbilitiesCooldownHandler => GetComponent<AbilitiesCooldownHandler>();
    #endregion

    [Header("NUMERIC PARAMETERS")]
    [SerializeField] private float abilityCooldown;
    [SerializeField] private float abilityDamage;
    [SerializeField] private float abilityRange;
    [SerializeField] private float abilityAreaOfEffect;
    [SerializeField] private float abilityEffectDuration = 0f;
    #region Public refs
    public string AbilityDescription { get => abilityDescription; }
    public string AbilityName { get => abilityName; }
    public AbilityType AbilityType { get => abilityType; }
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

            LockCharacterInPlaceJustBeforeCasting();

            Cast();
            StartCoroutine(PutAbilityOnCooldown(abilityEffectDuration));
        }
    }

    private void LockCharacterInPlaceJustBeforeCasting()
    {
        Controller.Agent.ResetPath();
    }

    private IEnumerator PutAbilityOnCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        AbilitiesCooldownHandler.PutAbilityOnCooldown(this);
    }
}