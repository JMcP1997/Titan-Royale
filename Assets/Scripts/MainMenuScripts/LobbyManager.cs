using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Com.KaijuCrew.TitanRoyale;
using Unity.VisualScripting;

public class LobbyManager : MonoBehaviourPunCallbacks
{


    [SerializeField] TMP_InputField roomInputField;

    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject roomPanel;

    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text lobbyPanelErrorText;

    [SerializeField] RoomListItem roomItemPrefab;
    List<RoomListItem> roomItemList = new List<RoomListItem>();
    [SerializeField] Transform roomListItemContainer;

    [SerializeField] float timeBetweenUpdates = 1.5f;
    private float nextUpdateTime;

    List<PlayerListItem> playerItemList = new List<PlayerListItem>();
    [SerializeField] PlayerListItem playerListItemPrefab;
    [SerializeField] Transform playerListItemContainer;

    [SerializeField] GameObject playButton;
    [SerializeField] int minimumPlayerCount = 2;

    public static LobbyManager Instance { get; private set; }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= minimumPlayerCount)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton?.SetActive(false);
        }
    }

    private void Awake()
    {
        if(Instance!=null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

    }

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinLobby();
        }

    }

    public void OnClickCreate()
    {
        if(roomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 12, BroadcastPropsChangeToAll = true }) ;
            lobbyPanelErrorText.text = "";
        }
        else
        {
            lobbyPanelErrorText.text = "Please Enter a Name for your Room";
        }
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        roomName.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach(RoomListItem item in roomItemList)
        {
            Destroy(item.gameObject);
        }
        roomItemList.Clear();

        foreach(RoomInfo room in list)
        {
            if (room.RemovedFromList)
                continue;
            if(room.IsOpen)
            {
                RoomListItem newRoom = Instantiate(roomItemPrefab, roomListItemContainer);
                newRoom.SetUp(room.Name);
                roomItemList.Add(newRoom);
            }
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    void UpdatePlayerList()
    {
        foreach(PlayerListItem item in playerItemList)
        {
            Destroy(item.gameObject);
        }
        playerItemList.Clear();

        if(PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerListItem newPlayerItem = Instantiate(playerListItemPrefab, playerListItemContainer);
            newPlayerItem.SetUp(player.Value);

            if(player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }
            playerItemList.Add(newPlayerItem);
        }
    }

    public void OnClickPlayButton()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("LevelArena");
    }

}
