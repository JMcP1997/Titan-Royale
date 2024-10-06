using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicAttack : MonoBehaviour
{

    [SerializeField] private protected int damage;
    [SerializeField] private protected float attackRate;

    [SerializeField] private protected PlayerCombatController sourceController;

    protected PhotonView PV;

    private protected List<Collider> immuneColliders = new List<Collider>();

    private protected virtual void OnEnable()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Awake()
    {
        sourceController = GetComponentInParent<PlayerCombatController>();
        PV = GetComponent<PhotonView>();
    }

    public abstract void OnAttackStart();
    public abstract void OnAttackEnd(int attackPointIndex);

    public int GetDamage()
    {
        return damage;
    }

    public float GetAttackRate()
    {
        return attackRate;
    }

    public void AddCollider(int photonViewID)
    {
        PV.RPC("RPC_AddCollider", RpcTarget.All, photonViewID);
    }

    public void RemoveCollider(int photonViewID)
    {
        PV.RPC("RPC_RemoveCollider", RpcTarget.All, photonViewID);
    }

    [PunRPC]
    private protected void RPC_AddCollider(int photonViewID)
    {
        Collider colliderToIgnore = PhotonView.Find(photonViewID).gameObject.GetComponent<Collider>();
        immuneColliders.Add(colliderToIgnore);
    }

    [PunRPC]
    private protected void RPC_RemoveCollider(int photonViewID)
    {
        if(PhotonView.Find(photonViewID).gameObject.GetComponent<Collider>())
        {
            Collider colliderToIgnore = PhotonView.Find(photonViewID).gameObject.GetComponent<Collider>();
            if (immuneColliders.Contains(colliderToIgnore))
            {
                immuneColliders.Remove(colliderToIgnore);
            }
        }

        else return;
    }
}
