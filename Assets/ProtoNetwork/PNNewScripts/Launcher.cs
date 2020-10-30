using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using Photon.Pun;
using Photon.Realtime;

namespace GameNetwork
{
    public class Launcher : MonoBehaviourPunCallbacks
    {

        #region Fields 
        //Serializable Fields
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players")]
        [SerializeField]
        private byte maxPlayersPerRoom = 6;

        [Tooltip("The Ui Panel to let the user enter name")]
        [SerializeField]
        private GameObject namePanel;
        [Tooltip("The UI menu to choose a way to play")]
        [SerializeField]
        private GameObject createOrJoinWindow;
        [Tooltip("The UI menu to enter a room name")]
        [SerializeField]
        private GameObject joinWindow;
        [SerializeField]
        private GameObject roomNameInputField;

        //Fields
        private bool isConnecting;
        #endregion

        #region Callbacks
        //Callbacks

        void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            namePanel.SetActive(true);
            createOrJoinWindow.SetActive(false);
            joinWindow.SetActive(false);
        }

        //Network Callbacks

        public override void OnJoinedRoom()
        {
            Debug.Log("Room name is: " + PhotonNetwork.CurrentRoom.Name);
        }

        public override void OnConnectedToMaster()
        {

        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            joinWindow.SetActive(true);
            Debug.Log("There is no room to join");
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            createOrJoinWindow.SetActive(true);
            Debug.Log("There is no room to join");
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);
        }
        #endregion

        #region Methods
        //Methods
        public void ValidateName() //"Play" button when entering name
        {
            namePanel.SetActive(false);
            createOrJoinWindow.SetActive(true);
            if (PhotonNetwork.IsConnected)
            {
                return;
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
            }
        }


        public void CreateRoom() //"Create a room" button
        {
            createOrJoinWindow.SetActive(false);
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

        public void ActiveConnectRoomNameUI()
        {
            createOrJoinWindow.SetActive(false);
            joinWindow.SetActive(true);
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
                joinWindow.SetActive(false);
                PhotonNetwork.JoinRoom(roomNameInputField.GetComponent<TMP_InputField>().text, null);
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void ConnectRandom() //"Join a random room" button
        {
            createOrJoinWindow.SetActive(false);
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
            }
        }
        #endregion

    }
}

