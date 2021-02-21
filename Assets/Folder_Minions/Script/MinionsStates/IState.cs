using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    //Prépare l'état
    void Enter(Sc_Minions_Base parent);

    void Update();

    void Exit();

}
