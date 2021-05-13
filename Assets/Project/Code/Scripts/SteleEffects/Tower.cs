using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Range(1,4)]
    [SerializeField] private int amountOfProjectileToSpawn = 1;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform rangedAttackEmiterPosition;

    public bool CanAttack = true;

    EntityStats Stats => GetComponent<EntityStats>();

    public GameObject Projectile { get => projectile; }
    public int AmountOfProjectileToSpawn { get => amountOfProjectileToSpawn; set => amountOfProjectileToSpawn = value; }

    public IEnumerator ShotProjectileOntoTarget(EntityStats targetStats)
    {
        float delay = Stats.GetStat(StatType.AttackSpeed).Value;

        for (int i = 0; i < AmountOfProjectileToSpawn; i++)
        {
            if (targetStats.IsDead /*&& !CanAttack*/) yield break;

            CanAttack = false;

            yield return new WaitForSeconds(delay);

            GameObject autoAttackProjectile = Instantiate(projectile, rangedAttackEmiterPosition.position, projectile.transform.rotation);

            ProjectileLogic attackProjectile = autoAttackProjectile.GetComponent<ProjectileLogic>();

            attackProjectile.ProjectileType = ProjectileType.TravelsToAPosition;

            attackProjectile.ProjectileSender = transform;
            attackProjectile.Target = targetStats.transform;
        }

        //yield return new WaitForSeconds(delay);

        CanAttack = true;
    }
}