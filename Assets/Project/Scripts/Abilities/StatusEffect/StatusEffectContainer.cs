using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectContainer : MonoBehaviour
{
    [SerializeField] private StatusEffect statuesEffect;
    public StatusEffect StatuesEffect { get => statuesEffect; set => statuesEffect = value; }
}
