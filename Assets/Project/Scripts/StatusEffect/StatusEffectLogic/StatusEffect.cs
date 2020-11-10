using UnityEngine;

public enum TypeOfEffect { EffectToMovementSpeed, EffectToDamage, EffectToSomethingElse }

public abstract class StatusEffect : MonoBehaviour
{
    [Header("CORE PARAMETERS")]
    [SerializeField] private string statusEffectName;
    [SerializeField] private string statusEffectDescription;
    [SerializeField] private Sprite statusEffectIcon;
    //[SerializeField] private List<Transform> targets = new List<Transform>();
    [SerializeField] private Transform target;
    [SerializeField] private GameObject statusEffectPrefab;
    [SerializeField] private bool doStatusEffectResetTheValueAffectedToInitialValueBeforeApplying;
    [SerializeField] private TypeOfEffect typeOfEffect;

    [Header("NUMERIC PARAMETERS")]
    [SerializeField] private float statusEffectDuration;
    public string StatusEffectDescription { get => statusEffectDescription; }
    public string StatusEffectName { get => statusEffectName; }
    public GameObject StatusEffectPrefab { get => statusEffectPrefab; }
    public float StatusEffectDuration { get => statusEffectDuration; set => statusEffectDuration = value; }
    public Sprite StatusEffectIcon { get => statusEffectIcon; }
    public Transform Target { get => target; set => target = value; }
    //public List<Transform> Targets { get => targets; set => targets = value; }
    public StatusEffectHandler StatusEffectHandler { get; set; }
    public TypeOfEffect TypeOfEffect { get => typeOfEffect; set => typeOfEffect = value; }
    public StatusEffectContainer StatusEffectContainer { get; set; }
    public bool DoStatusEffectResetTheValueAffectedToInitialValueBeforeApplying { get => doStatusEffectResetTheValueAffectedToInitialValueBeforeApplying; }
    private bool TargetWasNotFound => Target == null;


    protected abstract void ApplyStatusEffectOnTarget(Transform targetFound);
    public abstract void RemoveStatusEffect();

    protected virtual void CheckForExistingStatusEffect(StatusEffectHandler statusEffectHandler)
    {
        if (statusEffectHandler.AreThereSimilarExistingStatusEffectApplied(this))
            statusEffectHandler.RemoveStatusEffectOfSameTypeThatHasAlreadyBeenApplied();
    }

    #region Adding Or Removing Target
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is in trigger");
            if (TargetWasNotFound)
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

    public CharacterController GetTargetCharacterController(Transform targetFound)
    {
        return targetFound.GetComponent<CharacterController>();
    }

    public StatusEffectHandler GetTargetStatusEffectHandler(Transform targetFound)
    {
        return targetFound.GetComponent<StatusEffectHandler>();
    }

    public PlayerHUD GetTargetHUD(Transform targetFound)
    {
        return targetFound.Find("PlayerHUD").GetComponent<PlayerHUD>();
    }
}

