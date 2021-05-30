using UnityEngine;

    public class PlayerManager : MonoBehaviour
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
            GetComponent<PlayerController>().InstantiateCharacterCameraAtStartOfTheGame();
            playerHUD.SetActive(true);
        }
    }

