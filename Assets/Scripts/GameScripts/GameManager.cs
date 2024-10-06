using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

public class GameManager: MonoBehaviour, IOnEventCallback
{

    public static GameManager Instance { get; private set; }

    public bool testing;

    Dictionary<Player, bool> players = new Dictionary<Player, bool>();

    private void Start()
    {
        if((Instance!= null))
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            players.Add(player, false);
        }
    }

    private void Update()
    {
        if(testing && Input.GetKeyDown(KeyCode.P))
        {
            foreach(Player player in players.Keys)
            {
                Debug.Log(player.ToString() + players[player].ToString());
            }
        }
    }

    public bool CheckPlayerVictory()
    {
        if(testing)
        {
            return false;
        }
        int numberOfDeadPlayers = 0;
        foreach(Player player in players.Keys)
        {
            if (players[player])
            {
                numberOfDeadPlayers++;
            }
        }

        return numberOfDeadPlayers >= players.Count - 1;

    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {

        if(photonEvent.Code == EventCodes.PlayerDiedEventCode)
        {
            int receivedId = (int)photonEvent.CustomData;
            PhotonView PV = PhotonView.Find(receivedId);

            foreach(Player player in players.Keys)
            {
                if(PV.Owner == player)
                {
                    players[player] = true;
                    break;
                }
            }
        }
    }
}