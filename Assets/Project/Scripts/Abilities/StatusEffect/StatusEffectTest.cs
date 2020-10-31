using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectTest : StatusEffect
{
    public List<CharacterController> currentCharacterControllers = new List<CharacterController>();

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Applying Status Effect");
            SetTarget();
            ApplyStatusEffect();
        }
    }

    protected override void ApplyStatusEffect()
    {
        Debug.Log("Apply");
        //foreach (CharacterController targets in currentCharacterControllers)
        //{
        //    targets.NavMeshAgent.speed = 10f;
        //    targets.CurrentSpeed = 10f;
        //    StatusEffectCooldownHandler.ApplyStatusEffectDuration(this);
        //}
        for (int i = 0; i < currentCharacterControllers.Count; i++)
        {
            currentCharacterControllers[i].NavMeshAgent.speed = 10f;
            currentCharacterControllers[i].CurrentSpeed = 10f;
            StatusEffectCooldownHandler.ApplyStatusEffectDuration(this);
        }
    }

    public override void RemoveStatusEffect()
    {
        Debug.Log("Remove");
        //foreach (CharacterController targets in currentCharacterControllers)
        //{
        //    targets.NavMeshAgent.speed = targets.InitialSpeed;
        //    targets.CurrentSpeed = targets.InitialSpeed;
        //}
        for (int i = currentCharacterControllers.Count - 1; i >= 0; i--)
        {
            currentCharacterControllers[i].NavMeshAgent.speed = currentCharacterControllers[i].InitialSpeed;
            currentCharacterControllers[i].CurrentSpeed = currentCharacterControllers[i].InitialSpeed;
            currentCharacterControllers.RemoveAt(i);
        }
    }

    public override void SetTarget()
    {
        Debug.Log("Set");
        for (int i = 0; i < Targets.Count; i++)
        {
            currentCharacterControllers.Add(Targets[i].GetComponent<CharacterController>());
        }
    }
}
