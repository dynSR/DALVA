using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType
{
    Harmless,
    Harmful,
}

[CreateAssetMenu(fileName = "StatusEffect_", menuName = "ScriptableObjects/StatusEffects", order = 1)]
public class StatusEffect : ScriptableObject
{
    [Header("CORE INFORMATIONS")]
    [SerializeField] private string statusEffectName;
    [SerializeField] private string statusEffectDescription;
    [SerializeField] private float statusEffectDuration;
    [SerializeField] private StatusEffectType type;
    [SerializeField] private int statusEffectId;

    [Header("APPEARENCE")]
    [SerializeField] private Sprite statusEffectIcon;
    [SerializeField] private GameObject statusEffectVFXPrefab;

    [Header("PROPERTIES")]
    [SerializeField] private bool canApplyOnAlly = false;
    [SerializeField] private bool canStunTarget = false;
    [SerializeField] private bool canRootTarget = false;
    [SerializeField] private bool isStackable = false;

    public string StatusEffectName { get => statusEffectName; }
    public string StatusEffectDescription { get => statusEffectDescription; }
    public float StatusEffectDuration { get => statusEffectDuration; set => statusEffectDuration = value; }
    public int StatusEffectId { get => statusEffectId; }
    public Sprite StatusEffectIcon { get => statusEffectIcon; }
    public GameObject StatusEffectVFXPrefab { get => statusEffectVFXPrefab; }
    public StatusEffectType Type { get => type; set => type = value; }

    [Header("LOGIC STUFFS")]
    [SerializeField] private List<StatModifier> statModifiers;
    public GameObject CreatedVFX { get; set; }
    public StatusEffectHandler TargetStatusEffectHandler { get; set; }
    public StatusEffectContainer StatusEffectContainer { get; set; }
    public bool CanApplyOnAlly { get => canApplyOnAlly; set => canApplyOnAlly = value; }
    public bool CanStunTarget { get => canStunTarget; set => canStunTarget = value; }
    public bool CanRootTarget { get => canRootTarget; set => canRootTarget = value; }
    public bool IsStackable { get => isStackable; set => isStackable = value; }
    public List<StatModifier> StatModifiers { get => statModifiers; set => statModifiers = value; }

    #region Apply - Remove Effect
    public void ApplyEffect(Transform target)
    {
        if (GetTargetStatusEffectHandler(target) != null)
        {
            TargetStatusEffectHandler = GetTargetStatusEffectHandler(target);

            if (GetTargetStatusEffectHandler(target).IsEffectAlreadyApplied(this) && !IsStackable)
            {
                GetTargetStatusEffectHandler(target).ResetCooldown(this);
                return;
            }

            for (int i = 0; i < StatModifiers.Count; i++)
            {
                GetTargetStats(target).GetStat(StatModifiers[i].StatType).AddModifier(StatModifiers[i]);
                GetTargetStats(target).UpdateNavMeshAgentSpeed(StatModifiers[i].StatType);
                //if (StatModifiers[i].StatType == StatType.MovementSpeed)
                //{
                //    GetTargetController(target).SetNavMeshAgentSpeed(GetTargetController(target).Agent, GetTargetStats(target).GetStat(StatType.MovementSpeed).Value);
                //}
            }

            GetTargetStats(target).UpdateStats();

            if (CanRootTarget) GetTargetController(target).RootTarget();
            if (CanStunTarget) GetTargetController(target).StunTarget();

            GetTargetStatusEffectHandler(target).AddNewEffect(this);
            CreateVFXOnApplication(StatusEffectVFXPrefab, target);
        }
    }

    public void RemoveEffect(Transform target, StatusEffect effectToRemove)
    {
        for (int i = 0; i < StatModifiers.Count; i++)
        {
            GetTargetStats(target).GetStat(StatModifiers[i].StatType).RemoveModifier(StatModifiers[i]);
            GetTargetStats(target).GetStat(StatModifiers[i].StatType).MaxValue = GetTargetStats(target).GetStat(StatModifiers[i].StatType).CalculateValue();

            GetTargetStats(target).UpdateNavMeshAgentSpeed(StatModifiers[i].StatType);

            //if (StatModifiers[i].StatType == StatType.MovementSpeed)
            //{
            //    GetTargetController(target).SetNavMeshAgentSpeed(GetTargetController(target).Agent, GetTargetStats(target).GetStat(StatType.MovementSpeed).Value);
            //}
            if (StatModifiers[i].StatType == StatType.Shield)
            {
                GetTargetStats(target).RemoveShieldOnTarget(target, StatModifiers[i].Value);
            }

            GetTargetStats(target).UpdateStats();
        }

        //Maybe it will not work has to be checked !!!!!!!!!!!
        if (GetTargetStatusEffectHandler(target).AppliedStatusEffects.Count > 1)
        {
            for (int i = 0; i < GetTargetStatusEffectHandler(target).AppliedStatusEffects.Count; i++)
            {
                StatusEffect effect = GetTargetStatusEffectHandler(target).AppliedStatusEffects[i].statusEffect;

                if (effect != effectToRemove
                    && effectToRemove.CanRootTarget
                    && !effect.CanRootTarget) 
                    GetTargetController(target).UnRootTarget();

                if (effect != effectToRemove 
                    && effectToRemove.CanStunTarget
                    && !effect.CanStunTarget)
                {
                    GetTargetController(target).GetComponent<InteractionSystem>().CanPerformAttack = true;
                    GetTargetController(target).UnStunTarget();
                }   
            }
        }
        else
        {
            if (effectToRemove.CanRootTarget)
                GetTargetController(target).IsRooted = false;

            if (effectToRemove.CanStunTarget)
            {
                GetTargetController(target).GetComponent<InteractionSystem>().CanPerformAttack = true;
                GetTargetController(target).IsStunned = false;
            }  
        }

        Destroy(CreatedVFX);
    }
    #endregion

    #region Get target informations
    public CharacterController GetTargetController(Transform target)
    {
        return target.GetComponent<CharacterController>();
    }

    public EntityStats GetTargetStats(Transform target)
    {
        return target.GetComponent<EntityStats>();
    }

    public StatusEffectHandler GetTargetStatusEffectHandler(Transform target)
    {
        return target.GetComponent<StatusEffectHandler>();
    }
    #endregion

    #region Feedback
    private void CreateVFXOnApplication(GameObject vfxToCreate, Transform target)
    {
        if (vfxToCreate != null)
        {
            GameObject vfxCopy = Instantiate(vfxToCreate, target.position, target.rotation);
            vfxCopy.transform.SetParent(target);
            CreatedVFX = vfxCopy;
        }
    }
    #endregion
}