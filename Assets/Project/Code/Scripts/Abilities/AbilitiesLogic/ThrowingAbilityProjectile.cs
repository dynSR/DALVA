﻿using System.Collections;
using UnityEngine;

public class ThrowingAbilityProjectile : MonoBehaviour
{
    [SerializeField] private Transform aimProjectileEmiterPos;

    #region Réfs
    private CharacterController Controller => GetComponent<CharacterController>();
    public Transform AimProjectileEmiterPos { get => aimProjectileEmiterPos; }
    #endregion

    public IEnumerator ThrowProjectile(GameObject projectile, Transform spawnLocation, Ability ability = null)
    {
        yield return new WaitForSeconds(Controller.RotationSpeed);

        GameObject projectileInstance = Instantiate(projectile, spawnLocation.position, spawnLocation.rotation);

        ProjectileLogic _projectile = projectileInstance.GetComponent<ProjectileLogic>();
        _projectile.ProjectileSender = transform;

        if (ability != null) _projectile.Ability = ability;

        Debug.Log("ThrowingProjectile");
    }
}