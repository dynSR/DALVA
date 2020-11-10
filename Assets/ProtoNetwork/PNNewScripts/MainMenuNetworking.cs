using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using Photon.Pun;
using Photon.Realtime;

namespace GameNetwork
{
    public class MainMenuNetworking : MonoBehaviourPunCallbacks
    {

        #region Fields 
        //Serializable Fields
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players")]
        [SerializeField]
        private byte maxPlayersPerRoom = 6;

        [Tooltip("The UI Panel to let the user enter his name")]
        [SerializeField]
        private GameObject nicknamePanel;

        [Tooltip("The UI menu to choose a way to play")]
        [SerializeField]
        private GameObject mainMenuWindow;
        [Tooltip("The UI menu to enter a room name")]
        [SerializeField]
        private GameObject joinWithRoomNameWindow;
        [Tooltip("The name of the room to join")]
        [SerializeField]
        private GameObject roomNameInputField;
        [Tooltip("The UI Panel of the room")]
        [SerializeField]
        private GameObject roomPanel;

        [Tooltip("The text to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject connectingText;
        [Tooltip("The text to inform that there is no room with this code or the room is full")]
        [SerializeField]
        private GameObject failedJoinRoomText;
        [Tooltip("The text to inform that there is no room to join")]
        [SerializeField]
        private GameObject failedJoinRandomRoomText;

        //Fields
        private bool isConnecting;
        private float timer;
        #endregion

        #region Callbacks
        //Callbacks

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            nicknamePanel.SetActive(true);
            mainMenuWindow.SetActive(false);
            joinWithRoomNameWindow.SetActive(false);
            connectingText.SetActive(false);
            failedJoinRoomText.SetActive(false);
            failedJoinRandomRoomText.SetActive(false);
            roomPanel.SetActive(false);

            timer = 1f;
        }

        private void Update()
        {
            if (failedJoinRandomRoomText.activeSelf)
            {
                if(timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    timer = 1f;
                    failedJoinRandomRoomText.SetActive(false);
                }
            }

            if (failedJoinRoomText.activeSelf)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    timer = 1f;
                    failedJoinRoomText.SetActive(false);
                }
            }
        }

        //Network Callbacks

        public override void OnConnectedToMaster()
        {
            mainMenuWindow.SetActive(true);
            connectingText.SetActive(false);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnJoinedRoom()
        {
            //PhotonNetwork.LoadLevel("PNWaitingRoom");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            joinWithRoomNameWindow.SetActive(true);
            failedJoinRoomText.SetActive(true);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            mainMenuWindow.SetActive(true);
            failedJoinRandomRoomText.SetActive(true);
        }

        #endregion

        #region Methods
        //Methods
        public void ValidateName() //"Play" button when entering name
        {
            if (PhotonNetwork.IsConnected)
            {
                nicknamePanel.SetActive(false);
                mainMenuWindow.SetActive(true);
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                connectingText.SetActive(true);
                nicknamePanel.SetActive(false);
            }
        }

        public void ChangeName() //"Change username" button
        {
            nicknamePanel.SetActive(true);
            mainMenuWindow.SetActive(false);
        }

        public void CreateRoom() //"Create a room" button
        {
            if (PhotonNetwork.IsConnected)
            {
                mainMenuWindow.SetActive(false);
                roomPanel.SetActive(true);
                int randomRoomName = Random.Range(1000, 10000);
                PhotonNetwork.CreateRoom(randomRoomName.ToString(), new RoomOptions { MaxPlayers = maxPlayersPerRoom });
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void ToggleConnectRoomNameUI()
        {
            mainMenuWindow.SetActive(!mainMenuWindow.activeSelf);
            joinWithRoomNameWindow.SetActive(!joinWithRoomNameWindow.activeSelf);
        }

        public void ConnectRoomName()
        {
            if (string.IsNullOrEmpty(roomNameInputField.GetComponent<TMP_InputField>().text))
            {
                Debug.LogError("Room name null or empty");
                return;
            }

            if (PhotonNetwork.IsConnected)
            {
                joinWithRoomNameWindow.SetActive(false);
                //roomPanel.SetActive(true);
                PhotonNetwork.JoinRoom(roomNameInputField.GetComponent<TMP_InputField>().text, null);
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void ConnectRandom() //"Join a random room" button
        {
            
            if (PhotonNetwork.IsConnected)
            {
                mainMenuWindow.SetActive(false);
                //roomPanel.SetActive(true);
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void LeaveRoom()
        {
            //roomPanel.SetActive(false);
            mainMenuWindow.SetActive(true);
            PhotonNetwork.LeaveRoom();
        }
        #endregion

    }
}

