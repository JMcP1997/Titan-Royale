using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    [SerializeField] GameObject playerUIPrefab;

    [SerializeField] GameObject[] playerPrefabs;


    [SerializeField] Transform[] spawnPoints;

    private PlayerController targetPlayerController;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void SetSpawnPoints(Transform[] spawnPoints)
    {
        this.spawnPoints = spawnPoints;
    }


    private void Start()
    {
        if(PV.IsMine)
        {
            CreateController();
            //Only run the process that creates a controllable player avatar if youre the local player
        }
    }

    private void CreateController()
    {
        GameObject playerToSpawn = playerPrefabs[(int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"]];
        GameObject localPlayer = PhotonNetwork.Instantiate(Path.Combine("RefactorMonsterPrefabs", playerToSpawn.name), spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber].position, Quaternion.identity);
        //Choose which monster to spawn based of the preferences selected in the lobby

        PlayerController playerController = localPlayer.GetComponent<PlayerController>();
        targetPlayerController = playerController;
        playerController.UpdatePlayerNameText(PhotonNetwork.LocalPlayer.NickName);
        //Set the player controller of the local player and update their nameplate

        PlayerUI playerUI = Instantiate(playerUIPrefab, localPlayer.transform.position, Quaternion.identity).GetComponent<PlayerUI>();
        playerUI.SetTarget(localPlayer.GetComponent<PlayerController>());
        playerUI.SetButtonFunction(GameOver);
        //Create a UI for the player and store a reference to it
        //Then run the appropriate functions on the UI for the start of a new game

    }

    private void GameOver()
    {
        PhotonNetwork.LeaveRoom();
    }




}
