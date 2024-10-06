using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAbilityPoint : MonoBehaviour
{
    protected PlayerCombatController combatController;
    protected MonsterActiveAbility sourceAbility;

    [SerializeField] Projectile projectile;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other == combatController.GetPlayerController().GetCharacterController())
        {
            return;
            //Check to make sure we're not hitting ourself
        }
        else if (sourceAbility.GetImmuneColliders().Contains(other))
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
                Debug.Log(other.gameObject);
                break;
        }
    }

    private void HandlePlayerCombat(Collider other)
    {
        MonsterController enemyMonster = other.GetComponent<MonsterController>();

        sourceAbility.Use(other);
        other.GetComponent<IDamageable>().TakeDamage(sourceAbility.GetAbilityDamage());

        PlayAbilitySound(EvaluateAbilitySource(sourceAbility));

        if (enemyMonster.CheckDeath())
        {
            combatController.OnKilledEnemy(enemyMonster.GetExperienceForKillingThisUnit());
        }

        projectile.OnProjectileHit();
    }

    private void HandleNPCCombat(Collider other)
    {
        NPCController enemyNPC = other.GetComponent<NPCController>();

        enemyNPC.UpdatePlayerHostility(combatController.GetPlayerController());

        sourceAbility.Use(other);

        PlayAbilitySound(EvaluateAbilitySource(sourceAbility));

        other.GetComponent<IDamageable>().TakeDamage(sourceAbility.GetAbilityDamage());

        projectile.OnProjectileHit();

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
        switch (abilityKeyBind)
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

    public void SetAbilityPoint(int PhotonViewID, string abilityPathName)
    {
        combatController = PhotonView.Find(PhotonViewID).gameObject.GetComponent<PlayerCombatController>();
        sourceAbility = combatController.gameObject.transform.Find(abilityPathName).GetComponent<MonsterActiveAbility>();
    }

}
