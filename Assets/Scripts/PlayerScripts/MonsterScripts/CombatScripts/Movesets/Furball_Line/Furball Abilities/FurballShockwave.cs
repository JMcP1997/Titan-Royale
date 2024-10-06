using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurballShockwave : MonoBehaviour
{
    [SerializeField] PhotonView PV;
    [SerializeField] SphereCollider sphereCollider;

    private MonsterActiveAbility sourceAbility;

    [SerializeField] float targetRadius;

    private float shockwaveIntensity = 6;

    private List<Collider> hitColldiers = new List<Collider>();

    [PunRPC]
    void DestroyShockwave()
    {
        if(PV.IsMine)
        {
            PhotonNetwork.Destroy(PV);
        }
    }

    [PunRPC]
    void StartExpansion()
    {
        StartCoroutine("ShockwaveExpansion");
    }

    public IEnumerator ShockwaveExpansion()
    {
        float timeElapsed = 0;
        float lerpDuration = .65f;
        while (timeElapsed < lerpDuration)
        {
            sphereCollider.radius = Mathf.Lerp(sphereCollider.radius, targetRadius, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        PV.RPC("DestroyShockwave", RpcTarget.All);
    }

    public void SetColliderToIgnore(int photonViewID)
    {
        PV.RPC("RPC_SetColliderToIgnore", RpcTarget.All, photonViewID);
        PV.RPC("StartExpansion", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_SetColliderToIgnore(int photonViewID)
    {
        sourceAbility = PhotonView.Find(photonViewID).GetComponent<MonsterActiveAbility>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other == sourceAbility.GetSourceController().GetPlayerController().GetCharacterController() || hitColldiers.Contains(other))
        {
            return;
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Vector3 colliderDirection = ((other.gameObject.transform.position) - gameObject.transform.position).normalized;
                CharacterController targetController = other.gameObject.GetComponent<CharacterController>();

                targetController.Move(colliderDirection * Time.deltaTime * shockwaveIntensity);
                other.GetComponent<IDamageable>().TakeDamage(sourceAbility.GetSourceController().GetCurrentQAbility().GetAbilityDamage());

                hitColldiers.Add(other);
            }
            else if(other.gameObject.CompareTag("NPC"))
            {
                Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
                Vector3 colliderDirection = ((other.gameObject.transform.position) - gameObject.transform.position).normalized;

                rb.AddForce(colliderDirection * shockwaveIntensity);
                other.GetComponent<IDamageable>().TakeDamage(sourceAbility.GetSourceController().GetCurrentQAbility().GetAbilityDamage());

                hitColldiers.Add(other);
            }
        }
    }
}
