using System.Collections;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    [SerializeField] private Transform emmiterPosition;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float rotationSpeed;

    private CharacterController CharacterController => GetComponent<CharacterController>();
    private CombatBehaviour CombatBehaviour=> GetComponent<CombatBehaviour>();
    public Transform EmmiterPosition { get => emmiterPosition; }

    public IEnumerator LaunchAProjectile(GameObject projectile, Transform spawnLocation, ProjectileType projectileType)
    {
        Instantiate(projectile, spawnLocation.position, spawnLocation.rotation);

        ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
        projectileController.ProjectileType = projectileType;

        yield return new WaitForSeconds(rotationSpeed);

        
        projectileController.ProjectileSender = transform;
        projectileController.Target = CombatBehaviour.TargetedEnemy;

        Debug.Log("Projectile Type : " + projectile.GetComponent<ProjectileController>().ProjectileType.ToString());
        Debug.Log("Projectile Sender : " + projectile.GetComponent<ProjectileController>().ProjectileSender.ToString());
        Debug.Log("Projectile Target : " + projectile.GetComponent<ProjectileController>().Target.ToString());
    }

    public void TurnCharacterTowardsLaunchDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //GameObject renderer = transform.GetChild(0).gameObject;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layer))
        {
            CharacterController.HandleCharacterRotation(transform, hit.point, CharacterController.RotateVelocity, rotationSpeed);

            //Vector3 targetPoint = hit.point;
            //Quaternion targetRotation = Quaternion.LookRotation(forward: targetPoint - renderer.transform.position);
            //targetRotation.x = 0;
            //targetRotation.z = 0;
            //renderer.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, (float)rotationSpeed * Time.deltaTime);
        }
    }
}
