using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElderHuskBasicAttack : BasicAttack 
{
    [SerializeField] AttackPoint[] damagePoints;

    private int index = 0;

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
        damagePoints[index].gameObject.SetActive(true);
        CheckIndex();
    }

    [PunRPC]
    void RPC_OnAttackEnd(int attackPointIndex)
    {
        damagePoints[attackPointIndex].Disable();
    }

    private void CheckIndex()
    {
        Debug.Log("IndexChecked");
        if (index + 1 >= damagePoints.Length)
        {
            index = 0;
        }
        else
        {
            index++;
        }
    }

    #endregion
}
