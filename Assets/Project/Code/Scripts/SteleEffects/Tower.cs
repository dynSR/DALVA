using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform rangedAttackEmiterPosition;

    public bool CanAttack = true;

    EntityStats Stats => GetComponent<EntityStats>();

    public GameObject Projectile { get => projectile; }

    public IEnumerator ShotProjectileOntoTarget(EntityStats targetStats)
    {
        Debug.Log("ShotProjectileOntoTarget");

        float delay = Stats.GetStat(StatType.AttackSpeed).Value;

        if (targetStats.IsDead || !CanAttack) yield break;

        GameObject autoAttackProjectile = Instantiate(projectile, rangedAttackEmiterPosition.position, projectile.transform.rotation);
        ProjectileLogic attackProjectile = autoAttackProjectile.GetComponent<ProjectileLogic>();

        attackProjectile.ProjectileType = ProjectileType.TravelsToAPosition;
        attackProjectile.ProjectileSender = transform;
        attackProjectile.Target = targetStats.transform;

        CanAttack = false;

        yield return new WaitForSeconds(1 / delay);

        CanAttack = true;
    }
}