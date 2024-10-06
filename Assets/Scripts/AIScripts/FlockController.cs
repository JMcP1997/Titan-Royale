using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class FlockController : MonoBehaviour
{

    [Tooltip("Check this if this controller should spawn slimes NPCs")][SerializeField] bool slimeController = false;
    [Tooltip("Check this if this controller should spawn orc NPCs")] [SerializeField] bool orcController = false;
    [Tooltip("Check this if this controller should spawn golem NPCs")] [SerializeField] bool golemController = false;


    [Tooltip("Set the minimum amount of small-sized NPCs that should spawn")] [SerializeField] float smallNPCMinimumSpawn;
    [Tooltip("Set the maximum amount of small-sized NPCs that should spawn")] [SerializeField] float smallNPCMaximumSpawn;

    [Tooltip("Set the minimum amount of medium-sized NPCs that should spawn")] [SerializeField] float mediumNPCMinimumSpawn;
    [Tooltip("Set the maximum amount of medium-sized NPCs that should spawn")] [SerializeField] float mediumNPCMaximumSpawn;


    [Tooltip("Set the minimum amount of big-sized NPCs that should spawn")] [SerializeField] float bigNPCMinimumSpawn;
    [Tooltip("Set the maximum amount of big-sized NPCs that should spawn")] [SerializeField] float bigNPCMaximumSpawn;

    // Declare float to eventually assign a flock size for the small NPCs
    private float smallFlockSize;

    // Declare float to eventually assign a flock size for the medium NPCs
    private float mediumFlockSize;

    // Declare float to eventually assign a flock size for the big NPCs
    private float bigFlockSize;


    private List<EnemySimpleFSM> npcList = new List<EnemySimpleFSM>();

    private List<PlayerController> playerControllerList = new List<PlayerController>();

    private List<Transform> playerTransformList = new List<Transform>();

    public bool unused;
    //This is just so we can check which flock controllers did not end up being used at run time

    //[SerializeField] float timeBetweenRespawnChecks = 30f;
    private float timeSinceLastSpawn;

    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPC_SetFlockSize", RpcTarget.AllBuffered);
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
        /*
        if(Time.time - timeSinceLastSpawn >= timeBetweenRespawnChecks)
        {
            CheckRespawn();
        }
        */
    }

    public void GeneralNPCInstantiation()
    {
        // Calls the slime instantiation code
        SlimeInstantiation();

        // Calls the orc instantiation code
        OrcInstantiation();

        // Calls the golem instantiation code
        GolemInstantiation();

    }



    private void SlimeInstantiation()
    {
        if (slimeController)
        {

            for (int i = 0; i <= smallFlockSize; i++)
            {
                EnemySimpleFSM newBlueSlimeNPC = PhotonNetwork.InstantiateRoomObject(Path.Combine("NPCEnemyPrefabs", "BlueSlimeNPC"), transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), transform.rotation)
                .GetComponent<EnemySimpleFSM>();

                
                newBlueSlimeNPC.SetFlockController(PV.ViewID);

                PV.RPC("RPC_UpdateNPCMonsterList", RpcTarget.AllBuffered, newBlueSlimeNPC.gameObject.GetComponent<PhotonView>().ViewID);
            }


            for (int i = 0; i <= mediumFlockSize; i++)
            {
                EnemySimpleFSM newGreenSlimeNPC = PhotonNetwork.InstantiateRoomObject(Path.Combine("NPCEnemyPrefabs", "GreenSlimeNPC"), transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), transform.rotation)
                .GetComponent<EnemySimpleFSM>();

                newGreenSlimeNPC.SetFlockController(PV.ViewID);

                PV.RPC("RPC_UpdateNPCMonsterList", RpcTarget.AllBuffered, newGreenSlimeNPC.gameObject.GetComponent<PhotonView>().ViewID);
            }

            for (int i = 0; i <= bigFlockSize; i++)
            {

                EnemySimpleFSM newRedSlimeNPC = PhotonNetwork.InstantiateRoomObject(Path.Combine("NPCEnemyPrefabs", "RedSlimeNPC"), transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), transform.rotation)
               .GetComponent<EnemySimpleFSM>();

                newRedSlimeNPC.SetFlockController(PV.ViewID);

                PV.RPC("RPC_UpdateNPCMonsterList", RpcTarget.AllBuffered, newRedSlimeNPC.gameObject.GetComponent<PhotonView>().ViewID);
            }

        }

    }


    private void OrcInstantiation()
    {
        if (orcController)
        {

            for (int i = 0; i <= smallFlockSize; i++)
            {

                EnemySimpleFSM newSmallOrcNPC = PhotonNetwork.InstantiateRoomObject(Path.Combine("NPCEnemyPrefabs", "SmallOrcNPC"), transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30)), transform.rotation)
                .GetComponent<EnemySimpleFSM>();

                newSmallOrcNPC.SetFlockController(PV.ViewID);

                PV.RPC("RPC_UpdateNPCMonsterList", RpcTarget.AllBuffered, newSmallOrcNPC.gameObject.GetComponent<PhotonView>().ViewID);

            }


            for (int i = 0; i <= mediumFlockSize; i++)
            {

                EnemySimpleFSM newMediumOrcNPC = PhotonNetwork.InstantiateRoomObject(Path.Combine("NPCEnemyPrefabs", "MediumOrcNPC"), transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30)), transform.rotation)
                .GetComponent<EnemySimpleFSM>();

                newMediumOrcNPC.SetFlockController(PV.ViewID);

                PV.RPC("RPC_UpdateNPCMonsterList", RpcTarget.AllBuffered, newMediumOrcNPC.gameObject.GetComponent<PhotonView>().ViewID);

            }


            for (int i = 0; i <= bigFlockSize; i++)
            {

                EnemySimpleFSM newBigOrcNPC = PhotonNetwork.InstantiateRoomObject(Path.Combine("NPCEnemyPrefabs", "BigOrcNPC"), transform.position + new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30)), transform.rotation)
                .GetComponent<EnemySimpleFSM>();

                newBigOrcNPC.SetFlockController(PV.ViewID);

                PV.RPC("RPC_UpdateNPCMonsterList", RpcTarget.AllBuffered, newBigOrcNPC.gameObject.GetComponent<PhotonView>().ViewID);

            }


        }


    }


    private void GolemInstantiation()
    {
        if (golemController)
        {

            for (int i = 0; i <= smallFlockSize; i++)
            {

                EnemySimpleFSM newSmallGolemNPC = PhotonNetwork.InstantiateRoomObject(Path.Combine("NPCEnemyPrefabs", "SmallGolemNPC"), transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), transform.rotation)
                .GetComponent<EnemySimpleFSM>();

                newSmallGolemNPC.SetFlockController(PV.ViewID);

                PV.RPC("RPC_UpdateNPCMonsterList", RpcTarget.AllBuffered, newSmallGolemNPC.gameObject.GetComponent<PhotonView>().ViewID);

            }


            for (int i = 0; i <= mediumFlockSize; i++)
            {

                EnemySimpleFSM newMediumGolemNPC = PhotonNetwork.InstantiateRoomObject(Path.Combine("NPCEnemyPrefabs", "MediumGolemNPC"), transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), transform.rotation)
                .GetComponent<EnemySimpleFSM>();

                newMediumGolemNPC.SetFlockController(PV.ViewID);

                PV.RPC("RPC_UpdateNPCMonsterList", RpcTarget.AllBuffered, newMediumGolemNPC.gameObject.GetComponent<PhotonView>().ViewID);

            }

            for (int i = 0; i <= bigFlockSize; i++)
            {

                EnemySimpleFSM newBigGolemNPC = PhotonNetwork.InstantiateRoomObject(Path.Combine("NPCEnemyPrefabs", "BigGolemNPC"), transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10)), transform.rotation)
                .GetComponent<EnemySimpleFSM>();

                newBigGolemNPC.SetFlockController(PV.ViewID);

                PV.RPC("RPC_UpdateNPCMonsterList", RpcTarget.AllBuffered, newBigGolemNPC.gameObject.GetComponent<PhotonView>().ViewID);


            }

        }


    }

    public void RemoveNPCFromList(EnemySimpleFSM npc)
    {
        int photonViewID = npc.gameObject.GetComponent<PhotonView>().ViewID;

        PV.RPC("RPC_RemoveNPCMonsterFromList", RpcTarget.All, photonViewID);
        //Called by boids when they die


    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerController newPlayerController = other.GetComponent<PlayerController>();
            Transform newPlayerTransform = other.GetComponent<Transform>();
            if (!playerControllerList.Contains(newPlayerController))
            {
                playerControllerList.Add(newPlayerController);
            }
            if (!playerTransformList.Contains(newPlayerTransform))
            {

                playerTransformList.Add(newPlayerTransform);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerController exitingPlayerController = other.GetComponent<PlayerController>();
            Transform exitingPlayerTransform = other.GetComponent<Transform>();
            if (playerControllerList.Contains(exitingPlayerController))
            {
                playerControllerList.Remove(exitingPlayerController);
            }
            if (playerTransformList.Contains(exitingPlayerTransform))
            {

                playerTransformList.Remove(exitingPlayerTransform);
            }
        }
    }

    public List<PlayerController> GetPlayerControllersInRange()
    {
        return playerControllerList;
    }

    public List<Transform> GetPlayerTransformsInRange()
    {
        return playerTransformList;
    }


    public List<EnemySimpleFSM> GetNPCList()
    {
        return npcList;
    }

    private void CheckRespawn()
    {
        if(npcList.Count < 1)
        {
            GeneralNPCInstantiation();
        }
    }

    #region RPCs

    [PunRPC]
    private void RPC_SetFlockSize()
    {
        // Assigns a random value from a range for the size of the small NPCs that will spawn
        smallFlockSize = Random.Range(smallNPCMinimumSpawn, smallNPCMaximumSpawn);

        // Assigns a random value from a range for the size of the medium NPCs that will spawn
        mediumFlockSize = Random.Range(mediumNPCMinimumSpawn, mediumNPCMaximumSpawn);

        // Assigns a random value from a range for the size of the big NPCs that will spawn
        bigFlockSize = Random.Range(bigNPCMinimumSpawn, bigNPCMaximumSpawn);
    }

    [PunRPC]
    private void RPC_UpdateNPCMonsterList(int monsterPhotonViewID)
    {
        npcList.Add(PhotonView.Find(monsterPhotonViewID).gameObject.GetComponent<EnemySimpleFSM>());
    }

    [PunRPC]
    private void RPC_RemoveNPCMonsterFromList(int monsterPhotonViewID)
    {
        EnemySimpleFSM npcToRemove = PhotonView.Find(monsterPhotonViewID).gameObject.GetComponent<EnemySimpleFSM>();
        npcList.Remove(npcToRemove);
        if(npcToRemove.CheckPhotonView().IsMine)
        {
            npcToRemove.DeleteNPC();
        }
    }

    #endregion

    #region Unused Code

    //[SerializeField] float minimumNPCSpawn;
    //[SerializeField] float maximumNPCSpawn;

    //private float flockSize;

    /*
        flockSize = Random.Range(minimumNPCSpawn, maximumNPCSpawn);


        for (int i = 0; i <= flockSize; i++)
        {
            EnemySimpleFSM newGreenSlimeNPC = PhotonNetwork.InstantiateRoomObject(Path.Combine("NPCEnemyPrefabs", "GreenSlimeNPC"), transform.position, transform.rotation)
                .GetComponent<EnemySimpleFSM>();
            EnemySimpleFSM newBlueSlimeNPC = PhotonNetwork.InstantiateRoomObject(Path.Combine("NPCEnemyPrefabs", "BlueSlimeNPC"), transform.position, transform.rotation)
                .GetComponent<EnemySimpleFSM>();
            EnemySimpleFSM newRedSlimeNPC = PhotonNetwork.InstantiateRoomObject(Path.Combine("NPCEnemyPrefabs", "RedSlimeNPC"), transform.position, transform.rotation)
                .GetComponent<EnemySimpleFSM>();

            newGreenSlimeNPC.SetFlockController(this);
            newBlueSlimeNPC.SetFlockController(this);
            newRedSlimeNPC.SetFlockController(this);

            npcList.Add(newGreenSlimeNPC);
            npcList.Add(newBlueSlimeNPC);
            npcList.Add(newRedSlimeNPC);


        }
        */
    #endregion
}
