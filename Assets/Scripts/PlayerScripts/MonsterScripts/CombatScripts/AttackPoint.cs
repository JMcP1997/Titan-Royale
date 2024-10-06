using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    [SerializeField] protected PlayerCombatController combatController;

    private protected List<Collider> immuneColliders = new List<Collider>();

    private protected float immuneTime = .2f;

    private protected string targetTag;

    [SerializeField] bool criticalAttackPoint;

    private protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamageable>() == combatController.GetPlayerController().GetComponent<IDamageable>())
        {
            return;
            //Dont want to be able to punch yourself;
        }
        else
        {
            targetTag = other.gameObject.tag;
            EvaluateCombatTarget(targetTag, other);
        }
    }

    private protected void EvaluateCombatTarget(string targetTag, Collider other)
    {
        switch(targetTag)
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
        targetTag = "";
    }
    private protected virtual void HandlePlayerCombat(Collider other)
    {
        if (!immuneColliders.Contains(other))
        {
            other.GetComponent<IDamageable>().TakeDamage(criticalAttackPoint? (int)(combatController.GetCurrentBasicAttack().GetDamage() * 1.25f) : combatController.GetCurrentBasicAttack().GetDamage());
            StartCoroutine(ImmunityTimer(other.GetComponent<PhotonView>().ViewID));
            //If target is elligible for damage, apply damage to the target
            if (other.TryGetComponent<MonsterController>(out MonsterController monster))
            {
                if (monster.CheckDeath())
                {
                    combatController.OnKilledEnemy(monster.GetExperienceForKillingThisUnit());
                }
            }
            //Check to see if the target has a 'MonsterController' component (Indicating that its an enemy player) and then check that monster controller to see if your attack has killed it
        }
    }

    private protected virtual void HandleNPCCombat(Collider other)
    {
        NPCController npc = other.GetComponent<NPCController>();

        StartCoroutine(ImmunityTimer(npc.gameObject.GetComponent<PhotonView>().ViewID));

        npc.UpdatePlayerHostility(combatController.GetPlayerController());

        other.GetComponent<IDamageable>().TakeDamage(criticalAttackPoint ? combatController.GetCurrentBasicAttack().GetDamage() * 2 : combatController.GetCurrentBasicAttack().GetDamage());

        return;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    IEnumerator ImmunityTimer(int photonViewID)
    {

        combatController.GetCurrentBasicAttack().AddCollider(photonViewID);

        yield return new WaitForSeconds(immuneTime);

        combatController.GetCurrentBasicAttack().RemoveCollider(photonViewID);

        //This coroutine ensures that enemies aren't accidentally hit with the same punch multiple times. 
    }
}
