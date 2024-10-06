using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.Services.Analytics;
using System.Diagnostics.Contracts;

public class MonsterController : MonoBehaviour
{
    [SerializeField] private PlayerStats[] monsterStatLines;
    [SerializeField] private EvolutionStats[] evolutionStatLines;
    //These arrays will contain the data needed for a monster to properly evolve
    //The data includes things like the monster model, the player controller dimensions, camera and nameplate height, etc


    private PlayerStats currentMonsterStats;
    private EvolutionStats currentEvolutionData;
    //These variables will store the currently relevant data set from the above array

    private int currentHealth;
    private int currentEvolutionExperience;

    public event Action OnPlayerHealthChanged;
    public event Action OnPlayerExperienceChanged;
    public event Action OnPlayerEvolved;
    public event Action OnPlayerDied;
    //The PlayerController and the UI will need to know when certain important things happen and by subscribing to these events
    //This way, the MonsterController does not need to 'know about' the PlayerController

    [SerializeField] private GameObject[] evolutionModels;
    private CharacterController characterController;
    private PlayerController playerController;
    private PhotonView PV;

    private bool reducingDamage;
    private float damageReductionPercentage;

    private int currentEvolutionLevel = 0;

    [SerializeField] private bool testing;

    private bool deathRegistered = false;

    bool recentlyDamaged;
    [SerializeField ]int regenAmountPerSecond = 2;
    float regenTimer;
    [SerializeField] float timeToRegen;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
        //All controller Scripts MUST have a reference to the Player Controller
    }

    public void SetPhotonView(PhotonView PV)
    {
        this.PV = PV;
    }

    private void Update()
    {
        if(!PV.IsMine)
        {
            return;
        }
        if(testing && Input.GetKeyDown(KeyCode.T))
        {
            GainExperience(500);
        }
        if(recentlyDamaged)
        {
            CancelInvoke();
            regenTimer += Time.deltaTime;
        }
        if(regenTimer > timeToRegen && currentHealth < currentMonsterStats.maxHealth && !playerController.deathRegistered)
        {
            recentlyDamaged = false;
            regenTimer = 0;
            InvokeRepeating("RegenHealth", 0, 1f);
        }
    }

    public void GainExperience(int evolutionExperience)
    {
        currentEvolutionExperience += evolutionExperience;
        if (currentEvolutionExperience > currentMonsterStats.evolutionCost)
        {
            currentEvolutionExperience = currentMonsterStats.evolutionCost;
            //No reason for the evolution experience to be able to exceed the amount needed to evolve
            //No banking XP for the next evolution stage
        }
        OnPlayerExperienceChanged?.Invoke();
        //Let the UI know it needs to update
    }

    public void HandleEvolution()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Evolve();
        }
    }

    #region Photon Events

    public void SendPlayerDiedEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent(EventCodes.PlayerDiedEventCode, PV.ViewID, raiseEventOptions, sendOptions);

        deathRegistered = true;
    }

    #endregion

    #region RPC Calls

    public void Evolve()
    {
        PV.RPC("RPC_Evolve", RpcTarget.AllBuffered);

        OnPlayerHealthChanged?.Invoke();
        OnPlayerEvolved?.Invoke();
        OnPlayerExperienceChanged?.Invoke();
        //A couple of important things happen when a player evolves so we invoke all the requisite events
    }

    private void UpdateStats()
    {
        PV.RPC("RPC_UpdateStats", RpcTarget.AllBuffered);
    }

    private void UpdateEvolutionData()
    {
        PV.RPC("RPC_UpdateEvolutionData", RpcTarget.AllBuffered);
    }

    private void UpdatePlayerModel()
    {
        PV.RPC("RPC_UpdatePlayerModel", RpcTarget.AllBuffered);
    }

    private void UpdateAnimator()
    {
        PV.RPC("RPC_UpdateCharacterAnimator", RpcTarget.AllBuffered);
    }
    public void CreatePlayerStats()
    {
        PV.RPC("RPC_CreatePlayerStats", RpcTarget.All);
        if(PV.IsMine)
        {
            playerController.SetupUI();
            if(playerController.CheckIsRanged())
            {
                playerController.ActivateCrosshairUIElement();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
        playerController.OnPlayerDamaged();
    }

    public void HealDamage(int healing)
    {
        PV.RPC("RPC_HealDamage", RpcTarget.All, healing);
    }

    public void RegenHealth()
    {
        PV.RPC("RPC_HealDamage", RpcTarget.All, regenAmountPerSecond);
    }

    public void SetDamageReduction(float damageReduction)
    {
        PV.RPC("RPC_SetDamageReduction", RpcTarget.All, damageReduction);
    }

    public void ClearDamageReduction()
    {
        PV.RPC("RPC_ClearDamageReduction", RpcTarget.All);
    }

    #endregion

    #region 'Check' Functions

    public int CheckCurrentHealth()
    {
        return currentHealth;
    }

    public int CheckEvolutionExperience()
    {
        return currentEvolutionExperience;
    }

    public bool CheckDeath()
    {
        if (currentHealth <= 0)
        {
            if (!playerController.deathRegistered && PV.IsMine)
            {
                SendPlayerDiedEvent();
            }
            return true;

        }
        else return false;

        //The actual implementation of death will happen elsewhere, checking for death can be an important tool for other functions interacting with the monster manager
    }

    public bool CheckCanEvolve()
    {
        if (currentEvolutionExperience == currentMonsterStats.evolutionCost && currentEvolutionLevel < 2)

        {
            return true;
            //if the monster is currently at max experience points and theyre not already at max level, then they can evolve
        }
        else return false;
        //If not, they cant evolve yet
    }

    public bool CheckDeathRegistered()
    {
        return deathRegistered;
    }

    #endregion

    #region 'Get' Functions

    public PlayerStats GetCurrentMonsterStats()
    {
        return currentMonsterStats;
    }

    public EvolutionStats GetCurrentEvolutionStats()
    {
        return currentEvolutionData;
    }

    public int GetExperienceForKillingThisUnit()
    {
        return currentMonsterStats.experienceOnKill;
    }

    public int GetCurrentEvolutionLevel()
    {
        return currentEvolutionLevel;
    }

    #endregion 

    #region RPCs

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        if(reducingDamage)
        {
            damage = (int)MathF.Round(damage * damageReductionPercentage);
        }
        currentHealth -= damage;
        PV.RPC("RPC_ToggleRecentlyDamaged", RpcTarget.All);
        if(currentHealth <= 0)
        {
            currentHealth = 0;
        }
        OnPlayerHealthChanged?.Invoke();
        if(CheckDeath())
        {
            PV.RPC("RPC_Die", RpcTarget.All);
        }
        //Simple method that takes an int as a parameter and then subtracts that parameter from the monster's current health
        //Because the player's health has now changed, we'll alert any other methods that might be interested in that change (UI)
    }

    [PunRPC]
    void RPC_ToggleRecentlyDamaged()
    {
        recentlyDamaged = true;
    }

    [PunRPC]
    void RPC_HealDamage(int healing)
    {
        currentHealth += healing;
        if (currentHealth > currentMonsterStats.maxHealth)
        {
            currentHealth = currentMonsterStats.maxHealth;
        }
        OnPlayerHealthChanged?.Invoke();
        //Same method as above, but for healing. the check to make sure the health value doesn't go above its maximum is much more important here
    }

    [PunRPC]
    void RPC_Evolve()
    {
        Debug.Log(currentEvolutionLevel);

        currentEvolutionLevel += 1;
        Debug.Log(currentEvolutionLevel);
        UpdateStats();
        UpdateEvolutionData();
        UpdatePlayerModel();
        UpdateAnimator();


        OnPlayerEvolved?.Invoke();
    }

    [PunRPC]
    void RPC_UpdateStats()
    {
        int h = currentHealth;
        int a = currentMonsterStats.maxHealth;
        //First, save the players current health and their max health before evolving
        //This will be important in calculating their health after evolving, we dont want players to heal by evolving

        currentMonsterStats = monsterStatLines[currentEvolutionLevel];
        //Now we've evolved and our statline has been updated

        currentHealth = h + (currentMonsterStats.maxHealth - a);
        //In order to get the monster's post evolution health, we take the health we had before and add the difference in our new max health and previous max health

        currentEvolutionExperience = 0;

        if (PV.IsMine)
        {
            OnPlayerHealthChanged?.Invoke();
            OnPlayerExperienceChanged?.Invoke();
        }

        Debug.Log(currentEvolutionLevel + PV.ViewID);
    }

    [PunRPC]
    void RPC_UpdateEvolutionData()
    {
        currentEvolutionData = evolutionStatLines[currentEvolutionLevel];
        StartCoroutine(currentEvolutionData.characterControllerLerp(characterController));
    }

    [PunRPC]
    void RPC_UpdatePlayerModel()
    {

        if (currentEvolutionLevel - 1 >= 0 && currentEvolutionLevel - 1 < evolutionModels.Length)
        {
            evolutionModels[currentEvolutionLevel - 1].SetActive(false);
        }

        if (currentEvolutionLevel >= 0 && currentEvolutionLevel < evolutionModels.Length)
        {
            evolutionModels[currentEvolutionLevel].SetActive(true);
        }
    }

    [PunRPC]
    void RPC_UpdateCharacterAnimator()
    {

        playerController.UpdateAnimationController(currentEvolutionLevel);
        //Change the animator that the Aniamtion controller is looking at
        //We want to separate aesthetic from logic here so each animator will be placed on the character it is animating
    }

    [PunRPC]
    void RPC_Die()
    {
        OnPlayerDied?.Invoke();
    }

    [PunRPC]
    void RPC_SetDamageReduction(float damageReductionPercentage)
    {

        reducingDamage = true;
        this.damageReductionPercentage = damageReductionPercentage;
    }

    [PunRPC]
    void RPC_ClearDamageReduction()
    {

        reducingDamage = false;
        damageReductionPercentage = 0;
    }

    [PunRPC]
    void RPC_CreatePlayerStats()
    {
        currentMonsterStats = monsterStatLines[0];
        currentEvolutionData = evolutionStatLines[0];
        //This method is only called once when the game starts, it sets up all the monster stats to what they should be for a fresh game

        currentHealth = currentMonsterStats.maxHealth;
        currentEvolutionExperience = 0;
        //New game means starting with a monster's max health and 0 progress towards the next evolution

        Debug.Log("Player Stats created");
    }
    #endregion
}
