using UnityEngine;
using Photon.Pun;

namespace GameNetwork
{
    public class PlayerManager : MonoBehaviourPun
    {
        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject localPlayerInstance;

        [Tooltip("Player Prefab HUD")]
        public GameObject playerHUD;

        protected virtual void Awake()
        {
            //Reseau
            if (GameObject.Find("GameNetworkManager") != null && photonView != null && photonView.IsMine)
            {
                localPlayerInstance = this.gameObject;
                localPlayerInstance.GetComponent<CharacterController>().InstantiateCharacterCameraAtStartOfTheGame();
            }
            ////Local
            else if (GameObject.Find("GameNetworkManager") == null)
            {
                GetComponent<CharacterController>().InstantiateCharacterCameraAtStartOfTheGame();
                playerHUD.SetActive(true);
            }   
        }

        private void Start()
        {
            //Everything that belong to the local player
            if (photonView.IsMine)
            {
                playerHUD.SetActive(true);
            }
        }
    }
}

