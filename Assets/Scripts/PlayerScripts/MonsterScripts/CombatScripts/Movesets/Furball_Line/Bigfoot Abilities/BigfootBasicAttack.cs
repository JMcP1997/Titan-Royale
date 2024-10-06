using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigfootBasicAttack : BasicAttack
{

    [SerializeField] AttackPoint damagePoint;

    public override void OnAttackStart()
    {
        PV.RPC("RPC_OnAttackStart", RpcTarget.All);
    }

    public override void OnAttackEnd(int attackPointIndex)
    {
        PV.RPC("RPC_OnAttackEnd", RpcTarget.All, attackPointIndex);
    }

    #region RPCs

    [PunRPC]
    void RPC_OnAttackStart()
    {
        damagePoint.gameObject.SetActive(true);
    }

    [PunRPC]
    void RPC_OnAttackEnd(int attackPointIndex)
    {
        damagePoint.Disable();
    }

    #endregion

}
