using System.Collections;
using UnityEngine;

public class ThrowingProjectile : MonoBehaviour
{
    [SerializeField] private Transform aimProjectileEmiterPos;

    #region Réfs
    private CharacterController Controller => GetComponent<CharacterController>();
    public Transform AimProjectileEmiterPos { get => aimProjectileEmiterPos; }
    #endregion

    public IEnumerator LaunchAProjectile(GameObject projectile, Transform spawnLocation)
    {
        yield return new WaitForSeconds(Controller.RotationSpeed);

        GameObject projectileInstance = Instantiate(projectile, spawnLocation.position, spawnLocation.rotation);

        ProjectileLogic _projectile = projectileInstance.GetComponent<ProjectileLogic>();
        _projectile.ProjectileSender = transform;

        Debug.Log("ThrowingProjectile");
    }
}