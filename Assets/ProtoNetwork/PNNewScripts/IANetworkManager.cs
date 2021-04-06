using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace GameNetwork
{
    public class IANetworkManager : MonoBehaviourPun
    {
        public static IANetworkManager singleton;

        public void Awake()
        {
            if (singleton == null) singleton = this;
            else Destroy(gameObject);
        }

        [PunRPC]
        void StateUpdate(GameObject target,IState state, PhotonView player)
        {
            target.GetComponent<NPCController>().ChangeState(state);
        }

        [PunRPC]
        void MasterStateUpdate(GameObject target, IState state, PhotonView player)
        {
            photonView.RPC("StateUpdate", Photon.Pun.RpcTarget.All, target, state, player);
        }
    }
}

