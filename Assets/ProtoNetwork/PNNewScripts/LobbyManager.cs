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

        public override void OnCreatedRoom()
        {
            UpdatePlayerList();
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log(other.NickName + " entered room");
            
            if (PhotonNetwork.IsMasterClient)
            {
                GetComponent<PhotonView>().RPC("PUNUpdateOtherNameList", RpcTarget.All);
                startGameButton.interactable = true;
                /*if(PhotonNetwork.PlayerList.Length == 6)
                {
                    startGameButton.interactable = true;
                }
                else
                {
                    startGameButton.interactable = false;
                }*/
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log(other.NickName + " leaved room");

            if (PhotonNetwork.IsMasterClient)
            {
                GetComponent<PhotonView>().RPC("PUNUpdateOtherNameList", RpcTarget.All);

                //startGameButton.interactable = false;
            }
        }

        #endregion

        #region Methods

        [PunRPC]
        void PUNUpdateOtherNameList()
        {
            UpdatePlayerList();
        }

        private void UpdatePlayerList()
        {
            playerList.Clear();
            foreach (Player item in PhotonNetwork.PlayerList)
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

