using UnityEngine;
using UnityEngine.AI;

public abstract class StatusEffectLogic : MonoBehaviour
{
    [SerializeField] private StatusEffect statusEffect;
    [SerializeField] private Transform target;
    public Transform Target { get => target; set => target = value; }
    private bool TargetIsNotSet => Target == null;

    public StatusEffectHandler StatusEffectHandler { get; set; }
    public StatusEffectContainerLogic StatusEffectContainer { get; set; }
    public StatusEffect StatusEffect { get => statusEffect; }

    protected abstract void ApplyStatusEffectOnTarget(Transform targetFound);
    public abstract void RemoveEffect();

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

            ApplyStatusEffectOnTarget(Target);
            CreateVFXOnApplication(StatusEffect.StatusEffectVFXPrefab, Target);
            PlaySoundOnApplication(StatusEffect.StatusEffectSound, Target);
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

            if (vfxCopy.GetComponent<Lifetime>() != null)
                StartCoroutine(vfxCopy.GetComponent<Lifetime>().DestroyAfterATime(StatusEffect.StatusEffectDuration));
        }
    }

    private void PlaySoundOnApplication(AudioClip applicationSound, Transform target)
    {
        if (applicationSound != null)
            AudioSource.PlayClipAtPoint(applicationSound, target.position);
    }
    #endregion
}