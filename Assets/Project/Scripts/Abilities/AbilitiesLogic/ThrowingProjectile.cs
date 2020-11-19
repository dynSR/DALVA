using System.Collections;
using UnityEngine;

public class ThrowingProjectile : MonoBehaviour
{
    [SerializeField] private Transform aimProjectileEmiterPos;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float rotationSpeed;

    private CharacterController CharacterController => GetComponent<CharacterController>();
    public Transform AimProjectileEmiterPos { get => aimProjectileEmiterPos; }

    public IEnumerator LaunchAProjectile(GameObject projectile, Transform spawnLocation, ProjectileType projectileType)
    {
        TurnCharacterTowardsLaunchDirection();

        yield return new WaitForSeconds(rotationSpeed);

        GameObject projectileInstance = Instantiate(projectile, spawnLocation.position, spawnLocation.rotation);

        ProjectileController projectileController = projectileInstance.GetComponent<ProjectileController>();
        projectileController.ProjectileType = projectileType;

        projectileController.ProjectileSender = transform;
        Debug.Log("ThrowingProjectile");
    }

    public void TurnCharacterTowardsLaunchDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
        {
            CharacterController.HandleCharacterRotation(transform, hit.point, CharacterController.RotateVelocity, rotationSpeed);
        }
    }
}
