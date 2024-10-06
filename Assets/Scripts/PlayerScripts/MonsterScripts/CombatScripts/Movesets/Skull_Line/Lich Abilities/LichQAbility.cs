using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LichQAbility : MonsterActiveAbility
{
    [SerializeField] GameObject projectilePrefab;

    [SerializeField] Transform projectileSpawnPoint;

    Vector3 lookDirection;

    [SerializeField] LayerMask projectileLayerMask;

    GameObject mostRecentProjectile;

    private void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, projectileLayerMask))
        {
            lookDirection = raycastHit.point;
        }
    }

    public override void StartAbility()
    {
        return;
    }

    public void SummonSpiritBomb()
    {
        PV.RPC("RPC_StartAbility", RpcTarget.All);
    }

    public void PushSpiritBomb()
    {
        PV.RPC("RPC_PushSpiritBomb", RpcTarget.All);
    }

    [PunRPC]
    private protected override void RPC_StartAbility()
    {
        if (PV.IsMine)
        {
            GameObject firedProjectile = PhotonNetwork.Instantiate(Path.Combine("AbilityPrefabs", "SkullAbilities", projectilePrefab.name), projectileSpawnPoint.position, Quaternion.LookRotation(lookDirection, Vector3.up));
            firedProjectile.GetComponent<RangedAbilityPoint>().SetAbilityPoint(sourceController.GetPlayerController().GetPhotonView().ViewID, "Stage3_Lich/Moveset/Lich_QAbility");
            mostRecentProjectile = firedProjectile;
        }
    }

    [PunRPC]
    private void RPC_PushSpiritBomb()
    {
        if(PV.IsMine)
        {
            if (mostRecentProjectile != null)
            {
                mostRecentProjectile.GetComponentInChildren<SpiritBombProjectile>().SetProjectile(sourceController.GetPlayerController().GetPhotonView().ViewID, lookDirection);
                mostRecentProjectile.GetComponentInChildren<SpiritBombProjectile>().SetSphere();
            }
        }
    }
}
