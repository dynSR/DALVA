using UnityEngine;

public class Haste : StatusEffectSystem
{
    [SerializeField] private float multiplicator;
    [SerializeField] private float valueToAdd;
    [Tooltip("If false, the value will be multiplicated by the value multiplicator.")]
    [SerializeField] private bool addsToStat;

    protected override void ApplyStatusEffectOnTarget(Transform target)
    {
        if (GetTargetStatusEffectHandler(target) != null)
        {
            if (GetTargetStatusEffectHandler(target).IsEffectAlreadyApplied(this)) return;

            if (!addsToStat)
                GetTargetCharacterController(target).CurrentMoveSpeed *= multiplicator;
            if (addsToStat)
                GetTargetCharacterController(target).CurrentMoveSpeed += valueToAdd;

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