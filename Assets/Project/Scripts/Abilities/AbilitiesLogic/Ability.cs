using UnityEngine;
using Photon.Pun;
using System.Collections;

public enum AbilityType { Buff, Heal, Debuff, Projectile, CrowdControl, Movement, Shield } //A étoffer si besoin !

[RequireComponent(typeof(CharacterController))]
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
    private Stats CharacterStats => GetComponent<Stats>();
    private CharacterController CharacterController => GetComponent<CharacterController>();
    public CombatBehaviour CombatBehaviour => GetComponent<CombatBehaviour>();
    private AbilitiesCooldownHandler CooldownHandler => GetComponent<AbilitiesCooldownHandler>();

    [Header("NUMERIC PARAMETERS")]
    [SerializeField] private float abilityCooldown;
    [SerializeField] private float abilityDamage;
    [SerializeField] private float abilityRange;
    [SerializeField] private float abilityAreaOfEffect;
    [SerializeField] private float abilityEffectDuration = 0f;

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

    protected abstract void Cast();

    protected virtual void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true){ return; }

        if (CharacterStats.IsDead) return;

        if (Input.GetKeyDown(AbilityKey))
        {
            if (CooldownHandler.IsAbilityOnCooldown(this)) return;

            CombatBehaviour.TargetedEnemy = null;

            LockCharacterInPlaceJustBeforeCasting();

            Cast();
            StartCoroutine(PutAbilityOnCooldown(abilityEffectDuration));
        }
    }

    private void LockCharacterInPlaceJustBeforeCasting()
    {
        CharacterController.Agent.ResetPath();
    }

    private IEnumerator PutAbilityOnCooldown(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        CooldownHandler.PutAbilityOnCooldown(this);
    }
}