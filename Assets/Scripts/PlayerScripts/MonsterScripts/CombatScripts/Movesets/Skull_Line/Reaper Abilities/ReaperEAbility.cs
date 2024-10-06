using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class ReaperEAbility : MonsterActiveAbility
{
    [SerializeField] Transform abilitySpawnPoint;

    [SerializeField] GameObject projectilePrefab;

    private Vector3 projectileDirection;

    [SerializeField] LayerMask projectileLayerMask;

    [SerializeField] float stunTime;
    public override void Use(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayerController enemyPlayer = other.gameObject.GetComponent<PlayerController>();
            enemyPlayer.OnPlayerStunned(stunTime);
        }
    }

    private void Update()
    {
        if(!PV.IsMine)
        {
            return;
        }

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, projectileLayerMask))
        {
            projectileDirection = (raycastHit.point - sourceController.gameObject.transform.position).normalized;
        }
    }

    public override void StartAbility()
    {
        PV.RPC("RPC_StartAbility", RpcTarget.All);
    }

    [PunRPC]
    private protected override void RPC_StartAbility()
    {
        GameObject firedProjectile = PhotonNetwork.Instantiate(Path.Combine("AbilityPrefabs", "SkullAbilities", projectilePrefab.name), abilitySpawnPoint.position, Quaternion.LookRotation(projectileDirection, Vector3.up));
        firedProjectile.GetComponentInChildren<ReaperProjectile>().SetProjectile(sourceController.GetPlayerController().GetPhotonView().ViewID, projectileDirection);
        firedProjectile.GetComponentInChildren<RangedAbilityPoint>().SetAbilityPoint(sourceController.GetPlayerController().GetPhotonView().ViewID, "Stage2_Reaper/Moveset/Reaper_EAbility");
    }
}
