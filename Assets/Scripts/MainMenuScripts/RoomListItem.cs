using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;

    LobbyManager manager = LobbyManager.Instance;

    public void SetUp(string roomName)
    {
        this.roomName.text = roomName;
    }

    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}
