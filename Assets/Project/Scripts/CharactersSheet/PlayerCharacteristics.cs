using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacteristics : CharacterCaracteristics
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        
    }

    public override void OnDeath()
    {
        base.OnDeath();
    }

    public override void TakeDamage(float damageTaken)
    {
        base.TakeDamage(damageTaken);
    }
}
