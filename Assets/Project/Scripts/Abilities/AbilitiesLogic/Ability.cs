using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public enum AbilityType { Buff, Heal, Debuff, Projectile, CrowdControl, Movement, Shield } //A étoffer si besoin !

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CooldownHandler))]
public abstract class Ability : MonoBehaviourPun
{
    [Header("CORE PARAMETERS")]
    [SerializeField] private string abilityName;
    [SerializeField] private string abilityDescription;
    [SerializeField] private AbilityType abilityType;
    [SerializeField] private KeyCode abilityKey;
    [SerializeField] private Sprite abilityIcon;
    [SerializeField] private GameObject abilityPrefab;
    private CharacterController CharacterController => GetComponent<CharacterController>();
    private CooldownHandler CooldownHandler => GetComponent<CooldownHandler>();

    [Header("NUMERIC PARAMETERS")]
    [SerializeField] private bool isInstantCast = false;
    [SerializeField] private float abilityCooldown;
    [SerializeField] private float abilityDamage;
    [SerializeField] private float abilityRange;
    [SerializeField] private float abilityAreaOfEffect;

    public string AbilityDescription { get => abilityDescription; }
    public string AbilityName { get => abilityName; }
    public AbilityType AbilityType { get => abilityType; }
    public GameObject AbilityPrefab { get => abilityPrefab; }

    public float AbilityCooldown { get => abilityCooldown; set => abilityCooldown = value; }
    public float AbilityDamage { get => abilityDamage; set => abilityDamage = value; }
    public float AbilityRange { get => abilityRange; }
    public float AbilityAreaOfEffect { get => abilityAreaOfEffect; }
    public bool IsInstantCast { get => isInstantCast; }
    public Sprite AbilityIcon { get => abilityIcon; }
    public KeyCode AbilityKey { get => abilityKey; }

    protected abstract void Cast();

    protected virtual void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (Input.GetKeyDown(AbilityKey))
        {
            if (CooldownHandler.IsAbilityOnCooldown(this)) return;

            HandleCharacterBehaviourBeforeCasting();
            Cast();
            CooldownHandler.PutAbilityOnCooldown(this);
        }
    }

    private void HandleCharacterBehaviourBeforeCasting()
    {
        if (!IsInstantCast)
        {
            CharacterController.NavMeshAgent.ResetPath();
        }
    }
}
