using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SasquatchEAbility : MonsterActiveAbility
{
    [SerializeField] private GameObject shockwavePrefab;

    public override void Use(Collider other)
    {
        Debug.Log("Hit the ground");
        PhotonNetwork.Instantiate(Path.Combine("AbilityPrefabs", "FurballAbilities", shockwavePrefab.name), abilityPoints[0].transform.position, Quaternion.identity).
            GetComponent<FurballShockwave>().SetColliderToIgnore(PV.ViewID);
    }
}
