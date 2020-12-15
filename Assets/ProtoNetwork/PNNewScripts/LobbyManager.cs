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
        private List<string> playerList = new List<string>();

        [Tooltip("The list of all UI text for Dalva's players")]
        [SerializeField]
        public List<TextMeshProUGUI> playerDalvaListTMPro;
        public List<string> playerDalvaList = new List<string>();

        [Tooltip("The list of all UI text for Hulryck's players")]
        [SerializeField]
        public List<TextMeshProUGUI> playerHulryckListTMPro;
        public List<string> playerHulryckList = new List<string>();

        [Tooltip("The button to start a game (master only)")]
        [SerializeField]
        public Button startGameButton;

        [Tooltip("The button to join Dalva's team")]
        [SerializeField]
        public Button joinDalvaButton;

        [Tooltip("The button to join Huleryck's team")]
        [SerializeField]
        public Button joinHuleryckButton;

        [Tooltip("The TMPro text for room code")]
        [SerializeField]
        public TextMeshProUGUI roomCodeText;
        #endregion

        #region Callbacks

        private void Start()
        {
            UpdatePlayerList(playerList, playerListTMPro);

            if (PhotonNetwork.IsMasterClient) startGameButton.interactable = true;
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
        {
            Debug.Log(other.NickName + " entered room");
            UpdatePlayerList(playerList, playerListTMPro);

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
                UpdatePlayerList(playerList, playerListTMPro);
                //startGameButton.interactable = false;
            }
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("PNMainMenu");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            //SceneManager.LoadScene("PNMainMenu");
        }
        #endregion

        #region Methods

        private void UpdatePlayerList(List<string> nameList, List<TextMeshProUGUI> nameTextList)
        {
            //For general player list
            if(nameList == playerList)
            {
                nameList.Clear();
                foreach (Photon.Realtime.Player item in PhotonNetwork.PlayerList)
                {
                    nameList.Add(item.NickName);
                }
            }

            for (int i = 0; i < nameTextList.Count; i++)
            {
                if(i < nameList.Count)
                {
                    if (nameTextList[i].text != nameList[i])
                    {
                        nameTextList[i].text = nameList[i];
                    }
                }
                else
                {
                    nameTextList[i].text = "";
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

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();

        }

        public void JoinTeam(string team)
        {
            gameObject.GetComponent<PhotonView>().RPC("RPCUpdatePlayerList", RpcTarget.AllBuffered, PhotonNetwork.NickName, team);
            if(team == "Dalva")
            {
                joinDalvaButton.interactable = false;
                joinHuleryckButton.interactable = true;
                PlayerManager.localPlayerInstance.GetComponent<PlayerManager>().dalvasTeam = true;
            }
            else
            {
                joinDalvaButton.interactable = true;
                joinHuleryckButton.interactable = false;
                PlayerManager.localPlayerInstance.GetComponent<PlayerManager>().dalvasTeam = false;
            }
        }

        [PunRPC]
        public void RPCUpdatePlayerList(string nickname, string team)
        {
            if(team == "Dalva")
            {
                foreach(TextMeshProUGUI item in playerHulryckListTMPro)
                {
                    if (nickname != item.text) { }
                    else playerHulryckList.Remove(nickname);
                    UpdatePlayerList(playerHulryckList, playerHulryckListTMPro);
                }
                playerDalvaList.Add(nickname);
                UpdatePlayerList(playerDalvaList, playerDalvaListTMPro);
            }
            else
            {
                foreach (TextMeshProUGUI item in playerDalvaListTMPro)
                {
                    if (nickname != item.text) { }
                    else playerDalvaList.Remove(nickname);
                    UpdatePlayerList(playerDalvaList, playerDalvaListTMPro);
                }
                playerHulryckList.Add(nickname);
                UpdatePlayerList(playerHulryckList, playerHulryckListTMPro);
            }
        }

        #endregion
    }
}

