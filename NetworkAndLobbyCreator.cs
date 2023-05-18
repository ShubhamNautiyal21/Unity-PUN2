using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class NetworkAndLobbyCreator : MonoBehaviourPunCallbacks, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks, ILobbyCallbacks
{
    #region Public Variables

    public GameObject Loading;
    public Text rName;

    public bool waitForOthersToJoin;

    public List<RoomInfo> roomList;

    public Button playbutton;
    #endregion

    #region Local Varibales
    float waitForOthersToJoinTime;
    #endregion

    #region Photon Callbacks


    string myGameType;
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("Connection made to " + PhotonNetwork.CloudRegion + " server and ACN=" + PhotonNetwork.LocalPlayer.ActorNumber.ToString());

        PhotonNetwork.JoinLobby();
        Loading.SetActive(false);

    }

    public void InputFromUser()
    {
        PhotonNetwork.JoinRoom(toEnterRoomName.text.ToString());
    
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room... trying again");
        JoinRandomRooms(myGameType);


    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var item in roomList)
        {
            Debug.Log("this is room check this " + item.Name);

        }
     
        this.roomList = roomList;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError("Disconnected...");
    }

    public override void OnJoinedRoom()
    {
        Debug.LogError("Joined Room...");
       
    }

    public override void OnLeftRoom()
    {
        if (GameController.gameController != null)
        {
            GameController.gameController.GameLost();
        }
        Debug.LogError("left" + MainData.userType);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("check this" + returnCode);
        Debug.LogError("check this" + message);
        foreach (var item in roomList)
        {
            Debug.LogError(item.Name);
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(GameController.gameController!=null)
            GameController.gameController.GameWin();
    }
    void OnApplicationQuit()
    {
        PhotonNetwork.LeaveRoom();
        if (GameController.gameController != null)
        {
            GameController.gameController.GameLost();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //StartGame();
        StartGame();

    }
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    #endregion

    #region MonoBehaviour Functionalities

    public static NetworkAndLobbyCreator networkAndLobbyCreator;
    
    private void Awake()
    {
        
            PhotonNetwork.AutomaticallySyncScene = true;
            if (networkAndLobbyCreator == null)
                networkAndLobbyCreator = this;
            else
                Destroy(this.gameObject);


           // DontDestroyOnLoad(this.gameObject);
            PhotonNetwork.ConnectUsingSettings();
        
    }
   
    private void Update()
    {
        if (waitForOthersToJoin)
        {
            waitForOthersToJoinTime -= Time.deltaTime;



            if (waitForOthersToJoinTime <= 0)
            {
                waitForOthersToJoin = false;
                CouldNotJoinRoom();
            }
        }

    }
    public GameObject countNotJoinRoom;
    public void CouldNotJoinRoom()
    {
        PhotonNetwork.LeaveRoom();
        waitToStartTheGame.SetActive(false);
        countNotJoinRoom.SetActive(true);
    }
   
    public void CreateNewRoom(string myGameType)
    {
        MainData.userType = "Owner";
        int randomRoomNumber = Random.Range(1, 10000);
        RoomOptions roomOps = new RoomOptions()
        {
            IsVisible = true,// users can see it, can be used to set private and public
            IsOpen = true, // people can see it, but cannot join if it is false
            MaxPlayers = (byte)2,
            CleanupCacheOnLeave=true// clear the cache automatically when a player leaves
        };
        PhotonNetwork.CreateRoom(randomRoomNumber.ToString()+ myGameType, roomOps);


    }
    string roomName;
    public GameObject shareRoomId;
    public Text roomID;
    public override void OnCreatedRoom()
    {
        Debug.LogError("created");
        //ShareRoomId(roomName);
    }
    public void ShareRoomId(string randomRoomNumber)
    {
        waitToStartTheGame.SetActive(false);
        shareRoomId.SetActive(true);
        roomID.text = randomRoomNumber;


    }


    public void RemoveRoom()
    {

        PhotonNetwork.LeaveRoom();
    }
    public void CRoom(string myGameType)
    {

      

        CreateNewRoom(myGameType);




    }
  
    private void StartGame()
    {
       

        if (PhotonNetwork.IsMasterClient)
        {
          
             StartCoroutine(WaitToGameStart());
          
        }
    }
    public GameObject startingGame;
    public IEnumerator WaitToGameStart()
    {

        startingGame.SetActive(true);
        yield return new WaitForSeconds(3);
        if(myGameType=="easy")
            PhotonNetwork.LoadLevel("GameScene");
        else if (myGameType == "medium")
            PhotonNetwork.LoadLevel("GameSceneMedium");
        Debug.LogError("Game shall start:::");
    }
    public void JRoom(string rName)
    {
        Debug.LogError(rName);
        string room = rName;
        Debug.LogError(room);
        MainData.userType = "Other";


        //  PhotonNetwork.JoinRoom(room);
        PhotonNetwork.JoinRoom(room);

        WaitingToStartGame();
        
    }
    public void JRoom(TextMeshProUGUI rName)
    {
        string room = rName.text;
        Debug.LogError(room);
        MainData.userType = "Other";


        //  PhotonNetwork.JoinRoom(room);
        PhotonNetwork.JoinRoom(room);

        WaitingToStartGame();
    }
    public GameObject waitToStartTheGame;
    public void WaitingToStartGame()
    {
        waitForOthersToJoinTime = 35;
        waitToStartTheGame.SetActive(true);
        waitForOthersToJoin = true;

    }
    public void JoinRandomRooms(string gameType)
    {
        myGameType = gameType;
        if (gameType == "easy")
        {
            MainData.rowValue = 3;
            MainData.columnValue = 6;
        }
        else if (gameType == "medium")
        {
         
                MainData.rowValue = 6;
                MainData.columnValue = 6;
            
        }
        WaitingToStartGame();
        StartCoroutine(WaitingToCheckForRooms());
    }
    bool gameFound;
    int checkedTimes;
    IEnumerator WaitingToCheckForRooms()
    {
        if (roomList.Count > 0)

        {
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].Name.Contains(myGameType))
                {
                    startingGame.SetActive(true);
                    MainData.userType = "Other";
                    PhotonNetwork.JoinRoom(roomList[i].Name);
                    gameFound = true;
                    break;


                }

            }


        }
        if (gameFound)
        {
            yield return new WaitForSeconds(0);
        }
        else if (!gameFound && checkedTimes < 4)
        {
            ++checkedTimes;
            yield return new WaitForSeconds(1);
            StartCoroutine(WaitingToCheckForRooms());
        }

        else
        {
            CRoom(myGameType);
            yield return new WaitForSeconds(0);
        }

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        JoinRandomRooms(myGameType);

    }
   
    #endregion
}
