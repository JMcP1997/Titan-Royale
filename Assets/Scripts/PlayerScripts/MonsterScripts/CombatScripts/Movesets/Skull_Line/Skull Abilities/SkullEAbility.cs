using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullEAbility : MonsterActiveAbility
{
    [SerializeField] SkinnedMeshRenderer smr;

    [SerializeField] float maxInvisibleDuration;
    float timeSpentInvisible;

    [SerializeField] ParticleSystem abilityEffect;

    [SerializeField] GameObject namePlate;

    private bool isInvisible;

    private void Update()
    {
        if (isInvisible)
        {
            timeSpentInvisible += Time.deltaTime;
            toggled = true;
        }
        if (!isInvisible)
        {
            timeSpentInvisible -= (Time.deltaTime * .5f);
            if (timeSpentInvisible < 0)
                timeSpentInvisible = 0;
            toggled = false;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            PV.RPC("RPC_OnAbilityCompleted", RpcTarget.All);
        }
        if (timeSpentInvisible > maxInvisibleDuration)
        {
            PV.RPC("RPC_OnAbilityCompleted", RpcTarget.All);
            sourceController.StartCoroutine("E_AbilityCooldown");
            toggleReady = false;
            timeSpentInvisible = 0;
        }
    }

    public override void StartAbility()
    {
        PV.RPC("RPC_StartAbility", RpcTarget.All);
    }

    [PunRPC]
    private protected override void RPC_StartAbility()
    {
        smr.enabled = false;
        abilityEffect.Play();
        sourceController.SwitchAbilitiesUsable(false);
        namePlate.SetActive(false);
        PV.RPC("RPC_SwitchInvisibility", RpcTarget.All, true);
    }

    [PunRPC]
    private protected override void RPC_OnAbilityCompleted()
    {
        smr.enabled = true;
        if(isInvisible)
        {
            abilityEffect.Play();
        }
        if (!PV.IsMine)
        {
            namePlate.SetActive(true);
        }
        sourceController.SwitchAbilitiesUsable(true);
        PV.RPC("RPC_SwitchInvisibility", RpcTarget.All, false);
    }

    public override void Use(Collider other)
    {
        return;
    }

    public override float GetMaxToggleDuration()
    {
        return maxInvisibleDuration;
    }

    [PunRPC]
    private void RPC_SwitchInvisibility(bool isInvisible)
    {
        this.isInvisible = isInvisible;
    }
}
