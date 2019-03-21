using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class Connector : MonoBehaviourPunCallbacks
{

    public static bool connected_to_room = false;
    public static short is_host = -1;
    public static Room main_game_room;

    // connect to service
    void Start()
    {
        if (!PhotonNetwork.ConnectUsingSettings())
            Debug.LogError("PhotonNetwork.ConnectUsingSettings failed");
        else
        {
            Debug.Log("Connected");
        }
    }

    //  join a room
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinRoom("A3-Room");
    }

    //set player number, initialize game or set host
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
        Debug.Log("Room Joined, you are number " + PhotonNetwork.CurrentRoom.PlayerCount);
        Connector.connected_to_room = true;
        TestGameStart();
    }
    //create room
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Room not found. Creating Room");
        RoomOptions ro = new RoomOptions() { IsOpen = true, CleanupCacheOnLeave = true, MaxPlayers = 4 };
        PhotonNetwork.CreateRoom("A3-Room", ro);
    }
    //check for gamestart and update player count
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        TestGameStart();
    }

    void TestGameStart()
    {
        if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount && PhotonNetwork.IsMasterClient)
        {
            Player[] playerlist = PhotonNetwork.PlayerList;
            int player_no = 0;
            PhotonView caller = this.GetComponent<PhotonView>();
            foreach (Player player in playerlist)
            {
                caller.RPC("StartGame", player, ++player_no, PhotonNetwork.CurrentRoom.PlayerCount);
            }
            PhotonNetwork.SendAllOutgoingCommands();
        }
        else if(PhotonNetwork.IsMasterClient) { 
            GameObject.FindGameObjectWithTag("UIText").GetComponent<Text>().text = "Host Must Press Enter to Start " +
                "or Wait for all Players\n" +
                "[" + PhotonNetwork.CurrentRoom.PlayerCount + "/ 4 Players]";
        }
        else
        {
            GameObject.FindGameObjectWithTag("UIText").GetComponent<Text>().text = "Wait for host to start " +
    "or Wait for all Players\n" +
    "[" + PhotonNetwork.CurrentRoom.PlayerCount + "/ 4 Players]";
        }
    }

    public static void callGameStart()
    {
        Player[] playerlist = PhotonNetwork.PlayerList;
        int player_no = 0;
        PhotonView caller = GameObject.FindGameObjectWithTag("GameController").GetComponent<PhotonView>();
        foreach (Player player in playerlist)
        {
            caller.RPC("StartGame", player, ++player_no, PhotonNetwork.CurrentRoom.PlayerCount);
        }
        PhotonNetwork.SendAllOutgoingCommands();
    }

    //RPC call to start game setup
    [PunRPC]
    void StartGame(int player_no, byte all_players)
    {
        MapSetup.player_no = player_no;
        MapSetup.all_players = (int)all_players;
        MapSetup.player_text.GetComponent<Text>().text = "You are Player " + player_no;
        MapSetup.initialize();
    }

    void Update()
    {
        if (Connector.connected_to_room && (PhotonNetwork.IsMasterClient ? 1: 0) != Connector.is_host)
        {
            if (PhotonNetwork.IsMasterClient == true)
            {
                Debug.Log("you are host");

                MapSetup.player_text.GetComponent<Text>().text = "You are Host";
                Connector.is_host = 1;
                Connector.main_game_room = PhotonNetwork.CurrentRoom;

            }
            else
            {
                MapSetup.player_text.GetComponent<Text>().text = "You are Player " + PhotonNetwork.CurrentRoom.PlayerCount;
                Connector.is_host = 0;
                Connector.main_game_room = PhotonNetwork.CurrentRoom;
            }
        }
    }
}
