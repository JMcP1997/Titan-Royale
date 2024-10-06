using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkullBasicAttack : BasicAttack
{
    [SerializeField] Transform firePoint;

    [SerializeField] GameObject projectilePrefab;
    [SerializeField] LayerMask projectileLayerMask;

    [Range(0, 1)]
    [SerializeField] float aimSensitivity;

    private Vector3 shotDirection;

    [SerializeField] CinemachineVirtualCamera aimCamera;

    private bool isAiming;
    [SerializeField] private PlayerCameraController cameraController;

    private protected override void OnEnable()
    {
        base.OnEnable();
        if (!PV.IsMine)
        {
            Destroy(aimCamera.gameObject);
        }
    }

    public override void OnAttackEnd(int attackPointIndex)
    {
        return;
    }

    private void Update()
    {
        if(!PV.IsMine)
        {
            return;
        }
        Vector3 lookDirection = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, projectileLayerMask))
        {
            lookDirection = raycastHit.point;
            shotDirection = (raycastHit.point - firePoint.position).normalized;
        }

        isAiming = Input.GetMouseButton(1);

        if(isAiming)
        {
            aimCamera.gameObject.SetActive(true);
            cameraController.SetSensitivity(aimSensitivity);

            Vector3 aimDirection = (lookDirection - transform.position).normalized;

            sourceController.gameObject.transform.forward = Vector3.Lerp(sourceController.gameObject.transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            aimCamera.gameObject.SetActive(false);
            cameraController.SetSensitivity(cameraController.GetNormalSensitivity());
        }
    }

    public override void OnAttackStart()
    {
        GameObject firedProjectile = PhotonNetwork.Instantiate(Path.Combine("AbilityPrefabs", "SkullAbilities", projectilePrefab.name), firePoint.position, Quaternion.identity);
        firedProjectile.GetComponent<SkullProjectile>().SetProjectile(sourceController.GetPlayerController().GetPhotonView().ViewID, shotDirection);
        firedProjectile.GetComponent<RangedAttackPoint>().SetCombatController(sourceController.GetPlayerController().GetPhotonView().ViewID);
    }

}
