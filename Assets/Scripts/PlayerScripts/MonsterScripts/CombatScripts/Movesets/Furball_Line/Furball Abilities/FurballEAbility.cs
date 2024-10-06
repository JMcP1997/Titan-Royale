using System.IO;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurballEAbility : MonsterActiveAbility
{
    [SerializeField] private GameObject shockwavePrefab;

    public override void Use(Collider other)
    {
        PhotonNetwork.Instantiate(Path.Combine("AbilityPrefabs", "FurballAbilities", shockwavePrefab.name), abilityPoints[0].transform.position, Quaternion.identity).
            GetComponent<FurballShockwave>().SetColliderToIgnore(PV.ViewID);
    }
}
