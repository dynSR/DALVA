using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

namespace GameNetwork
{
    public class GameNetworkManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Fields
        public static GameNetworkManager singleton;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public Transform spawn;

        #endregion

        #region Callbacks

        public void Awake()
        {
            if (singleton == null) singleton = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (playerPrefab == null)
            {
                Debug.Log("Missing player prefab", this);
            }
            else
            {
                if (PlayerManager.localPlayerInstance == null)
                {
                    PhotonNetwork.Instantiate(this.playerPrefab.name, spawn.position, Quaternion.identity);
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }

        #endregion
    }
}

