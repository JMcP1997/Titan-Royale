using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPoint : MonoBehaviour
{
    [SerializeField] protected PlayerCombatController combatController;
    [SerializeField] protected MonsterActiveAbility sourceAbility;

    private float immuneTime;

    private void Start()
    {
        if (sourceAbility.GetAbilityCooldown() > 3)
        {
            immuneTime = 1;
        }
        else
        {
            immuneTime = sourceAbility.GetAbilityCooldown()/2;
        }
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if(other == combatController.GetPlayerController().GetCharacterController())
        {
            return;
            //Check to make sure we're not hitting ourself
        }
        else if(sourceAbility.GetImmuneColliders().Contains(other))
        {
            return;
            //Check to make sure we havent already hit the target collider
        }
        else
        {
            string targetTag = other.tag;
            EvaluateCombatTarget(targetTag, other);
        }
    }

    private void EvaluateCombatTarget(string targetTag, Collider other)
    {
        switch (targetTag)
        {
            case "Player":
                HandlePlayerCombat(other);
                break;
            case "NPC":
                HandleNPCCombat(other);
                break;
            default:
                Debug.Log("No Tag Detected, please check to make sure all characters and objects are appropriately tagged");
                break;
        }
    }

    private void HandlePlayerCombat(Collider other)
    {
        MonsterController enemyMonster = other.GetComponent<MonsterController>();

        sourceAbility.Use(other);
        other.GetComponent<IDamageable>().TakeDamage(sourceAbility.GetAbilityDamage());
        sourceAbility.ImmuneTimer(other, immuneTime);

        PlayAbilitySound(EvaluateAbilitySource(sourceAbility));

        if(enemyMonster.CheckDeath())
        {
            combatController.OnKilledEnemy(enemyMonster.GetExperienceForKillingThisUnit());
        }
    }

    private void HandleNPCCombat(Collider other)
    {
        NPCController enemyNPC = other.GetComponent<NPCController>();

        enemyNPC.UpdatePlayerHostility(combatController.GetPlayerController());

        sourceAbility.Use(other);

        PlayAbilitySound(EvaluateAbilitySource(sourceAbility));

        other.GetComponent<IDamageable>().TakeDamage(sourceAbility.GetAbilityDamage());

        sourceAbility.ImmuneTimer(other, immuneTime);

    }

    private string EvaluateAbilitySource(MonsterActiveAbility sourceAbilityForComparison)
    {
        if (sourceAbilityForComparison == combatController.GetCurrentQAbility())
        {
            return "Q";
        }
        if (sourceAbilityForComparison == combatController.GetCurrentEAbility())
        {
            return "E";
        }
        else return "No appropriate Keybind string found";
    }

    private void PlayAbilitySound(string abilityKeyBind)
    {
        switch(abilityKeyBind)
        {
            case "Q":
                combatController.GetPlayerController().OnPlayerUsedAbility_Q();
                break;
            case "E":
                combatController.GetPlayerController().OnPlayerUsedAbility_E();
                break;
            default:
                Debug.Log(abilityKeyBind);
                break;
        }
    }
    

}
