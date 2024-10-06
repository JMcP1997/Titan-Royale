using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Com.KaijuCrew.TitanRoyale
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        public static RoomManager Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;

            //Singleton setup for the room manager. There will only ever be one Room manager, so this is an appropriate Singleton use
        }

        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "LevelArena") //if we're in the game scene
            {
                SpawnPointHolder spawnLocations = FindObjectOfType<SpawnPointHolder>();
                PlayerManager playerManager = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
                playerManager.SetSpawnPoints(spawnLocations.GetSpawnPoints());
            }
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.LoadLevel("PregameLobby");
            PhotonNetwork.Destroy(this.gameObject);
        }

    }
}
