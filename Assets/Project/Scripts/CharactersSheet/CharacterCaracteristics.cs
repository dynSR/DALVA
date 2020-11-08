using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClass { Warrior, Marksman, Mage, Healer }
public class CharacterCaracteristics : MonoBehaviour, IDamageable<float>, IKillable
{
    [Header("CORE PARAMETERS")]
    [SerializeField] private string characterName;
    [SerializeField] private CharacterClass characterClass;
    [SerializeField] private List<Ability> characterAbilities;

    [Header("NUMERIC PARAMETERS")]
    private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private float healthRegeneration;
    private float currentCooldownReduction;
    [SerializeField] private float maxCooldownReduction;
    [SerializeField] private float timeToRespawn;

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float CurrentHealth { get => currentHealth; set => currentHealth = Mathf.Clamp(value, 0, MaxHealth); }
    public float HealthRegeneration { get => healthRegeneration; set => healthRegeneration = value; }

    public float MaxCooldownReduction { get => maxCooldownReduction; set => maxCooldownReduction = value; }
    public float CurrentCooldownReduction { get => currentCooldownReduction; set => currentCooldownReduction = Mathf.Clamp(value, 0, MaxCooldownReduction); }

    public bool IsDead => CurrentHealth <= 0;
    public bool IsCooldownReductionCapped => CurrentCooldownReduction >= MaxCooldownReduction;

    public List<Ability> CharacterAbilities { get => characterAbilities; }
    public float TimeToRespawn { get => timeToRespawn; set => timeToRespawn = value; }

    protected virtual void Awake()
    {
        GetAllCharacterAbilities();
    }

    protected virtual void Start()
    {
        SetCurrentHealthAtStartOfTheGame();
        SetCurrentCooldownReductionAtStartOfTheGame(MaxCooldownReduction);
    }

    private void SetCurrentHealthAtStartOfTheGame()
    {
        CurrentHealth = MaxHealth;
    }

    private void SetCurrentCooldownReductionAtStartOfTheGame(float cooldownValue)
    {
        CurrentCooldownReduction = cooldownValue;
    }

    private void GetAllCharacterAbilities()
    {
        foreach (Ability abilityFound in GetComponents<Ability>())
        {
            CharacterAbilities.Add(abilityFound);
        }
    }

    public virtual void OnDeath()
    {
        //Bloquer les inputs pour les compétences

        //Afficher le HUD de mort pendant le temps de la mort

        //Start une coroutine de respawn -> après un temps t le personnage réapparaît à son point de spawn
        //StartCoroutine(Respawn(TimeToRespawn));
    }

    public virtual void TakeDamage(float damageTaken)
    {
        CurrentHealth -= damageTaken;
    }

    protected IEnumerator Respawn(float timeBeforeRespawn)
    {
        yield return new WaitForSeconds(timeBeforeRespawn);
        //Respawn
    }
}
