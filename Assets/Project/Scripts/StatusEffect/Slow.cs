using UnityEngine;

public class Slow : StatusEffectSystem
{
    [SerializeField] private float divisor;
    [SerializeField] private float valueToTakeOff;
    [Tooltip("If false, the value will be divied by the value divisor.")]
    [SerializeField] private bool takesOffToStat;

    protected override void ApplyStatusEffectOnTarget(Transform target)
    {
        if (GetTargetStatusEffectHandler(target) != null)
        {
            if (GetTargetStatusEffectHandler(target).IsEffectAlreadyApplied(this)) return;

            if (!takesOffToStat)
                GetTargetCharacterController(target).CurrentMoveSpeed /= divisor;
            if (takesOffToStat)
                GetTargetCharacterController(target).CurrentMoveSpeed -= valueToTakeOff;

            GetTargetStatusEffectHandler(target).AddNewEffect(this);
        }
    }

    public override void RemoveEffect()
    {
        if (GetTargetStatusEffectHandler(Target) != null)
        {
            GetTargetCharacterController(Target).CurrentMoveSpeed = GetTargetCharacterController(Target).InitialMoveSpeed;
            // ajouter l'additive speed à la fin de l'effet
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

}