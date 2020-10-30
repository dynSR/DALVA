using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectTest : StatusEffect
{
    public float initialSpeed;

    protected override void Start()
    {
        base.Start();
        initialSpeed = CharacterController.NavMeshAgent.speed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ApplyStatusEffect();
        }
    }

    protected override void ApplyStatusEffect()
    {
        CharacterController.NavMeshAgent.speed = 10;
        StatusEffectCooldownHandler.ApplyStatusEffect(this);
    }

    public override void RemoveStatusEffect()
    {
        CharacterController.NavMeshAgent.speed = initialSpeed;
    }
}
