using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    //Prépare l'état
    void Enter(NPCController controller);

    void OnUpdate();

    void Exit();
}