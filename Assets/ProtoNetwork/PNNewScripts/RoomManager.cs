using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Photon.Pun;
using Photon.Realtime;

namespace GameNetwork
{
    public class RoomManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Fields

        [Tooltip("The list of all UI text for players' name")]
        [SerializeField]
        public List<TextMeshProUGUI> playerList;

        #endregion

        #region Callbacks

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            foreach (TextMeshProUGUI item in playerList)
            {
                if (item.text == "")
                {
                    item.text = PhotonNetwork.NickName;
                    Debug.Log("OK");
                    return;
                }
                else
                {
                    Debug.Log("notOK");
                }
            }
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log("Entered room");

            foreach (TextMeshProUGUI item in playerList)
            {
                if (item.text == "")
                {
                    item.text = other.NickName;
                    Debug.Log("OK");
                    return;
                }
                else
                {
                    Debug.Log("notOK");
                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            foreach (TextMeshProUGUI item in playerList)
            {
                if (item.text == other.NickName)
                {
                    item.text = "";
                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                
            }
        }

        #endregion

        #region Observable callbacks

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(playerList);
            }
            else
            {
                // Network player, receive data
                this.playerList = (List<TextMeshProUGUI>)stream.ReceiveNext();
            }
        }

        #endregion
    }
}

