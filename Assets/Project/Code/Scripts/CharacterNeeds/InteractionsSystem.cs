using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatType { MeleeCombat, RangedCombat }

public class InteractionsSystem : TargetHandler
{
    [Header("BASIC ATTACK")]
    [SerializeField] private Transform rangedAttackEmiterPosition;
    [SerializeField] private GameObject rangedAttackProjectile;

    [Header("INTERACTIONS STATE")]
    [SerializeField] private bool canPerformAttack = true;
    [SerializeField] private bool isHarvesting = false;

    public bool CanPerformAttack { get => canPerformAttack; set => canPerformAttack = value; }
    public bool IsHarvesting { get => isHarvesting; set => isHarvesting = value; }

    public CombatType CombatAttackType { get; set; }

    protected override void Update() => base.Update();

    #region Interaction
    public override void Interact()
    {
        if (Target.GetComponent<CharacterStats>() != null && Target.GetComponent<CharacterStats>().IsDead)
        {
            ResetInteractionState();
            CharacterAnimator.SetTrigger("NoTarget");
            Target = null;
            return;
        }

        if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Harvester && Target.GetComponent<HarvesterLogic>().IsInteractable)
        {
            CharacterAnimator.SetBool("Attack", false);

            IsHarvesting = true;
            CharacterAnimator.SetBool("IsCollecting", true);

            Target.GetComponent<HarvesterLogic>().PlayerFound = transform;
            Target.GetComponent<HarvesterLogic>().harvestState = HarvestState.PlayerIsHarvestingRessources;

            Debug.Log("Interaction with harvester !");
        }

        else if (Target.GetComponent<EntityDetection>().TypeOfEntity == TypeOfEntity.Enemy && CanPerformAttack)
        {
            CharacterAnimator.SetBool("IsCollecting", false);

            if (CombatAttackType == CombatType.MeleeCombat)
            {
                StartCoroutine(AttackInterval(CombatAttackType));
                Debug.Log("Melee Attack performed !");
            }
            else if (CombatAttackType == CombatType.RangedCombat)
            {
                StartCoroutine(AttackInterval(CombatAttackType));
                Debug.Log("Ranged Attack performed !");
            }
        }
    }

    IEnumerator AttackInterval(CombatType attackType)
    {
        Debug.Log("Attack Interval");

        if (attackType == CombatType.MeleeCombat)
        {
            CharacterAnimator.SetFloat("AttackSpeed", CharacterStats.CurrentAttackSpeed);
            CharacterAnimator.SetBool("Attack", true);

            //MeleeAttack(); //Debug without animation

            CanPerformAttack = false;
        }
        else if (attackType == CombatType.RangedCombat)
        {
            CharacterAnimator.SetFloat("AttackSpeed", CharacterStats.CurrentAttackSpeed);
            CharacterAnimator.SetBool("Attack", true);

            //RangedAttack(); //Debug without animation

            CanPerformAttack = false;
        }

        yield return new WaitForSeconds(1 / CharacterStats.CurrentAttackSpeed);

        if (attackType == CombatType.MeleeCombat)
        {
            CharacterAnimator.SetBool("Attack", false);
            CanPerformAttack = true;
        }
        else if (attackType == CombatType.RangedCombat)
        {
            CharacterAnimator.SetBool("Attack", false);
            CanPerformAttack = true;
        }
    }

    #region Behaviours of every type of attack - Melee / Ranged
    public void MeleeAttack()
    {
        if (Target != null)
        {
            if (Target.GetComponent<CharacterStats>() != null)
            {
                Target.GetComponent<CharacterStats>().TakeDamage(transform, CharacterStats.CurrentAttackDamage, CharacterStats.CurrentMagicDamage, CharacterStats.CurrentCriticalStrikeChance, CharacterStats.CurrentCriticalStrikeMultiplier, CharacterStats.CurrentArmorPenetration, CharacterStats.CurrentMagicResistancePenetration);
            }
        }

        CanPerformAttack = true;
    }

    public void RangedAttack()
    {
        if (Target != null)
        {
            Debug.Log("Auto Attack Projectile Instantiated");

            GameObject autoAttackProjectile = Instantiate(rangedAttackProjectile, rangedAttackEmiterPosition.position, rangedAttackProjectile.transform.rotation);

            ProjectileLogic attackProjectile = autoAttackProjectile.GetComponent<ProjectileLogic>();

            attackProjectile.ProjectileType = ProjectileType.TravelsToAPosition;
            attackProjectile.ProjectileSender = transform;
            attackProjectile.Target = Target;
        }

        CanPerformAttack = true;
    }
    #endregion

    public override void ResetInteractionState()
    {
        CanPerformAttack = true;
        CharacterAnimator.SetBool("Attack", false);

        IsHarvesting = false;
        CharacterAnimator.SetBool("IsCollecting", false);
    }
    #endregion

}