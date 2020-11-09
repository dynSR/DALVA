using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
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
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Take Damage");
            TakeDamage(200, 150);
            Debug.Log(CurrentHealth);
        }
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
