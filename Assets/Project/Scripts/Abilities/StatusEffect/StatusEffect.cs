using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class StatusEffect : MonoBehaviour
{
    [Header("CORE PARAMETERS")]
    [SerializeField] private string statusEffectName;
    [SerializeField] private string statusEffectDescription;
    [SerializeField] private Sprite statusEffectIcon;
    [SerializeField] private List<Transform> targets = new List<Transform>();
    [SerializeField] private GameObject statusEffectPrefab;

    [Header("NUMERIC PARAMETERS")]
    [SerializeField] private float statusEffectDuration;

    public string StatusEffectDescription { get => statusEffectDescription; }
    public string StatusEffectName { get => statusEffectName; }
    public GameObject StatusEffectPrefab { get => statusEffectPrefab; }
    public float StatusEffectDuration { get => statusEffectDuration; set => statusEffectDuration = value; }
    public Sprite StatusEffectIcon { get => statusEffectIcon; }
    public CharacterController CharacterController { get; private set; }
    public CharacterCaracteristics CharacterCharacteristics { get; private set; }
    public StatusEffectCooldownHandler StatusEffectCooldownHandler { get; set; }
    public List<Transform> Targets { get => targets; set => targets = value; }

    protected virtual void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        StatusEffectCooldownHandler = GetComponent<StatusEffectCooldownHandler>();
        CharacterCharacteristics = GetComponent<CharacterCaracteristics>();
    }

    protected abstract void ApplyStatusEffect();
    public abstract void RemoveStatusEffect();
    public abstract void SetTarget();
}
