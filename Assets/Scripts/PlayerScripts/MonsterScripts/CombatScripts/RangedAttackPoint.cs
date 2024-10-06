using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackPoint : AttackPoint
{
    [SerializeField] Projectile sourceProjectile;

    private protected override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamageable>() == combatController.GetPlayerController().GetComponent<IDamageable>())
        {
            if(combatController.GetPlayerController().GetPhotonView().IsMine)
            {
                PhotonNetwork.Destroy(sourceProjectile.gameObject);
            }
            //Normally this is used to prevent players from hitting themselves and instructs the attack point to simply ignore
            //The source player, but if we dont destroy the projectile, players will be able to fire while running away
        }
        else
        {
            targetTag = other.gameObject.tag;
            EvaluateCombatTarget(targetTag, other);
        }
    }

    private protected override void HandlePlayerCombat(Collider other)
    {
        other.GetComponent<IDamageable>().TakeDamage(combatController.GetCurrentBasicAttack().GetDamage());
        if (other.TryGetComponent<MonsterController>(out MonsterController monster))
        {
            if (monster.CheckDeath())
            {
                combatController.OnKilledEnemy(monster.GetExperienceForKillingThisUnit());
            }
        }

        sourceProjectile.OnProjectileHit();
    }

    private protected override void HandleNPCCombat(Collider other)
    {
        NPCController npc = other.GetComponent<NPCController>();

        npc.UpdatePlayerHostility(combatController.GetPlayerController());

        other.GetComponent<IDamageable>().TakeDamage(combatController.GetCurrentBasicAttack().GetDamage());

        sourceProjectile.OnProjectileHit();
    }

    public void SetCombatController(int photonView)
    {
        GetComponent<PhotonView>().RPC("RPC_SetCombatController", RpcTarget.All, photonView);
    }

    [PunRPC]
    private void RPC_SetCombatController(int photonView)
    {
        combatController = PhotonView.Find(photonView).GetComponent<PlayerCombatController>();
    }
}
