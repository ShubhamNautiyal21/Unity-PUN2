using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class RPCMainClass : MonoBehaviourPun
{
    public PhotonView PV;
    public static RPCMainClass rPCMainClass;
    private void Start()
    {
        rPCMainClass = this;
        PV = GetComponent<PhotonView>();
       
    }
    public void GameOver()
    {
       
        object[] objectArray = {"GameWon", "true" };
        // PV.RPC("Name of the RPC Function", (RpcTarget.Others , MasterClient, All), data/object to share);
        PV.RPC("GameOver", RpcTarget.OthersBuffered, objectArray);
    }
    public void SendMatrixToOpponent(int[,] listToShare,Values val)
    {
          
          string localS = JsonUtility.ToJson(val);
          object[] objectArray = { localS.Length, localS };
         PV.RPC("RPCSendingOpponentGameMatrix", RpcTarget.OthersBuffered, objectArray);
        //PV.RPC("RPCSendingOpponentGameMatrix", RpcTarget.OthersBuffered, localS);

    }
    public void SendingOtherDetails(int i,int j)
    {
        
        GameController.gameController.ActivateOpponentTurn();
        object[] objectArray = { "TileClicked", i,j };

        Debug.LogError("player clicked " + i + j);
        PV.RPC("RPCTileClicked", RpcTarget.OthersBuffered, objectArray);
    } 
    public void GameStart()
    {
        
            object[] objectArray = { "StartGame", "true" };
            PV.RPC("GameShouldStart", RpcTarget.OthersBuffered, objectArray);
        
    }
    [PunRPC]
    public void RPCTileClicked(object[] obj)
    {
        
        int i = int.Parse(obj[1].ToString());
        int j = int.Parse(obj[2].ToString());

        Debug.LogError("Opponent clicked "+i+j);
        if (i != -1)
        {
            GameController.gameController.SettingOpponentImages(i, j);
        }
        GameController.gameController.SwapTurns();
    }
    [PunRPC]
    public void RPCSendingOpponentGameMatrix(object[] obj)
    {

        GameController.gameController.SettingGameGrid(obj[1].ToString());
       

    }
    [PunRPC]
    public void GameShouldStart(object[] obj)
    {

        if (obj[0].ToString().Equals("StartGame"))
        {
            GameController.gameController.gamePanel2.SetActive(false);
            GameController.gameController.mainGamePanel.SetActive(true);
        }
    }
    [PunRPC]
    public void GameOver(object[] obj)
    {
        SoundAndMusic.soundAndMusic.PlaySoundClip("win");
        GameController.gameController.gameWon.SetActive(true);
        
    }
}
