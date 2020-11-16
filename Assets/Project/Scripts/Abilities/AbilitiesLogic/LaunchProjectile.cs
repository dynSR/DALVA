using System.Collections;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    [SerializeField] private Transform emmiterPosition;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float rotationSpeed;

    private CharacterController CharacterController => GetComponent<CharacterController>();
    public Transform EmmiterPosition { get => emmiterPosition; }

    public IEnumerator LaunchAProjectile(GameObject projectile, Transform spawnLocation, ProjectileType projectileType)
    {
        TurnCharacterTowardsLaunchDirection();

        yield return new WaitForSeconds(rotationSpeed);

        Instantiate(projectile, spawnLocation.position, spawnLocation.rotation);

        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.ProjectileType = projectileType;

        projectileController.ProjectileSender = transform;
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
