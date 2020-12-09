using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

using Photon.Pun;
using Photon.Realtime;

namespace GameNetwork
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        #region Fields

        [Tooltip("The list of all UI text for players' name")]
        [SerializeField]
        public List<TextMeshProUGUI> playerListTMPro;
        public List<string> playerList = new List<string>();
        [Tooltip("The button to start a game (master only)")]
        [SerializeField]
        public Button startGameButton;

        #endregion

        #region Callbacks

        private void Start()
        {
            UpdatePlayerList();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
        {
            Debug.Log(other.NickName + " entered room");
            UpdatePlayerList();
            /*if (PhotonNetwork.IsMasterClient)
            {
                if(PhotonNetwork.PlayerList.Length == 6)
                {
                    startGameButton.interactable = true;
                }
                else
                {
                    startGameButton.interactable = false;
                }
            }*/
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player other)
        {
            Debug.Log(other.NickName + " leaved room");

            if (PhotonNetwork.IsMasterClient)
            {
                UpdatePlayerList();
                //startGameButton.interactable = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene("PNMainMenu");
        }
        #endregion

        #region Methods

        private void UpdatePlayerList()
        {
            playerList.Clear();
            foreach (Photon.Realtime.Player item in PhotonNetwork.PlayerList)
            {
                playerList.Add(item.NickName);
            }

            for (int i = 0; i < 6; i++)
            {
                if(i < playerList.Count)
                {
                    if (playerListTMPro[i].text != playerList[i])
                    {
                        playerListTMPro[i].text = playerList[i];
                    }
                }
                else
                {
                    playerListTMPro[i].text = "";
                }
            }
        }

        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("PNSandbox");
            }
        }

        #endregion
    }
}

