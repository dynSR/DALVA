using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractions : InteractionSystem
{
    public bool HasATarget => Target != null;
    public bool debugHASATARGET;

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
