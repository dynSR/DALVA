using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace GameNetwork
{
    public class IANetworkManager : MonoBehaviourPun
    {
        [PunRPC]
        void StateUpdate(GameObject target,IState state, PhotonView player)
        {
            target.GetComponent<NPCController>().ChangeState(state);
        }
    }
}

