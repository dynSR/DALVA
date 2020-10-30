using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class StatusEffect : MonoBehaviour
{
    [SerializeField] private string statusEffectName;
    [SerializeField] private string statusEffectDescription;
    [SerializeField] private float statusEffectDuration;
    [SerializeField] private Image statusEffectIcon;
    [SerializeField] private GameObject statusEffectPrefab;

    public string StatusEffectDescription { get => statusEffectDescription; }
    public string StatusEffectName { get => statusEffectName; }
    public GameObject StatusEffectPrefab { get => statusEffectPrefab; }
    public float StatusEffectDuration { get => statusEffectDuration; set => statusEffectDuration = value; }
    public Image StatusEffectIcon { get => statusEffectIcon; }
    public CharacterController CharacterController { get; private set; }
    public CharacterCaracteristics CharacterCharacteristics { get; private set; }
    public StatusEffectCooldownHandler StatusEffectCooldownHandler { get; set; }

    protected virtual void Start()
    {
        CharacterController = GetComponent<CharacterController>();
        StatusEffectCooldownHandler = GetComponent<StatusEffectCooldownHandler>();
        CharacterCharacteristics = GetComponent<CharacterCaracteristics>();
    }

    protected abstract void ApplyStatusEffect();
    public abstract void RemoveStatusEffect();
}
