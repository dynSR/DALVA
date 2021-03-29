
public class IdlingState : IState
{
    private NPCController controller;

    void IState.Enter(NPCController controller)
    {
        this.controller = controller;

        controller.IsInIdleState = true;

        controller.transform.LookAt(controller.PositionToLookAt);
        controller.Stats.CanTakeDamage = true;
        controller.AggroStep = 8;
        controller.handlingAggroSteps = false;
    }

    void IState.Exit()
    {
        controller.NPCInteractions.StoppingDistance = controller.Stats.GetStat(StatType.AttackRange).Value;
        controller.IsInIdleState = false;
    }

    void IState.OnUpdate()
    {

    }
}
