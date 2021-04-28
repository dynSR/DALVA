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

        private bool isMine;

        private PlayerManager[] pmList;

        public static EntityStats myCharacterStats = null;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            //Reseau
            if (GameObject.Find("GameNetworkManager") != null && photonView != null && photonView.IsMine)
            {
                localPlayerInstance = gameObject;
                localPlayerInstance.GetComponent<PlayerController>().InstantiateCharacterCameraAtStartOfTheGame();
            }
            //Local
            else if (GameObject.Find("GameNetworkManager") == null)
            {
                GetComponent<PlayerController>().InstantiateCharacterCameraAtStartOfTheGame();
                playerHUD.SetActive(true);
            }
        }

        private void Start()
        {
            ////Everything that belong to the local player
            //if (photonView.IsMine)
            //{
            //    playerHUD.SetActive(true);
            //    isMine = true;
            //}

            ////Match the team of the player and the character
            //if(photonView.Owner.CustomProperties["Team"].ToString() == "0") photonView.GetComponent<EntityStats>().EntityTeam = EntityTeam.DALVA;
            //else photonView.GetComponent<EntityStats>().EntityTeam = EntityTeam.HULRYCK;
        }
    }
}

