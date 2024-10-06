using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IDamageable
{
    [SerializeField] float maxHealth;
    private float currentHealth;

    [SerializeField] float experienceGivenOnKill;

    private bool isDead = false;

    [SerializeField] EnemySimpleFSM sourceStateMachine;
    [SerializeField] NPCCombatController combatController;
    [SerializeField] Animator npcAnimator;

    private bool hasGivenXP = false;

    
    private List<PlayerController> attackingPlayers = new List<PlayerController>();

    PhotonView PV;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        currentHealth = maxHealth;
    }

    public void ClearHostility()
    {
        attackingPlayers.Clear();
    }

    #region 'Get' + 'Check' Methods

    public List<PlayerController> GetAttackingPlayers()
    {
        return attackingPlayers;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public NPCCombatController GetCombatController()
    {
        return combatController;
    }

    public bool CheckDead()
    {
        return isDead;
    }

    #endregion

    #region RPC Calls

    public void UpdatePlayerHostility(PlayerController playerController)
    {
        int photonViewID = playerController.GetPhotonView().ViewID;
        if(PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPC_UpdatePlayerHostility", RpcTarget.AllBuffered, photonViewID);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PV.RPC("RPC_NPCTakeDamage", RpcTarget.AllBuffered, damageAmount);
        }
    }

    public void HealDamage(int healingAmount)
    {
        PV.RPC("RPC_NPCHealDamage", RpcTarget.AllBuffered, healingAmount);
    }
    #endregion

    #region RPCs

    [PunRPC]
    private void RPC_NPCTakeDamage(int damage)
    {
        currentHealth -= damage;

        // npcAnimator.SetBool("isAttacking", false);

        npcAnimator.SetTrigger("isDamaged");
     
        if (currentHealth <= 0)
        {
            combatController.DeactivateAttackPoints();
            npcAnimator.SetTrigger("isDead");
            sourceStateMachine.currentState = EnemySimpleFSM.FSMState.Dead;
            if(!hasGivenXP)
            {
                sourceStateMachine.GetTargetPlayer().GainExperience((int)experienceGivenOnKill);
                hasGivenXP = true;
            }
            sourceStateMachine.GetTargetPlayer().GetCombatController().GetCurrentBasicAttack().RemoveCollider(PV.ViewID);
            isDead = true;
        }
        else
        {
            sourceStateMachine.currentState = EnemySimpleFSM.FSMState.Attack;
        }


    }

    [PunRPC]
    private void RPC_UpdatePlayerHostility(int photonViewID)
    {
        PlayerController attackingPlayer = PhotonView.Find(photonViewID).gameObject.GetComponent<PlayerController>();
        attackingPlayers.Add(attackingPlayer);

        sourceStateMachine.SetTargetPlayer(photonViewID);
    }

    [PunRPC]
    private void RPC_NPCHealDamage(int healingAmount)
    {
        currentHealth += healingAmount;
    }

    #endregion
}
