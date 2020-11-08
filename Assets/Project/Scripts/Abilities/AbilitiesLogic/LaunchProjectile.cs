using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    [SerializeField] private Transform emmiterPosition;
    [SerializeField] private LayerMask layer;
    [Range(100, 200)]
    [SerializeField] private int rotationSpeed = 150;

    public Transform EmmiterPosition { get => emmiterPosition; }

    public void LaunchAProjectile(GameObject projectile, Transform spawnLocation)
    {
        TurnCharacterTowardsLaunchDirection();
        Instantiate(projectile, spawnLocation.position, spawnLocation.rotation);
    }

    private void TurnCharacterTowardsLaunchDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        GameObject renderer = transform.GetChild(0).gameObject;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layer))
        {
            Vector3 targetPoint = ray.GetPoint(hit.distance);
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - renderer.transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            renderer.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, (float)rotationSpeed * Time.deltaTime);
        }
    }
}
