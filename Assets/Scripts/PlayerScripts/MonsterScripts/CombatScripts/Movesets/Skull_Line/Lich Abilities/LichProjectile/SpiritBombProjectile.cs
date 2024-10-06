using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpiritBombProjectile : Projectile
{
    Vector3 shotDirection;

    [SerializeField] Rigidbody rb;

    [SerializeField] float projectileSpeed;

    [SerializeField] SphereCollider sphere;

    Vector3 startPosition;

    [SerializeField] GameObject particles;

    [SerializeField] MeshRenderer mr;

    private float targetRadius;

    private void Awake()
    {
        targetRadius = sphere.radius * 1.5f;
        startPosition = transform.position;
        particles.SetActive(false);
    }

    public void SetProjectile(int photonView, Vector3 direction)
    {
        PV.RPC("RPC_SetProjectile", RpcTarget.All, photonView, direction);
    }

    private protected override void Update()
    {
        base.Update();
        if(shotDirection != null)
        {
            gameObject.transform.LookAt(shotDirection);
        }
    }

    public override void OnProjectileHit()
    {
        StartCoroutine(SpiritBombExplosion());
    }

    [PunRPC]
    private void RPC_SetProjectile(int photonView, Vector3 direction)
    {
        PlayerCombatController combatController = PhotonView.Find(photonView).GetComponent<PlayerCombatController>();
        shotDirection = (direction - startPosition).normalized;
        rb.velocity = Vector3.zero;
    }

    public void SetSphere()
    {
        PV.RPC("RPC_SetSphere", RpcTarget.All);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            OnProjectileHit();
        }
    }

    [PunRPC]
    private void RPC_SetSphere()
    {
        GetComponent<Animator>().enabled = false;
        sphere.enabled = true;
        rb.velocity = shotDirection * projectileSpeed;
    }

    IEnumerator SpiritBombExplosion()
    {
        mr.enabled = false;
        particles.SetActive(true);
        float timeElapsed = 0;
        float lerpDuration = 1f;
        while (timeElapsed < lerpDuration)
        {
            sphere.radius = Mathf.Lerp(sphere.radius, targetRadius, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        base.OnProjectileHit();
    }
}
