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

        [Tooltip("The list of all UI text for players' name")]
        [SerializeField]
        public List<TextMeshProUGUI> playerListTMPro;
        private List<string> playerList = new List<string>();

        [Tooltip("The list of all UI text for Dalva's players")]
        [SerializeField]
        public List<TextMeshProUGUI> playerDalvaListTMPro;
        public List<string> playerDalvaList = new List<string>();
        [SerializeField]
        public string joiningDalvaTeam
        {
            get { return _joiningDalvaTeam; }
            set
            {
                _joiningDalvaTeam = value;
                if (_joiningDalvaTeam == joiningDalvaTeam)
                {
                    foreach (TextMeshProUGUI item in playerHulryckListTMPro)
                    {
                        if (joiningDalvaTeam != item.text) { }
                        else playerHulryckList.Remove(_joiningDalvaTeam);
                        UpdatePlayerList(playerHulryckList, playerHulryckListTMPro);
                    }
                    playerDalvaList.Add(_joiningDalvaTeam);
                    UpdatePlayerList(playerDalvaList, playerDalvaListTMPro);
                }
            }
        }
        public string _joiningDalvaTeam;
                
        [Tooltip("The list of all UI text for Hulryck's players")]
        [SerializeField]
        public List<TextMeshProUGUI> playerHulryckListTMPro;
        public List<string> playerHulryckList = new List<string>();
        [SerializeField]
        public string joiningHulryckTeam
        {
            get { return _joiningHulryckTeam; }
            set
            {
                _joiningHulryckTeam = value;
                if (_joiningHulryckTeam == joiningHulryckTeam)
                {
                    foreach (TextMeshProUGUI item in playerDalvaListTMPro)
                    {
                        if (joiningDalvaTeam != item.text) { }
                        else playerDalvaList.Remove(_joiningHulryckTeam);
                        UpdatePlayerList(playerDalvaList, playerDalvaListTMPro);
                    }
                    playerHulryckList.Add(_joiningHulryckTeam);
                    UpdatePlayerList(playerHulryckList, playerHulryckListTMPro);
                }
            }
        }
        public string _joiningHulryckTeam;

        [SerializeField]
        public bool inDalvaTeam;

        [SerializeField]
        public string joinTeamNickname
        {
            get { return _joinTeamNickname; }
            set
            {
                Debug.Log("J'ai bien changé");
                _joinTeamNickname = value;
                if (_joinTeamNickname == joinTeamNickname && joinTeamNickname != null)
                {
                    if (inDalvaTeam)
                    {
                        foreach (TextMeshProUGUI item in playerHulryckListTMPro)
                        {
                            if (joinTeamNickname != item.text) { }
                            else playerHulryckList.Remove(joinTeamNickname);
                            UpdatePlayerList(playerHulryckList, playerHulryckListTMPro);
                        }
                        playerDalvaList.Add(joinTeamNickname);
                        UpdatePlayerList(playerDalvaList, playerDalvaListTMPro);
                    }
                    else
                    {
                        foreach (TextMeshProUGUI item in playerDalvaListTMPro)
                        {
                            if (joinTeamNickname != item.text) { }
                            else playerDalvaList.Remove(joinTeamNickname);
                            UpdatePlayerList(playerDalvaList, playerDalvaListTMPro);
                        }
                        playerHulryckList.Add(joinTeamNickname);
                        UpdatePlayerList(playerHulryckList, playerHulryckListTMPro);
                    }
                }
            }
        }
        public string _joinTeamNickname;

        [Tooltip("The button to start a game (master only)")]
        [SerializeField]
        public Button startGameButton;

        [Tooltip("The button to join Dalva's team")]
        [SerializeField]
        public Button joinDalvaButton;

        [Tooltip("The button to join Huleryck's team")]
        [SerializeField]
        public Button joinHulryckButton;

        [Tooltip("The TMPro text for room code")]
        [SerializeField]
        public TextMeshProUGUI roomCodeText;
        #endregion

        #region IPunObservable implementation


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                //stream.SendNext(_joiningDalvaTeam);
                //stream.SendNext(_joiningHulryckTeam);
                stream.SendNext(joinTeamNickname);
                stream.SendNext(inDalvaTeam);
            }
            else
            {
                // Network player, receive data
                //this.joiningDalvaTeam = (string)stream.ReceiveNext();
                //this.joiningHulryckTeam = (string)stream.ReceiveNext();
                this.joinTeamNickname = (string)stream.ReceiveNext();
                this.inDalvaTeam = (bool)stream.ReceiveNext();
            }
        }


        #endregion

        #region Callbacks

        private void Start()
        {
            UpdatePlayerList(playerList, playerListTMPro);

            if (PhotonNetwork.IsMasterClient) startGameButton.interactable = true;

            roomCodeText.text = "Room code : " + PhotonNetwork.CurrentRoom.Name;
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
                PhotonNetwork.LoadLevel("MapInPlaceHolder");
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("PNMainMenu");
        }

        public void JoinTeam(bool isDalvasButton)
        {
            //gameObject.GetComponent<PhotonView>().RPC("RPCUpdatePlayerList", RpcTarget.All, PhotonNetwork.NickName, isDalvasButton);
            
            if(isDalvasButton)
            {
                joinDalvaButton.interactable = false;
                joinHulryckButton.interactable = true;
                PlayerManager.localPlayerInstance.GetComponent<PlayerManager>().dalvasTeam = true;
                //joiningDalvaTeam = PhotonNetwork.NickName;
                //playerDalvaList.Add(PhotonNetwork.LocalPlayer.NickName);
                inDalvaTeam = true;
            }
            else
            {
                joinDalvaButton.interactable = true;
                joinHulryckButton.interactable = false;
                PlayerManager.localPlayerInstance.GetComponent<PlayerManager>().dalvasTeam = false;
                //joiningHulryckTeam = PhotonNetwork.NickName;
                //playerHulryckList.Add(PhotonNetwork.LocalPlayer.NickName);
                inDalvaTeam = false;
            }

            joinTeamNickname = PhotonNetwork.NickName;
        }

        [PunRPC]
        public void RPCUpdatePlayerList(string nickname, bool team)
        {
            if(team)
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

