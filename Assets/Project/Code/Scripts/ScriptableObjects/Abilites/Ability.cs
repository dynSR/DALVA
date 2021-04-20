using UnityEngine;

[CreateAssetMenu(fileName = "Ability_", menuName = "ScriptableObjects/Abilities", order = 3)]
public class Ability : ScriptableObject
{
    [Header("INFORMATIONS")]
    [SerializeField] private string abilityName;
    [TextArea][SerializeField] private string abilityDescription;
    [SerializeField] private KeyCode abilityKey;
    [SerializeField] private Sprite abilityIcon;
    [SerializeField] private int abilityCost = 0;
    [SerializeField] private GameObject abilityEffectObject;
    [SerializeField] private bool isPointAndClick = false;

    [Header("DAMAGE ATTRIBUTES")]
    [SerializeField] private float abilityPhysicalDamage = 0f;
    [SerializeField] private float abilityMagicalDamage = 0f;
    [Range(0.0f, 1.0f)][SerializeField] private float abilityPhysicalRatio = 0f;
    [Range(0.0f, 1.0f)][SerializeField] private float abilityMagicalRatio = 0f;
    [Range(0.0f, 1.0f)] [SerializeField] private float targetHealthThreshold = 0f;
    [Range(0.0f, 1.0f)] [SerializeField] private float abilityAddedDamageOnTargetHealthThreshold = 0f;

    [Header("HEAL")]
    [SerializeField] private float abilityHealValue = 0f;
    [SerializeField] private bool abilityCanHealTarget= false;
    [SerializeField] private float healMagicalRatio = 0f;
    [SerializeField] private float healHealthRatio = 0f;

    [Header("SHIELD")]
    [SerializeField] private float abilityShieldValue = 0f;
    [SerializeField] private bool abilityCanApplyShieldToTarget = false;
    [SerializeField] private float shieldMagicalRatio = 0f;
    [SerializeField] private float shieldHealthRatio = 0f;

    [Header("STATUS EFFECT")]
    [SerializeField] private StatusEffect abilityStatusEffect;

    [Header("MARK ATTRIBUTES")]
    [SerializeField] private float abilityMarkDuration = 0f;
    [SerializeField] private bool abilityCanMark = false;
    [SerializeField] private bool abilityCanConsumeMark = false;
    [SerializeField] private StatusEffect effectAppliedOnMarkedTarget;


    [Header("APPLICATION LIMITS")]
    [SerializeField] private float abilityRange = 0f;
    [SerializeField] private float abilityAreaOfEffect = 0f;

    [Header("TIMERS")]
    [SerializeField] private float abilityCooldown = 0f;
    [SerializeField] private float abilityTimeToCast = 0f;
    [SerializeField] private float abilityDuration = 0f;

    [Header("PROPERTIES")]
    [SerializeField] private bool instantCasting = false;
    [SerializeField] private float delayBeforeApplyingDamageOrEffect = 0f;

    #region Public refs
    public string AbilityName { get => abilityName; }
    public string AbilityDescription { get => abilityDescription; }
    public KeyCode AbilityKey { get => abilityKey; }
    public Sprite AbilityIcon { get => abilityIcon; }
    public GameObject AbilityEffectObject { get => abilityEffectObject; }
    public bool IsPointAndClick { get => isPointAndClick; }

    public int AbilityCost { get => abilityCost; set => abilityCost = value; }
    public float AbilityPhysicalDamage { get => abilityPhysicalDamage; set => abilityPhysicalDamage = value; }
    public float AbilityMagicalDamage { get => abilityMagicalDamage; set => abilityMagicalDamage = value; }
    public float AbilityMagicalRatio { get => abilityMagicalRatio; set => abilityMagicalRatio = value; }
    public float AbilityPhysicalRatio { get => abilityPhysicalRatio; set => abilityPhysicalRatio = value; }

    public float TargetHealthThreshold { get => targetHealthThreshold; set => targetHealthThreshold = value; }
    public float AbilityAddedDamageOnTargetHealthThreshold { get => abilityAddedDamageOnTargetHealthThreshold; set => abilityAddedDamageOnTargetHealthThreshold = value; }
    
    public float AbilityHealValue { get => abilityHealValue; set => abilityHealValue = value; }
    public bool AbilityCanHealTarget { get => abilityCanHealTarget; set => abilityCanHealTarget = value; }
    public float HealMagicalRatio { get => healMagicalRatio; set => healMagicalRatio = value; }
    public float HealHealthRatio { get => healHealthRatio; set => healHealthRatio = value; }

    public float AbilityShieldValue { get => abilityShieldValue; set => abilityShieldValue = value; }
    public bool AbilityCanApplyShieldToTarget { get => abilityCanApplyShieldToTarget; set => abilityCanApplyShieldToTarget = value; }
    public float ShieldHealthRatio { get => shieldHealthRatio; set => shieldHealthRatio = value; }
    public float ShieldMagicalRatio { get => shieldMagicalRatio; set => shieldMagicalRatio = value; }

    public StatusEffect AbilityStatusEffect { get => abilityStatusEffect; set => abilityStatusEffect = value; }

    public float AbilityMarkDuration { get => abilityMarkDuration; set => abilityMarkDuration = value; }
    public bool AbilityCanMark { get => abilityCanMark; set => abilityCanMark = value; }
    public bool AbilityCanConsumeMark { get => abilityCanConsumeMark; set => abilityCanConsumeMark = value; }
    public StatusEffect EffectAppliedOnMarkedTarget { get => effectAppliedOnMarkedTarget; set => effectAppliedOnMarkedTarget = value; }

    public float AbilityRange { get => abilityRange; set => abilityRange = value; }
    public float AbilityAreaOfEffect { get => abilityAreaOfEffect; set => abilityAreaOfEffect = value; }

    public float AbilityCooldown { get => abilityCooldown; set => abilityCooldown = value; }
    public float AbilityTimeToCast { get => abilityTimeToCast; set => abilityTimeToCast = value; }
    public float AbilityDuration { get => abilityDuration; set => abilityDuration = value; }

    public bool InstantCasting { get => instantCasting; set => instantCasting = value; }
    public float DelayBeforeApplyingDamageOrEffect { get => delayBeforeApplyingDamageOrEffect; set => delayBeforeApplyingDamageOrEffect = value; }
    #endregion
}