using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private protected PhotonView PV;

    [SerializeField] private protected float lifetime;
    private float projectileStartTime;

    private protected void Start()
    {
        projectileStartTime = Time.time;
    }

    private protected virtual void Update()
    {
        if(Time.time - projectileStartTime >= lifetime && PV.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
    public virtual void OnProjectileHit()
    {
        if (PV.IsMine)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    public PhotonView GetPhotonView()
    {
        return PV;
    }
}
