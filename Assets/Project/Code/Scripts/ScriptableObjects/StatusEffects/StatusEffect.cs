using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect_", menuName = "ScriptableObjects/StatusEffects", order = 1)]
public class StatusEffect : ScriptableObject
{
    [Header("STATUS EFFECT INFORMATIONS & COMPONENTS")]
    [SerializeField] private string statusEffectName;
    [SerializeField] private string statusEffectDescription;
    [SerializeField] private float statusEffectDuration;
    [SerializeField] private int statusEffectId;
    [SerializeField] private Sprite statusEffectIcon;
    [SerializeField] private GameObject statusEffectVFXPrefab;

    public string StatusEffectName { get => statusEffectName; }
    public string StatusEffectDescription { get => statusEffectDescription; }
    public float StatusEffectDuration { get => statusEffectDuration; set => statusEffectDuration = value; }
    public int StatusEffectId { get => statusEffectId; }
    public Sprite StatusEffectIcon { get => statusEffectIcon; }
    public GameObject StatusEffectVFXPrefab { get => statusEffectVFXPrefab; }

    [Header("LOGIC STUFFS")]
    [SerializeField] private List<StatModifier> statModifiers;
    public GameObject CreatedVFX { get; set; }
    public StatusEffectHandler TargetStatusEffectHandler { get; set; }
    public StatusEffectContainer StatusEffectContainer { get; set; }

    #region Apply - Remove Effect
    public void ApplyEffect(Transform target)
    {
        if (GetTargetStatusEffectHandler(target) != null)
        {
            TargetStatusEffectHandler = GetTargetStatusEffectHandler(target);

            if (GetTargetStatusEffectHandler(target).IsEffectAlreadyApplied(this))
            {
                GetTargetStatusEffectHandler(target).ResetCooldown(this);
                return;
            }

            for (int i = 0; i < statModifiers.Count; i++)
            {
                GetTargetStats(target).GetStat(statModifiers[i].StatType).AddModifier(statModifiers[i]);

                if (statModifiers[i].StatType == StatType.MovementSpeed)
                {
                    GetTargetController(target).SetNavMeshAgentSpeed(GetTargetController(target).Agent, GetTargetStats(target).GetStat(StatType.MovementSpeed).Value);
                }
            }
            
            GetTargetStatusEffectHandler(target).AddNewEffect(this);
            CreateVFXOnApplication(StatusEffectVFXPrefab, target);
        }
    }

    public void RemoveEffect(Transform target)
    {
        for (int i = 0; i < statModifiers.Count; i++)
        {
            GetTargetStats(target).GetStat(statModifiers[i].StatType).RemoveModifier(statModifiers[i]);

            if (statModifiers[i].StatType == StatType.MovementSpeed)
            {
                GetTargetController(target).SetNavMeshAgentSpeed(GetTargetController(target).Agent, GetTargetStats(target).GetStat(StatType.MovementSpeed).Value);
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

    public CharacterStat GetTargetStats(Transform target)
    {
        return target.GetComponent<CharacterStat>();
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