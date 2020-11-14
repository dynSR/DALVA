using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dalva_Project
{
    [RequireComponent(typeof(LaunchProjectile))]
    public class Ability_ProjectileTest01 : Ability
    {
        private LaunchProjectile LaunchProjectile => GetComponent<LaunchProjectile>();

        protected override void Update()
        {
            base.Update();
        }

        protected override void Cast()
        {
            StartCoroutine(LaunchProjectile.LaunchAProjectile(AbilityPrefab, LaunchProjectile.EmmiterPosition, ProjectileType.TravelsForward, CombatBehaviour.TargetedEnemy, transform));
        }
    }
}

