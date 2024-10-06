using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviourPunCallbacks
{

    [SerializeField] TMP_InputField usernameInput;

    [SerializeField] TMP_Text buttonText;
    [SerializeField] TMP_Text errorText;

    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Connecting...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            errorText.text = "Please Enter a Nickname";
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("PreGameLobby");
    }
}
