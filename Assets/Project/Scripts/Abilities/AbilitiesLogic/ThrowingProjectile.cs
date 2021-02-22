using System.Collections;
using UnityEngine;

public class ThrowingProjectile : MonoBehaviour
{
    [SerializeField] private Transform aimProjectileEmiterPos;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float rotationSpeed;

    private CharacterController CharacterController => GetComponent<CharacterController>();
    public Transform AimProjectileEmiterPos { get => aimProjectileEmiterPos; }

    public IEnumerator LaunchAProjectile(GameObject projectile, Transform spawnLocation)
    {
        TurnCharacterTowardsLaunchDirection();

        yield return new WaitForSeconds(rotationSpeed);

        GameObject projectileInstance = Instantiate(projectile, spawnLocation.position, spawnLocation.rotation);

        Projectile _projectile = projectileInstance.GetComponent<Projectile>();
        _projectile.ProjectileSender = transform;

        Debug.Log("ThrowingProjectile");
    }

    public void TurnCharacterTowardsLaunchDirection()
    {
        Ray ray = UtilityClass.RayFromMainCameraToMousePosition();

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
        {
            CharacterController.HandleCharacterRotation(transform, hit.point, CharacterController.RotateVelocity, rotationSpeed);
        }
    }
}