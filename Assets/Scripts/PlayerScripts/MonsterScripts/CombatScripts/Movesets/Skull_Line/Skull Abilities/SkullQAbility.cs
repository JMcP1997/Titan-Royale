using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class SkullQAbility : MonsterActiveAbility
{
    [SerializeField] float teleportationMaxDistance;

    [SerializeField] LayerMask teleportLayerMask;

    [SerializeField] GameObject playerBody;
    //[SerializeField] GameObject teleportAimIndicator;

    [SerializeField] Vector3 teleportPointOffset = new Vector3(0, 1, -.75f);

    [SerializeField] ParticleSystem abilityParticles;

#nullable enable
    Vector3 teleportationPoint;
    float distanceToTeleportPoint;
#nullable disable

    private void Update()
    {
        /*
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, teleportLayerMask))
        {
            Vector3 raycastHitDirection = (raycastHit.point - playerBody.transform.position).normalized;
            Vector3 originPoint = playerBody.transform.position;

            if (Vector3.Distance(originPoint, raycastHit.point) < teleportationMaxDistance)
            {
                teleportAimIndicator.transform.position = (originPoint += raycastHitDirection * Vector3.Distance(originPoint, raycastHit.point) + teleportPointOffset) ;
            }
            else teleportAimIndicator.transform.position = (originPoint += raycastHitDirection * teleportationMaxDistance) + teleportPointOffset;
        }
        */
    }

    public override void Use()
    {
        if (teleportationPoint != null)
        {
            PV.RPC("RPC_Teleport", RpcTarget.All);
        }
    }

    [PunRPC]
    private protected override void RPC_StartAbility()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);


        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, teleportLayerMask))
        {
            Vector3 raycastHitDirection = (raycastHit.point - playerBody.transform.position).normalized;
            Vector3 originPoint = playerBody.transform.position;

            if (Vector3.Distance(originPoint, raycastHit.point) < teleportationMaxDistance)
            {
                teleportationPoint = (originPoint += raycastHitDirection * Vector3.Distance(originPoint, raycastHit.point) + teleportPointOffset);

                Debug.Log(teleportationPoint);

                Use();
            }

            else
            {
                teleportationPoint = (originPoint += raycastHitDirection * teleportationMaxDistance) + teleportPointOffset;
                Debug.Log(teleportationPoint);

                Use();
            }
        }
    }
    
    [PunRPC]
    private void RPC_Teleport()
    {
        PlayerController playerController = sourceController.GetPlayerController();
        CharacterController characterController = playerController.GetMovementController().GetCharacterController();

        playerController.SetCanMove(false);
        abilityParticles.Play();
        playerController.ToggleCameraLocked(true);

        characterController.enabled = false;

        playerBody.transform.position = teleportationPoint;

        characterController.enabled = true;

        playerController.SetCanMove(true);
        abilityParticles.Play();
        playerController.ToggleCameraLocked(false);
    }

}
