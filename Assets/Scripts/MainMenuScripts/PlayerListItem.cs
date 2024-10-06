using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


namespace Com.KaijuCrew.TitanRoyale
{
    public class PlayerListItem : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_Text playerNameText;
        Player player;

        [SerializeField] Image backgroundImage;
        [SerializeField] Color highlightColor;

        [SerializeField] GameObject leftArrowButton;
        [SerializeField] GameObject rightArrowButton;

        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
        [SerializeField] Image playerAvatar;
        [SerializeField] Sprite[] avatars;

        [SerializeField] AudioSource listItemAudioSource;
        [SerializeField] AudioClip characterSelectSound;


        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
            listItemAudioSource.clip = characterSelectSound;
        }

        public void SetUp(Player player)
        {
            playerNameText.text = player.NickName;
            this.player = player;
            UpdatePlayerItem(player);
        }

        public void ApplyLocalChanges()
        {
            backgroundImage.color = highlightColor;
            leftArrowButton.SetActive(true);
            rightArrowButton.SetActive(true);
        }

        public void OnClickLeftArrow()
        {
            if ((int)playerProperties["playerAvatar"] == 0)
            {
                playerProperties["playerAvatar"] = avatars.Length - 1;
            }
            else
            {
                playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] - 1;               
            }
            PhotonNetwork.SetPlayerCustomProperties(playerProperties);
            listItemAudioSource.Play();
        }

        public void OnClickRightArrow()
        {
            if ((int)playerProperties["playerAvatar"] == avatars.Length - 1)
            {
                playerProperties["playerAvatar"] = 0;
            }
            else
            {
                playerProperties["playerAvatar"] = (int)playerProperties["playerAvatar"] + 1;
            }
            PhotonNetwork.SetPlayerCustomProperties(playerProperties);
            listItemAudioSource.Play();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            if(player == targetPlayer)
            {
                UpdatePlayerItem(targetPlayer);
            }
        }

        void UpdatePlayerItem(Player player)
        {
            if(player.CustomProperties.ContainsKey("playerAvatar"))
            {
                playerAvatar.sprite = avatars[(int)player.CustomProperties["playerAvatar"]];
                playerProperties["playerAvatar"] = (int)player.CustomProperties["playerAvatar"];
            }
            else
            {
                if(PhotonNetwork.LocalPlayer == player)
                {
                    playerProperties["playerAvatar"] = 0;
                }
                Debug.Log("Player Property set to 0: " + gameObject.name);
                PhotonNetwork.SetPlayerCustomProperties(playerProperties);
            }
        }
    }
}
