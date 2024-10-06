using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperProjectile : Projectile
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider sphere;

    [SerializeField] float projectileSpeed;

    [SerializeField] ParticleSystem contactParticles;

    Vector3 projectileDirection;

    [SerializeField] SimpleDestroy parentDestroy;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void SetProjectile(int photonView, Vector3 direction)
    {
        PV.RPC("RPC_SetProjectile", RpcTarget.All, photonView, direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            OnProjectileHit();
        }
    }

    public override void OnProjectileHit()
    {
        contactParticles.Play();
        if(PV.IsMine)
        {
            parentDestroy.DestroyParent();
        }
        
    }

    public void SetSpeed()
    {
        PV.RPC("RPC_SetSphere", RpcTarget.All);
        rb.velocity = projectileDirection * projectileSpeed;
    }

    [PunRPC]
    private void RPC_SetProjectile(int photonView, Vector3 direction)
    {
        PlayerCombatController combatController = PhotonView.Find(photonView).GetComponent<PlayerCombatController>();
        projectileDirection = direction.normalized;
    }

    [PunRPC]
    private void RPC_SetSphere()
    {
        GetComponent<Animator>().enabled = false;
        sphere.enabled = true;
    }
}
