using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class LichEAbility : MonsterActiveAbility
{
    private bool isCharging;
    float timeSpentCharging = 0;
    float maxCharge = 5f;

    [SerializeField] LayerMask projectileLayerMask;

    [SerializeField] GameObject projectilePrefab;

    [SerializeField] private Transform[] firePoints;
    int firePointIndex = 0;

    private List<LichProjectile> preparedProjectiles = new List<LichProjectile>();

    Vector3 lookDirection;

    private void Update()
    {
        if (isCharging)
        {
            timeSpentCharging += Time.deltaTime;
            toggled = true;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            PV.RPC("RPC_OnAbilityCompleted", RpcTarget.All);
        }
        if (timeSpentCharging > maxCharge)
        {
            PV.RPC("RPC_OnAbilityCompleted", RpcTarget.All);
            timeSpentCharging = 0;
        }

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, projectileLayerMask))
        {
            lookDirection = raycastHit.point;
        }
    }

    public override void StartAbility()
    {
        PV.RPC("RPC_StartAbility", RpcTarget.All);
    }

    [PunRPC]
    private protected override void RPC_StartAbility()
    {
        isCharging = true;
        InvokeRepeating("PrepareProjectiles", 0, 1);
    }

    [PunRPC]
    private protected override void RPC_OnAbilityCompleted()
    {
        CancelInvoke();
        PV.RPC("RPC_ReleaseProjectiles", RpcTarget.All);
        firePointIndex = 0;
        isCharging = false;
        toggled = false;
        sourceController.StartCoroutine("E_AbilityCooldown");
        toggleReady = false;
        RefillToggleMeter();
    }

    public override float GetMaxToggleDuration()
    {
        return maxCharge;
    }

    private void PrepareProjectiles()
    {
        PV.RPC("RPC_PrepareProjectiles", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_PrepareProjectiles()
    {
        if(PV.IsMine)
        {
            LichProjectile preparedProjectile = PhotonNetwork.Instantiate(Path.Combine("AbilityPrefabs", "SkullAbilities", projectilePrefab.name), firePoints[firePointIndex].position, Quaternion.LookRotation(lookDirection, Vector3.up)).GetComponent<LichProjectile>();
            preparedProjectile.transform.parent = firePoints[firePointIndex];
            preparedProjectiles.Add(preparedProjectile);
            firePointIndex++;
            if (firePointIndex >= firePoints.Length)
            {
                firePointIndex = 0;
                OnAbilityCompleted();
            }
        }
    }

    [PunRPC]
    private void RPC_ReleaseProjectiles()
    {
        if(PV.IsMine)
        {
            foreach (LichProjectile preparedProjectile in preparedProjectiles)
            {
                preparedProjectile.transform.parent = null;
                preparedProjectile.SetSphere();
                preparedProjectile.SetProjectile(PV.ViewID, lookDirection);
                preparedProjectile.gameObject.GetComponent<RangedAbilityPoint>().SetAbilityPoint(sourceController.GetPlayerController().GetPhotonView().ViewID, "Stage3_Lich/Moveset/Lich_EAbility");
            }
            preparedProjectiles.Clear();
        }
    }

    private void RefillToggleMeter()
    {
        foreach(AbilityIcon icon in sourceController.GetPlayerController().GetUIController().GetAbilityIcons())
        {
            if(icon.GetSourceAbility() == this)
            {
                icon.FillToggleMeter();
                break;
            }
        }
    }
}
