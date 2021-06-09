public class NPCInteractions : InteractionSystem
{
    public bool HasATarget => Target != null;

    #region Interaction
    public override void Interact()
    {
        base.Interact();
    }

    public override void ResetInteractionState()
    {
        base.ResetInteractionState();
    }
    #endregion
}