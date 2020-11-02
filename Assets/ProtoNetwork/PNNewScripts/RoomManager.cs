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
            playerList.Add(PhotonNetwork.NickName);
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
                playerList.Add(other.NickName);
                GetComponent<PhotonView>().RPC("UpdateOtherNameList", RpcTarget.Others, playerList);
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log(other.NickName + " leaved room");
            if (PhotonNetwork.IsMasterClient)
            {
                playerList.Remove(other.NickName);
                GetComponent<PhotonView>().RPC("UpdateOtherNameList", RpcTarget.Others, playerList);
            }
        }

        #endregion

        #region Methods

        [PunRPC]
        void UpdateOtherNameList(List<string> newList)
        {
            playerList = newList;
            UpdatePlayerList();
        }

        private void UpdatePlayerList()
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                if(playerListTMPro[i].text != playerList[i])
                {
                    playerListTMPro[i].text = playerList[i];
                }
            }
        }

        #endregion
    }
}

