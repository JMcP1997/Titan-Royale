using Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperBasicAttack : BasicAttack
{
    [SerializeField] AttackPoint[] attackPoints;

    private bool firstAttack = false;

    private protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnAttackEnd(int attackPointIndex)
    {
        PV.RPC("RPC_OnAttackEnd", RpcTarget.All);
    }

    public override void OnAttackStart()
    {
        PV.RPC("RPC_OnAttackStart", RpcTarget.All);
    }


    #region RPCs

    [PunRPC]
    void RPC_OnAttackStart()
    {
        if (firstAttack)
        {
            attackPoints[0].gameObject.SetActive(true);
        }
        else
        {
            foreach (AttackPoint attackPoint in attackPoints)
            {
                attackPoint.gameObject.SetActive(true);
            }
        }
        firstAttack = !firstAttack;
    }

    [PunRPC]
    void RPC_OnAttackEnd()
    {
        foreach(AttackPoint attackPoint in attackPoints)
        {
            attackPoint.Disable();
        }
    }

    #endregion
}
