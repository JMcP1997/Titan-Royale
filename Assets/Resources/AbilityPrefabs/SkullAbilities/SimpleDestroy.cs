using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDestroy : MonoBehaviour
{
    public void DestroyParent()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
