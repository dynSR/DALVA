using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlingState : IState
{
    private NPCController controller;

    void IState.Enter(NPCController controller)
    {
        this.controller = controller;
    }

    void IState.Exit()
    {
        throw new System.NotImplementedException();
    }

    void IState.OnUpdate()
    {
        throw new System.NotImplementedException();
    }
}
