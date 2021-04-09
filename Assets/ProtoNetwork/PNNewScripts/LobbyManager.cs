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
    public class LobbyManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Fields

        [Header("List concern")]
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
        
        [Header("Buttons and other infos")]
        [Tooltip("The button to start a game (master only)")]
        public Button startGameButton;

        [Tooltip("The button to join Dalva's team")]
        public Button joinDalvaButton;

        [Tooltip("The button to join Huleryck's team")]
        public Button joinHulryckButton;

        [Tooltip("The TMPro text for room code")]
        public TextMeshProUGUI roomCodeText;
        #endregion

        #region IPunObservable implementation
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }


        #endregion

        #region Callbacks

        private void Start()
        {
            UpdatePlayerList(playerList, playerListTMPro);

            if (PhotonNetwork.IsMasterClient) startGameButton.interactable = true;

            roomCodeText.text = "Room code : " + PhotonNetwork.CurrentRoom.Name;
        }

        public override void OnJoinedRoom()
        {
            playerDalvaList.Clear();
            playerHulryckList.Clear();

            foreach (Player item in PhotonNetwork.PlayerList)
            {
                if (item.CustomProperties.ContainsKey("Team"))
                {
                    if (item.CustomProperties["Team"].ToString() == "0") playerDalvaList.Add(item.NickName);
                    else if (item.CustomProperties["Team"].ToString() == "1") playerHulryckList.Add(item.NickName);
                }
            }
            UpdatePlayerList(playerDalvaList, playerDalvaListTMPro);
            UpdatePlayerList(playerHulryckList, playerHulryckListTMPro);
        }

        public override void OnPlayerEnteredRoom(Player other)
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

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log(other.NickName + " leaved room");
            UpdatePlayerList(playerList, playerListTMPro);

            /*if (PhotonNetwork.IsMasterClient)
            {
                UpdatePlayerList(playerList, playerListTMPro);
                //startGameButton.interactable = false;
            }*/
        }

        public override void OnLeftRoom()
        {
            //SceneManager.LoadScene("PNMainMenu");
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
                foreach (Player item in PhotonNetwork.PlayerList)
                {
                    nameList.Add(item.NickName);
                }
            }
            //For other lists
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
                PhotonNetwork.LoadLevel("MapInPlaceHolder");
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("PNMainMenu");
        }

        public void JoinTeam(int team)
        {
            //if the player already have a team, join the other team (because the actual team button is locked)
            if(PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Team"))
            {
                PhotonNetwork.LocalPlayer.CustomProperties["Team"] = team;
            }
            //else, join the team the player pressed the button
            else
            {
                //0 = dalva, 1 = hulryck
                ExitGames.Client.Photon.Hashtable playerTeam = new ExitGames.Client.Photon.Hashtable { { "Team", team } };
                PhotonNetwork.SetPlayerCustomProperties(playerTeam);
            }

            //the button of your team is now non interactable
            if (team == 0)
            {
                joinDalvaButton.interactable = false;
                joinHulryckButton.interactable = true;
            }
            else
            {
                joinDalvaButton.interactable = true;
                joinHulryckButton.interactable = false;
            }
                
            gameObject.GetComponent<PhotonView>().RPC("RPC_UpdatePlayerTeam", RpcTarget.All, team);

            Debug.Log("You're in team " + team.ToString() + " (0 = dalva, 1 = hulryck)");
        }

        [PunRPC]
        public void RPC_UpdatePlayerTeam(int team, PhotonMessageInfo info)
        {
            if(team == 0)
            {
                playerDalvaList.Add(info.Sender.NickName);
                UpdatePlayerList(playerDalvaList, playerDalvaListTMPro);
                foreach (TextMeshProUGUI item in playerHulryckListTMPro)
                {
                    if (info.Sender.NickName == item.text) playerHulryckList.Remove(info.Sender.NickName);
                    UpdatePlayerList(playerHulryckList, playerHulryckListTMPro);
                }
            }
            else
            {
                playerHulryckList.Add(info.Sender.NickName);
                UpdatePlayerList(playerHulryckList, playerHulryckListTMPro);
                foreach (TextMeshProUGUI item in playerDalvaListTMPro)
                {
                    if (info.Sender.NickName == item.text) playerDalvaList.Remove(info.Sender.NickName);
                    UpdatePlayerList(playerDalvaList, playerDalvaListTMPro);
                }
            }
        }

        #endregion
    }
}

