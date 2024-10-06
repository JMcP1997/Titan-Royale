using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [SerializeField] List<FlockController> allFlockControllers;

    private List<FlockController> randomlySelectedFlockControllers = new List<FlockController>();

    [SerializeField] float desiredNumberOfActiveSpawners;

    List<int> randomSelectedFlockViewIDList = new List<int>();
    List<int> unselectedFlockViewIDList = new List<int>();

    PhotonView PV;


    private void Start()
    {
        PV = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            // Handle the selection of flock controllers and activate them
            HandleSelectionOfFlockControllers();
            CreateViewIDList();
            ActivateRandomlySelectedControllers();
        }
    }

    public void HandleSelectionOfFlockControllers()
    {
        for (int i = 0; i < desiredNumberOfActiveSpawners; i++)
        {
            int randomSelection = Random.Range(0, allFlockControllers.Count);

            FlockController randomlySelectedController = allFlockControllers[randomSelection];

            allFlockControllers.Remove(randomlySelectedController);
            randomlySelectedFlockControllers.Add(randomlySelectedController);
        }
    }

    private void CreateViewIDList()
    {

        foreach(FlockController controller in randomlySelectedFlockControllers)
        {
            int controllerPhotonViewID = controller.gameObject.GetComponent<PhotonView>().ViewID;
            randomSelectedFlockViewIDList.Add(controllerPhotonViewID);
        }
        foreach(FlockController controller in allFlockControllers)
        {
            int controllerPhotonViewID = controller.gameObject.GetComponent<PhotonView>().ViewID;
            unselectedFlockViewIDList.Add(controllerPhotonViewID);
        }

        PV.RPC("RPC_ReceiveRandomlySelectedFlockControllers", RpcTarget.AllBuffered, randomSelectedFlockViewIDList.ToArray(), unselectedFlockViewIDList.ToArray());
    }

    private void ActivateRandomlySelectedControllers()
    {
        foreach(FlockController controller in randomlySelectedFlockControllers)
        {
            controller.GeneralNPCInstantiation();
        }
    }

    [PunRPC]
    private void RPC_ReceiveRandomlySelectedFlockControllers(int[] flockControllersID, int[] unusedFlockControllersID)
    {
        randomlySelectedFlockControllers.Clear();
        allFlockControllers.Clear();

        foreach(int controllerID in flockControllersID)
        {
            FlockController controller = PhotonView.Find(controllerID).gameObject.GetComponent< FlockController>();
            randomlySelectedFlockControllers.Add(controller);
        }

        foreach (int unusedControllerID in unusedFlockControllersID)
        {
            FlockController unusedController = PhotonView.Find(unusedControllerID).gameObject.GetComponent<FlockController>();
            allFlockControllers.Add(unusedController);
            unusedController.unused = true;
        }
    }
}