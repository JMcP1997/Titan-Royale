using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullProjectile : Projectile
{
    [SerializeField] Rigidbody rb;

    [SerializeField] float projectileSpeed;

    [SerializeField] ParticleSystem contactParticles;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void SetProjectile(int photonView, Vector3 direction)
    {
        PV.RPC("RPC_SetProjectile", RpcTarget.All, photonView, direction);
    }

    public override void OnProjectileHit()
    {
        contactParticles.Play();
        base.OnProjectileHit();
    }

    [PunRPC]
    private void RPC_SetProjectile(int photonView, Vector3 direction)
    {
        PlayerCombatController combatController = PhotonView.Find(photonView).GetComponent<PlayerCombatController>();
        rb.velocity = projectileSpeed * direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            OnProjectileHit();
        }
    }
}
