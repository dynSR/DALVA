using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace GameNetwork
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        #region Fields

        [Tooltip("The list of all UI text for players' name")]
        [SerializeField]
        public List<TextMeshProUGUI> playerListTMPro;
        public List<string> playerList = new List<string>();

        #endregion

        #region Callbacks

        private void Start()
        {
            
        }

        public override void OnCreatedRoom()
        {
            //playerList.Add(PhotonNetwork.NickName);
            //GetComponent<PhotonView>().RPC("UpdateOtherNameList", RpcTarget.OthersBuffered, PhotonNetwork.NickName);
            UpdatePlayerList();
        }

        public override void OnJoinedRoom()
        {
            
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log(other.NickName + " entered room");
            
            if (PhotonNetwork.IsMasterClient)
            {
                GetComponent<PhotonView>().RPC("PUNUpdateOtherNameList", RpcTarget.All);
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log(other.NickName + " leaved room");

            if (PhotonNetwork.IsMasterClient)
            {
                GetComponent<PhotonView>().RPC("PUNUpdateOtherNameList", RpcTarget.All);
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

        #endregion
    }
}

