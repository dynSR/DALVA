using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using Photon.Pun;
using Photon.Realtime;

namespace GameNetwork
{
    public class MainMenuNetworkManager : MonoBehaviourPunCallbacks
    {

        #region Fields 
        ////Serializable Fields
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players")]
        [SerializeField]
        private byte maxPlayersPerRoom = 6;
        
        //Start Panel
        [Tooltip("The Start UI panel")]
        [SerializeField]
        private GameObject startPanel;

        //Main Panel
        [Tooltip("The Main UI panel")]
        [SerializeField]
        private GameObject mainMenuPanel;
        [Tooltip("The InputField for the room's code")]
        [SerializeField]
        private GameObject roomCodeInputField;

        //Infos Panel
        [Tooltip("The text to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject connectingText;
        [Tooltip("The text to inform that there is no room to join")]
        [SerializeField]
        private GameObject failedJoinRandomRoomText;
        [Tooltip("The text to inform that there is no room with this code or the room is full")]
        [SerializeField]
        private GameObject failedJoinRoomText;
        [Tooltip("The text to inform that the nickname is null or empty")]
        [SerializeField]
        private GameObject wrongNicknameText;

        ////Fields
        private bool isConnecting;
        private float timer;
        private GameObject currentInfo;
        #endregion

        #region Callbacks
        ////Callbacks

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            //UI setActive
            startPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
            connectingText.SetActive(false);
            failedJoinRoomText.SetActive(false);
            failedJoinRandomRoomText.SetActive(false);
            wrongNicknameText.SetActive(false);

            timer = 1f;
            currentInfo = connectingText;
            
            if (PhotonNetwork.IsConnected)
            {
                startPanel.SetActive(false);
                mainMenuPanel.SetActive(true);
            }
        }

        private void Update()
        {
            InfoPanelUpdate(currentInfo);
        }

        ////Network Callbacks

        public override void OnConnectedToMaster()
        {
            mainMenuPanel.SetActive(true);
            connectingText.SetActive(false);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            startPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel("PNWaitingRoom");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            failedJoinRoomText.SetActive(true);
            currentInfo = failedJoinRoomText;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            failedJoinRandomRoomText.SetActive(true);
            currentInfo = failedJoinRandomRoomText;
        }

        #endregion

        #region Methods
        ////Methods

        //StartPanel.PlayButton
        public void PlayButton()
        {
            if (PhotonNetwork.IsConnected)
            {
                startPanel.SetActive(false);
                mainMenuPanel.SetActive(true);
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                currentInfo = connectingText;
                connectingText.SetActive(true);
                startPanel.SetActive(false);
            }
        }

        //MainPanel.HostButton
        public void CreateRoom()
        {
            if (PhotonNetwork.IsConnected)
            {
                int randomRoomName = Random.Range(1000, 10000);
                PhotonNetwork.CreateRoom(randomRoomName.ToString(), new RoomOptions { MaxPlayers = maxPlayersPerRoom });
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
            }
        }
        
        //MainPanel.RandomJoinButton
        public void ConnectRandom()
        {
            
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
            }
        }

        //MainPanel.JoinButton
        public void ConnectRoomName()
        {
            if (string.IsNullOrEmpty(roomCodeInputField.GetComponent<TMP_InputField>().text))
            {
                failedJoinRoomText.SetActive(true);
                currentInfo = failedJoinRoomText;
                return;
            }

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRoom(roomCodeInputField.GetComponent<TMP_InputField>().text, null);
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
            }
        }

        //MainPanel.BackButton
        public void BackToStartMenu()
        {
            mainMenuPanel.SetActive(false);
            startPanel.SetActive(true);
        }

        //Info message
        private void InfoPanelUpdate(GameObject info)
        {
            if (info.activeSelf)
            {
                if (info.name == connectingText.name)
                {
                    if (startPanel.activeSelf) info.SetActive(false);
                    return;
                }

                mainMenuPanel.SetActive(false);
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    info.SetActive(false);
                    mainMenuPanel.SetActive(true);
                    timer = 1f;
                }
            }
        }
        #endregion

    }
}

