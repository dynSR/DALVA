using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void OnDeath()
    {
        base.OnDeath();
    }

    public override void TakeDamage(float physicalDamageTaken, float magicDamageTaken)
    {
        base.TakeDamage(physicalDamageTaken, magicDamageTaken);
    }
}
