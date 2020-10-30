using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AbilityType { Buff, Heal, Debuff, Projectile, CrowdControl, Movement, Shield } //A étoffer si besoin !

public abstract class Ability : MonoBehaviour
{
    [Header("CORE PARAMETERS")]
    [SerializeField] private string abilityName;
    [SerializeField] private string abilityDescription;
    [SerializeField] private AbilityType abilityType;
    [SerializeField] private KeyCode abilityKey;
    [SerializeField] private GameObject abilityPrefab;
    private CharacterController characterController;
    private AbilityCooldownHandler cooldownHandler;

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

    protected abstract void Cast();

    protected virtual void Start()
    {
        characterController = GetComponent<CharacterController>();
        cooldownHandler = GetComponent<AbilityCooldownHandler>();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(abilityKey))
        {
            if (cooldownHandler.IsOnCooldown(this)) return;

            HandleCharacterBehaviourBeforeCasting();
            Cast();
            cooldownHandler.PutOnCooldown(this);
        }
    }

    private void HandleCharacterBehaviourBeforeCasting()
    {
        if (!IsInstantCast)
        {
            characterController.NavMeshAgent.ResetPath();
        }
    }
}
