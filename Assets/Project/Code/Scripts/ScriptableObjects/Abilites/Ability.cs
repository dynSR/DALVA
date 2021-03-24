using UnityEngine;

[CreateAssetMenu(fileName = "Ability_", menuName = "ScriptableObjects/Abilities", order = 3)]
public class Ability : ScriptableObject
{
    [Header("INFORMATIONS")]
    [SerializeField] private string abilityName;
    [SerializeField] private string abilityDescription;
    [SerializeField] private KeyCode abilityKey;
    [SerializeField] private Sprite abilityIcon;
    [SerializeField] private GameObject abilityProjectilePrefab;

    [Header("ATTRIBUTES VALUE")]
    [SerializeField] private float abilityCooldown = 0f;
    [SerializeField] private float abilityPhysicalDamage = 0f;
    [SerializeField] private float abilityMagicalDamage = 0f;
    [SerializeField] private float abilityRange = 0f;//Gestion de la range à ajouter -!-
    [SerializeField] private float abilityAreaOfEffect = 0f;
    [SerializeField] private float abilityCastingTime = 0f;
    [SerializeField] private float abilityEffectDuration = 0f;

    #region Public refs
    public string AbilityName { get => abilityName; }
    public string AbilityDescription { get => abilityDescription; }
    public KeyCode AbilityKey { get => abilityKey; }
    public Sprite AbilityIcon { get => abilityIcon; }
    
    public GameObject AbilityProjectilePrefab { get => abilityProjectilePrefab; }

    public float AbilityCooldown { get => abilityCooldown; set => abilityCooldown = value; }
    public float AbilityPhysicalDamage { get => abilityPhysicalDamage; set => abilityPhysicalDamage = value; }
    public float AbilityMagicalDamage { get => abilityMagicalDamage; set => abilityMagicalDamage = value; }
    public float AbilityRange { get => abilityRange; set => abilityRange = value; }
    public float AbilityAreaOfEffect { get => abilityAreaOfEffect; set => abilityAreaOfEffect = value; }

    public float AbilityCastingTime { get => abilityCastingTime; set => abilityCastingTime = value; }
    public float AbilityEffectDuration { get => abilityEffectDuration; set => abilityEffectDuration = value; }
    #endregion
}