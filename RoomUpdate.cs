using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUpdate : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.LogError("Entered Room..." + newPlayer);
        throw new System.NotImplementedException();
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.LogError("Entered Room..." + otherPlayer);
        throw new System.NotImplementedException();
    }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        throw new System.NotImplementedException();
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        throw new System.NotImplementedException();
    }

  
}
