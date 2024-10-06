using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElderHuskEAbility : MonsterActiveAbility
{
    [SerializeField] float percentageReduction;

    [SerializeField] float maxBlockDuration;
    float timeSpentBlocking;

    private bool isBlocking;

    private void Update()
    {
        if (isBlocking)
        {
            timeSpentBlocking += Time.deltaTime;
            toggled = true;
        }
        if (!isBlocking)
        {
            timeSpentBlocking -= (Time.deltaTime * .5f);
            if(timeSpentBlocking < 0)
                timeSpentBlocking = 0;
            toggled = false;
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            PV.RPC("RPC_OnAbilityCompleted", RpcTarget.All);
        }
        if (timeSpentBlocking > maxBlockDuration)
        {
            PV.RPC("RPC_OnAbilityCompleted", RpcTarget.All);
            sourceController.StartCoroutine("E_AbilityCooldown");
            toggleReady = false;
            timeSpentBlocking = 0;
        }
    }

    public override void StartAbility()
    {
        PV.RPC("RPC_StartAbility", RpcTarget.All);
    }

    [PunRPC]
    private protected override void RPC_StartAbility()
    {
        MonsterController monsterController = sourceController.GetPlayerController().GetMonsterController();
        sourceController.GetPlayerController().GetAnimationController().GetCurrentAnimator().SetBool("isBlocking", true);
        monsterController.SetDamageReduction(percentageReduction);
        sourceController.SwitchAbilitiesUsable(false);
        sourceController.GetPlayerController().SetCanBeStunned(false);
        isBlocking = true;
    }

    [PunRPC]
    private protected override void RPC_OnAbilityCompleted()
    {
        MonsterController monsterController = sourceController.GetPlayerController().GetMonsterController();
        sourceController.GetPlayerController().GetAnimationController().GetCurrentAnimator().SetBool("isBlocking", false);
        monsterController.ClearDamageReduction();
        sourceController.SwitchAbilitiesUsable(true);
        sourceController.GetPlayerController().SetCanBeStunned(true);
        isBlocking = false;
    }

    public override void Use(Collider other)
    {
        return;
    }

    public override float GetMaxToggleDuration()
    {
        return maxBlockDuration;
    }
}
