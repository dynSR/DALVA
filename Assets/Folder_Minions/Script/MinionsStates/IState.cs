using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    //Prépare l'état
    void Enter(MinionBehaviour parent);

    void Update();

    void Exit();
}