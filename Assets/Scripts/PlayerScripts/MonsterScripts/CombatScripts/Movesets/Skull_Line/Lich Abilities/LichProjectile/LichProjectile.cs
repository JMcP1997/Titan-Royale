using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LichProjectile : Projectile
{
    [SerializeField] Rigidbody rb;

    [SerializeField] float projectileSpeed;

    [SerializeField] Collider sphere;

    Vector3 shotDirection;
    public void SetProjectile(int photonView, Vector3 direction)
    {
        PV.RPC("RPC_SetProjectile", RpcTarget.All, photonView, direction);
    }

    [PunRPC]
    private void RPC_SetProjectile(int photonView, Vector3 direction)
    {
        PlayerCombatController combatController = PhotonView.Find(photonView).GetComponent<PlayerCombatController>();
        shotDirection = (direction - transform.position).normalized;
        rb.velocity = projectileSpeed * shotDirection;
    }

    public void SetSphere()
    {
        PV.RPC("RPC_SetSphere", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_SetSphere()
    {
        sphere.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            OnProjectileHit();
        }
    }
}
