using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TypeOfEffect { EffectToMovementSpeed, EffectToDamage, EffectToSomethingElse }

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterCaracteristics))]
[RequireComponent(typeof(CooldownHandler))]
public abstract class StatusEffect : MonoBehaviour
{
    [Header("CORE PARAMETERS")]
    [SerializeField] private string statusEffectName;
    [SerializeField] private string statusEffectDescription;
    [SerializeField] private Sprite statusEffectIcon;
    [SerializeField] private List<Transform> targets = new List<Transform>();
    [SerializeField] private GameObject statusEffectPrefab;
    [SerializeField] private TypeOfEffect typeOfEffect;

    [Header("NUMERIC PARAMETERS")]
    [SerializeField] private float statusEffectDuration;

    public string StatusEffectDescription { get => statusEffectDescription; }
    public string StatusEffectName { get => statusEffectName; }
    public GameObject StatusEffectPrefab { get => statusEffectPrefab; }
    public float StatusEffectDuration { get => statusEffectDuration; set => statusEffectDuration = value; }
    public Sprite StatusEffectIcon { get => statusEffectIcon; }
    public List<Transform> Targets { get => targets; set => targets = value; }
    public CooldownHandler StatusEffectDurationHandler => GetComponent<CooldownHandler>();
    public TypeOfEffect TypeOfEffect { get => typeOfEffect; set => typeOfEffect = value; }
    public StatusEffectContainer StatusEffectContainer { get; set; }

    protected abstract void ApplyStatusEffect();
    public abstract void RemoveStatusEffect();
    public abstract void SetTarget();

    public virtual void CheckForExistingStatusEffect()
    {
        StatusEffectDurationHandler.CheckForSimilarExistingStatusEffect(this);
    }
}
