using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Range(1,4)]
    [SerializeField] private int amountOfProjectileToSpawn = 1;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float delayBetweenShot = 0.15f;
    [SerializeField] private Transform rangedAttackEmiterPosition;

    public bool CanAttack = true;

    public GameObject Projectile { get => projectile; }

    public IEnumerator ShotProjectileOntoTarget(EntityStats targetStats)
    {
        for (int i = 0; i < amountOfProjectileToSpawn; i++)
        {
            if (targetStats.IsDead) yield break;

            CanAttack = false;

            yield return new WaitForSeconds(delayBetweenShot);

            GameObject autoAttackProjectile = Instantiate(projectile, rangedAttackEmiterPosition.position, projectile.transform.rotation);

            ProjectileLogic attackProjectile = autoAttackProjectile.GetComponent<ProjectileLogic>();

            attackProjectile.ProjectileType = ProjectileType.TravelsToAPosition;

            attackProjectile.ProjectileSender = transform;
            attackProjectile.Target = targetStats.transform;
        }

        yield return new WaitForSeconds(delayBetweenShot);

        CanAttack = true;
    }
}