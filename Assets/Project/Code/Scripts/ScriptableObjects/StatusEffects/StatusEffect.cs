using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect_", menuName = "ScriptableObjects/StatusEffects", order = 1)]
public class StatusEffect : ScriptableObject
{
    [Header("STATUS EFFECT INFORMATIONS & COMPONENTS")]
    [SerializeField] private string statusEffectName;
    [SerializeField] private string statusEffectDescription;
    [SerializeField] private float statusEffectDuration;
    [SerializeField] private int statusEffectId;
    [SerializeField] private Sprite statusEffectIcon;
    [SerializeField] private GameObject statusEffectVFXPrefab;
    [SerializeField] private AudioClip statusEffectSound;

    public string StatusEffectName { get => statusEffectName; }
    public string StatusEffectDescription { get => statusEffectDescription; }
    public float StatusEffectDuration { get => statusEffectDuration; set => statusEffectDuration = value; }
    public int StatusEffectId { get => statusEffectId; }
    public Sprite StatusEffectIcon { get => statusEffectIcon; }
    public GameObject StatusEffectVFXPrefab { get => statusEffectVFXPrefab; }
    public AudioClip StatusEffectSound { get => statusEffectSound; }
}