using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] MonsterActiveAbility[] QAbilities;
    [SerializeField] MonsterActiveAbility[] EAbilities;

    [SerializeField] BasicAttack[] BasicAttacks;


    private MonsterActiveAbility currentQAbility;
    private MonsterActiveAbility currentEAbility;

    private List<MonsterActiveAbility> currentActiveAbilities = new List<MonsterActiveAbility>();

    private BasicAttack currentBasicAttack;

    bool canUseQAbility = true;
    bool canUseEAbility = true;
    bool canBasicAttack = true;

    private PlayerController playerController;

    private PhotonView PV;

    public event EventHandler<MonsterActiveAbility> onAbilityUsed;
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        //Every controller script MUST have a reference to the PlayerController script

        playerController.GetMonsterController().OnPlayerEvolved += MonsterController_OnPlayerEvolved;
        //The Combat controller will keep track of what the current evolution's moveset is, so it will need to know when the player evolves so it can change said moveset

        playerController.GetMonsterController().OnPlayerDied += MonsterController_OnPlayerDied;

        currentQAbility = QAbilities[0];
        currentEAbility = EAbilities[0];
        currentBasicAttack = BasicAttacks[0];

        currentActiveAbilities.Add(currentQAbility);
        currentActiveAbilities.Add(currentEAbility);

        if(PV.IsMine)
        {
            playerController.SetupIcons();
        }
    }

    public void SetPhotonView(PhotonView PV)
    {
        this.PV = PV;
    }

    #region Use Abilities

    public void HandleCombatAbilities()
    {
        if(Input.GetKeyDown(KeyCode.Q) && canUseQAbility)
        {
            if (currentQAbility.CheckToggleable() && !currentQAbility.CheckToggleReady())
            {
                return;
            }
            UseQAbility();
            onAbilityUsed.Invoke(this, currentQAbility);
        }

        if(Input.GetKeyDown(KeyCode.E) && canUseEAbility)
        {
            if (currentEAbility.CheckToggleable() && !currentEAbility.CheckToggleReady())
            {
                return;
            }
            UseEAbility();
            onAbilityUsed.Invoke(this, currentEAbility);
        }

        if(Input.GetMouseButtonDown(0) && canBasicAttack)
        {
            UseBasicAttack();
            playerController.OnPlayerBasicAttack();
        }
    }
    public void UseQAbility()
    {
        currentQAbility.StartAbility();
        if (!currentQAbility.CheckToggleable())
        {
            StartCoroutine(Q_AbilityCooldown());
            Debug.Log("Starting Q Ability Cooldown");
        }
    }

    public void UseEAbility()
    {
        currentEAbility.StartAbility();
        if(!currentEAbility.CheckToggleable())
        {
            StartCoroutine(E_AbilityCooldown());
        }
    }

    //Some abilities wont have a hard cooldown, they'll be freely togglable on/off
    //If the ability is NOT togglable, that means it should immediately start its cooldown
    //If it is, cooldown will be handled inside the ability itself

    public void UseBasicAttack()
    {
        currentBasicAttack.OnAttackStart();
        StartCoroutine(BasicAttackCooldown());
    }

    #endregion

    private void MonsterController_OnPlayerEvolved()
    {
        //Any combat functionality that changes when a player evolves should happen here

        PV.RPC("RPC_UpdateMoveset", RpcTarget.All);
        UpdateAbilityList();
        if(PV.IsMine)
        {
            playerController.SetupIcons();
        }
    } 
    
    private void MonsterController_OnPlayerDied()
    {
        DisableCurrentHitboxes();

        //To ensure players cannot damage other players after they've died, this line of code ensures that all
        //Ability points will disable themselves as soon as the OnPlayerDied event is invoked

        //Note: The basic attack solution is crude, but creating a more elegant solution is not a high priority.
    }

    public void SwitchAbilitiesUsable(bool usability)
    {
        canUseQAbility = usability;
        canBasicAttack = usability;
    }

    public void OnKilledEnemy(int experienceGained)
    {
        playerController.GainExperience(experienceGained);
    }

    public void DisableCurrentHitboxes()
    {
        currentQAbility.OnAbilityCompleted();
        currentEAbility.OnAbilityCompleted();
        currentBasicAttack.OnAttackEnd(0);
        currentBasicAttack.OnAttackEnd(1);
    }

    private void UpdateAbilityList()
    {
        currentActiveAbilities.Clear();
        currentActiveAbilities.Add(currentQAbility);
        currentActiveAbilities.Add(currentEAbility);
    }

    #region Comabt Cooldowns

    IEnumerator Q_AbilityCooldown()
    {
        PV.RPC("RPC_ToggleQAbility", RpcTarget.All);

        yield return new WaitForSeconds(currentQAbility.GetAbilityCooldown());

        if(currentQAbility.CheckToggleable())
        {
            currentQAbility.SetToggleReady(true);
        }

        PV.RPC("RPC_ToggleQAbility", RpcTarget.All);
    }

    IEnumerator E_AbilityCooldown()
    {
        PV.RPC("RPC_ToggleEAbility", RpcTarget.All);

        yield return new WaitForSeconds(currentEAbility.GetAbilityCooldown());

        if(currentEAbility.CheckToggleable())
        {
            currentEAbility.SetToggleReady(true);
        }

        PV.RPC("RPC_ToggleEAbility", RpcTarget.All);
    }

    IEnumerator BasicAttackCooldown()
    {
        PV.RPC("RPC_ToggleBasicAttack", RpcTarget.All);

        yield return new WaitForSeconds(currentBasicAttack.GetAttackRate());

        PV.RPC("RPC_ToggleBasicAttack", RpcTarget.All);
    }

    public void ClearCooldown(MonsterActiveAbility abilityToReset)
    {
        if(abilityToReset == currentQAbility)
        {
            canUseQAbility = true;

            if (PV.IsMine)
            {
                playerController.ClearCooldown(currentQAbility);
            }
        }
        if(abilityToReset = currentEAbility)
        {
            canUseEAbility = true;

            if (PV.IsMine)
            {
                playerController.ClearCooldown(currentEAbility);
            }
        }

        Debug.Log("Cooldown Cleared");

    }

    #endregion

    #region Get Functions

    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    public BasicAttack GetCurrentBasicAttack()
    {
        return currentBasicAttack;
    }

    public MonsterActiveAbility GetCurrentQAbility()
    {
        return currentQAbility;
    }

    public MonsterActiveAbility GetCurrentEAbility()
    {
        return currentEAbility;
    }

    public List<MonsterActiveAbility> GetListOfCurrentActiveAbilities()
    {
        return currentActiveAbilities;
    }

    #endregion

    #region Check Functions

    public bool CheckCanQ()
    {
        return canUseQAbility;
    }

    public bool CheckCanE()
    {
        return canUseEAbility;
    }

    public bool CheckCanBasicAttack()
    {
        return canBasicAttack;
    }

    #endregion

    #region RPCs

    [PunRPC]
    void RPC_UpdateMoveset()
    {

        int index = playerController.GetCurrentEvolutionLevel();

        currentQAbility = QAbilities[index];
        currentEAbility = EAbilities[index];
        currentBasicAttack = BasicAttacks[index];
       
    }

    [PunRPC]
    void RPC_ToggleBasicAttack()
    {
        canBasicAttack = !canBasicAttack;
    }

    [PunRPC]
    void RPC_ToggleQAbility()
    {
        canUseQAbility = !canUseQAbility;
    }

    [PunRPC]
    void RPC_ToggleEAbility()
    {
        canUseEAbility = !canUseEAbility;
    }

    #endregion
}
