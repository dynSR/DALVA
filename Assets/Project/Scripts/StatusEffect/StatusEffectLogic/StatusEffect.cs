using UnityEngine;
using UnityEngine.AI;

public enum TypeOfEffect { AffectsMovementSpeed, AffectsDamage, AffectsSomethingElse }

public abstract class StatusEffect : MonoBehaviour
{
    #region Status effect informations et components
    [Header("STATUS EFFECT INFORMATIONS & COMPONENTS")]
    [SerializeField] private string statusEffectName;
    [SerializeField] private string statusEffectDescription;
    [SerializeField] private float statusEffectDuration;
    [SerializeField] private Sprite statusEffectIcon;
    [SerializeField] private GameObject statusEffectVFXPrefab;
    [SerializeField] private AudioClip statusEffectSound;
    [SerializeField] private TypeOfEffect typeOfEffect;
    [SerializeField] private bool valueAffectedResetsBeforeApplyingStatuEffect;

    #region Public Variables - Statu effect informations et components
    public string StatusEffectName { get => statusEffectName; }
    public string StatusEffectDescription { get => statusEffectDescription; }
    public float StatusEffectDuration { get => statusEffectDuration; set => statusEffectDuration = value; }
    public Sprite StatusEffectIcon { get => statusEffectIcon; }
    public GameObject StatusEffectVFXPrefab { get => statusEffectVFXPrefab; }
    public AudioClip StatusEffectSound { get => statusEffectSound; }
    public TypeOfEffect TypeOfEffect { get => typeOfEffect; set => typeOfEffect = value; }
    public bool ValueAffectedResetsBeforeApplyingStatuEffect { get => valueAffectedResetsBeforeApplyingStatuEffect; }
    #endregion
    #endregion

    #region Target(s) informations
    //[SerializeField] private List<Transform> targets = new List<Transform>();
    [SerializeField] private Transform target;
    #region Public variables - Target(s) informations
    public Transform Target { get => target; set => target = value; }
    //public List<Transform> Targets { get => targets; set => targets = value; }
    private bool TargetIsNotSet => Target == null;
    #endregion
    #endregion

    public StatusEffectHandler StatusEffectHandler { get; set; }
    public StatusEffectContainer StatusEffectContainer { get; set; }

    protected abstract void ApplyStatusEffectOnTarget(Transform targetFound);
    public abstract void RemoveStatusEffect();

    protected virtual void CheckForExistingStatusEffect(StatusEffectHandler statusEffectHandler)
    {
        if (statusEffectHandler.AreThereSimilarExistingStatusEffects(this))
            statusEffectHandler.RemoveStatusEffectOfSameTypeAlreadyApplied();
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

            //if (Targets.Count <= 0)
            //{
            //    Targets.Add(other.transform);
            //}
            //else
            //{
            //    for (int i = 0; i < Targets.Count; i++)
            //    {
            //        if (other.transform != Targets[i])
            //            Targets.Add(other.transform);
            //    }
            //}

            ApplyStatusEffectOnTarget(Target);
            CreateVFXOnApplication(StatusEffectVFXPrefab, Target);
            PlaySoundOnApplication(StatusEffectSound, Target);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is out of trigger");

            //if (Target == null) return;
            //if (Target == other.transform)
            //    Target = null;

            //for (int i = 0; i < Targets.Count; i++)
            //{
            //    if (other.transform == Targets[i])
            //        Targets.Remove(other.transform);
            //}
        }
    }
    #endregion

    #region Getting necessaries informations about target(s) found
    public CharacterController GetTargetCharacterController(Transform targetFound)
    {
        return targetFound.GetComponent<CharacterController>();
    }

    public void SetNavMeshAgentSpeed(NavMeshAgent agent, float speedToSet)
    {
        agent.speed = speedToSet;
    }

    public StatusEffectHandler GetTargetStatusEffectHandler(Transform targetFound)
    {
        return targetFound.GetComponent<StatusEffectHandler>();
    }

    public PlayerHUD GetTargetHUD(Transform targetFound)
    {
        return targetFound.Find("PlayerHUD").GetComponent<PlayerHUD>();
    }
    #endregion

    private void CreateVFXOnApplication(GameObject vfxToCreate, Transform target)
    {
        if (vfxToCreate != null)
        {
            GameObject vfxCopy = Instantiate(vfxToCreate, target.position, target.rotation);
            vfxCopy.transform.SetParent(target);

            if (vfxCopy.GetComponent<LifeTimeHandler>() != null)
                StartCoroutine(vfxCopy.GetComponent<LifeTimeHandler>().DestroyAfterATime(StatusEffectDuration));
        }
    }

    private void PlaySoundOnApplication(AudioClip applicationSound, Transform target)
    {
        if (applicationSound != null)
            AudioSource.PlayClipAtPoint(applicationSound, target.position);
    }
}