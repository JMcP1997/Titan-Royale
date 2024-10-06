using Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LichBasicAttack : BasicAttack
{
    [SerializeField] AttackPoint[] attackPoints;


    public override void OnAttackEnd(int attackPointIndex)
    {
        PV.RPC("RPC_OnAttackEnd", RpcTarget.All);
    }

    public override void OnAttackStart()
    {
        PV.RPC("RPC_OnAttackStart", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_OnAttackStart()
    {
        foreach (AttackPoint attackPoint in attackPoints)
        {
            attackPoint.gameObject.SetActive(true);
        }
    }

    [PunRPC]
    private void RPC_OnAttackEnd()
    {
        foreach (AttackPoint attackPoint in attackPoints)
        {
            attackPoint.Disable();
        }
    }
}
