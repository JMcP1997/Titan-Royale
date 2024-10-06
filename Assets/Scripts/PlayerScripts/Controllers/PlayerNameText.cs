using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNameText : MonoBehaviour
{
    [SerializeField] PlayerController controller;

    PhotonView PV;

    [SerializeField] TextMeshPro playerNameText;
    [SerializeField] Vector3[] textHeights;

    private bool textEnabled = true;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        if (PV.IsMine)
        {
            playerNameText.gameObject.SetActive(false);
            //Players don't need to see their own name plate
        }
    }

    private void Start()
    {
        controller.GetMonsterController().OnPlayerEvolved += MonsterController_OnPlayerEvolved;
    }

    private void Update()
    {
        if(!PV.IsMine)
        {
            gameObject.transform.LookAt(FindObjectOfType<Camera>().gameObject.transform);
            //Other player's nameplates should always face the camera being controlled by the local player
        }
    }

    private void MonsterController_OnPlayerEvolved()
    {
        AdjustTextHeight();
    }

    public void AdjustTextHeight()
    {
        PV.RPC("RPC_AdjustHeight", RpcTarget.All);
    }

    public void UpdatePlayerNameText(string playerName)
    {
        PV.RPC("RPC_UpdatePlayerNameText", RpcTarget.All, playerName);
    }

    [PunRPC]
    void RPC_UpdatePlayerNameText(string playerName)
    {
        playerNameText.text = playerName;
    }

    [PunRPC]
    void RPC_AdjustHeight()
    {
        gameObject.transform.position = textHeights[controller.GetCurrentEvolutionLevel()];
        //When players evolve, the height of their name text will need to be adjusted
        //The variable for the new height to match the new model is assignable in the inspector
    }

    public void HideNameText()
    {
        PV.RPC("RPC_HideNameText", RpcTarget.All);
    }

    public void ShowNameText()
    {
        PV.RPC("RPC_ShowNameText", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_HideNameText()
    {
        playerNameText.gameObject.SetActive(false);
    }

    [PunRPC]
    private void RPC_ShowNameText()
    {
        if(!PV.IsMine)
        {
            playerNameText.gameObject.SetActive(true);
        }
    }
}
