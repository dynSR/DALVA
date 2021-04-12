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

    [Header("DAMAGE ATTRIBUTES")]
    [SerializeField] private float abilityPhysicalDamage = 0f;
    [SerializeField] private float abilityMagicalDamage = 0f;
    [Range(0.0f, 1.0f)][SerializeField] private float abilityPhysicalRatio = 0f;
    [Range(0.0f, 1.0f)][SerializeField] private float abilityMagicalRatio = 0f;
    [SerializeField] private float abilityHealValue = 0f;

    [Header("APPLICATION LIMITS")]
    [SerializeField] private float abilityRange = 0f;//Gestion de la range à ajouter -!-
    [SerializeField] private float abilityAreaOfEffect = 0f;

    [Header("TIMERS")]
    [SerializeField] private float abilityCooldown = 0f;
    [SerializeField] private float abilityTimeToCast = 0f;
    [SerializeField] private float abilityDuration = 0f;

    [Header("PROPERTIES")]
    [SerializeField] private bool instantCasting = false;

    #region Public refs
    public string AbilityName { get => abilityName; }
    public string AbilityDescription { get => abilityDescription; }
    public KeyCode AbilityKey { get => abilityKey; }
    public Sprite AbilityIcon { get => abilityIcon; }
    public GameObject AbilityEffectObject { get => abilityEffectObject; }

    public int AbilityCost { get => abilityCost; set => abilityCost = value; }
    public float AbilityPhysicalDamage { get => abilityPhysicalDamage; set => abilityPhysicalDamage = value; }
    public float AbilityMagicalDamage { get => abilityMagicalDamage; set => abilityMagicalDamage = value; }
    public float AbilityMagicalRatio { get => abilityMagicalRatio; set => abilityMagicalRatio = value; }
    public float AbilityPhysicalRatio { get => abilityPhysicalRatio; set => abilityPhysicalRatio = value; }

    public float AbilityRange { get => abilityRange; set => abilityRange = value; }
    public float AbilityAreaOfEffect { get => abilityAreaOfEffect; set => abilityAreaOfEffect = value; }

    public float AbilityCooldown { get => abilityCooldown; set => abilityCooldown = value; }
    public float AbilityTimeToCast { get => abilityTimeToCast; set => abilityTimeToCast = value; }
    public float AbilityDuration { get => abilityDuration; set => abilityDuration = value; }

    public bool InstantCasting { get => instantCasting; set => instantCasting = value; }
    public float AbilityHealValue { get => abilityHealValue; set => abilityHealValue = value; }
    #endregion
}