using UnityEngine;

public class StatusEffectLogic : MonoBehaviour
{
    [SerializeField] private StatModifier statModifier;
    [SerializeField] private StatusEffect statusEffect;
    [SerializeField] private Transform target;
    public Transform Target { get => target; set => target = value; }
    private bool TargetIsNotSet => Target == null;

    public GameObject CreatedVFX { get; set; }

    public StatusEffect StatusEffect { get => statusEffect; }
    public StatusEffectHandler StatusEffectHandler { get; set; }
    public StatusEffectContainerLogic StatusEffectContainer { get; set; }
    
    protected virtual void ApplyStatusEffectOnTarget()
    {
        if (GetTargetStatusEffectHandler(Target) != null)
        {
            if (GetTargetStatusEffectHandler(Target).IsEffectAlreadyApplied(this))
            {
                GetTargetStatusEffectHandler(Target).ResetCooldown(this);
                return;
            } 

            Target.GetComponent<CharacterStat>().GetStat(statModifier.StatType).AddModifier(statModifier);

            if(statModifier.StatType == StatType.Movement_Speed)
            {
                Target.GetComponent<CharacterController>().SetNavMeshAgentSpeed(
                    Target.GetComponent<CharacterController>().Agent, 
                    Target.GetComponent<CharacterStat>().GetStat(StatType.Movement_Speed).Value);
            }

            GetTargetStatusEffectHandler(Target).AddNewEffect(this);
            CreateVFXOnApplication(StatusEffect.StatusEffectVFXPrefab, Target);
            PlaySoundOnApplication(StatusEffect.StatusEffectSound, Target);
        }
    }
    public virtual void RemoveEffect()
    {
        Target.GetComponent<CharacterStat>().GetStat(statModifier.StatType).RemoveModifier(statModifier);
        Destroy(CreatedVFX);
    }

    #region Adding or removing target(s) with trigger events
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is in trigger");

            if (TargetIsNotSet)
                Target = other.transform;
            else if (Target != other.transform)
                Target = other.transform;

            ApplyStatusEffectOnTarget();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is out of trigger");
        }
    }

    #endregion

    #region Getting necessaries informations about target(s) found
    public CharacterStat GetTargetCharacterStats(Transform targetFound)
    {
        return targetFound.GetComponent<CharacterStat>();
    }

    public StatusEffectHandler GetTargetStatusEffectHandler(Transform targetFound)
    {
        return targetFound.GetComponent<StatusEffectHandler>();
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

    private void PlaySoundOnApplication(AudioClip applicationSound, Transform target)
    {
        if (applicationSound != null)
            AudioSource.PlayClipAtPoint(applicationSound, target.position);
    }
    #endregion
}