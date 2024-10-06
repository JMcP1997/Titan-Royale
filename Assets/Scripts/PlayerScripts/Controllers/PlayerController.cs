using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics.Platform;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Player Controllers")]
    [SerializeField] PlayerMovementController movementController;
    [SerializeField] PlayerAnimationController animationController;
    [SerializeField] PlayerCombatController combatController;
    [SerializeField] MonsterController monsterController;
    [SerializeField] PlayerSoundController soundController;
    [SerializeField] PlayerNameText playerNameTextController;
    [SerializeField] PlayerCameraController cameraController;


    [SerializeField] float[] namePlateHeights;

    private PlayerUI playerUIController;
    //These references are all the various scripts over which PlayerController has control

    private bool isAlive = true;
    private bool canMove = true;
    private bool canAct = true;
    private bool canBeStunned = true;

    [SerializeField] float stunImmunityTimer = 3.5f;

    [SerializeField] private bool rangedCharacter = false;

    //These booleans will be used to control what actions can and can't be taken at a given time
    //For instance the canMove will govern movement but the canAct will be a catch-all boolean that prevents any input of any kind

    [SerializeField] TextMeshPro playerNameText;

    //These are the non-script components that the PlayerController will need to know
    //Some components are used by mulitple classes that the PlayerController uses, so to keep things simple, they will all pull their references from this class

    private CharacterController characterController;
    private PhotonView PV;

    public bool deathRegistered;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        characterController = GetComponent<CharacterController>();

        SetControllersPV(PV);
    }

    private void Start()
    {
        monsterController.CreatePlayerStats();

        // Locks the cursor to the center of the screen and makes it disappear while playing the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        monsterController.OnPlayerDied += MonsterController_PlayerDied;
    }

    private void FixedUpdate()
    {

        //Movement input goes into FixedUpdate to ensure that all movement is framerate independent
        if(!PV.IsMine)
        {
            return;
            //if the PhotonView does not belong to you, ignore all input
        }
        if(!isAlive)
        {
            return;
            //If your character has died, ignore all input
            //isAlive and canAct have been separated for the implementation of stuns in combat and so that unique functionality can be applied on death
        }

        HandleMovement();
        HandleJumping();


    }

    private void Update()
    {
        if(!PV.IsMine || !isAlive)
        {
            return;
        }

        cameraController.HandleCameraRotation();

        if(canMove)
        {
            HandleBasicMovementAnimation();
        }

        if (canAct)
        {
            HandleCombatAnimations();
            HandleCombat();
            if(CheckCanEvolve())
            {
                HandleEvolution();
            }
        }
    }

    #region Pass-Through Functions
    //This script is the central hub for all player control, so when calling a function here, we want to reach into the appropriate controller
    private void HandleMovement() => movementController.HandleMovement();

    private void HandleJumping() => movementController.HandleJumping();

    public void TakeDamage(int damage) => monsterController.TakeDamage(damage); 

    public void HealDamage(int healing) => monsterController.HealDamage(healing);

    public void GainExperience(int experienceGained) => monsterController.GainExperience(experienceGained);

    public int GetCurrentEvolutionLevel() => monsterController.GetCurrentEvolutionLevel();

    public bool CheckCanEvolve() => monsterController.CheckCanEvolve();

    public bool CheckDeathRegistered() => monsterController.CheckDeathRegistered();

    public void HandleEvolution() => monsterController.HandleEvolution();

    public void UpdatePlayerNameText(string newName) => playerNameTextController.UpdatePlayerNameText(newName);

    public void UpdateAnimationController(int currentEvolutionLevel) => animationController.UpdateAnimationController(currentEvolutionLevel);

    public void HandleBasicMovementAnimation() => animationController.HandleBasicMovement();

    public void HandleCombatAnimations() => animationController.HandleCombatAnimations();

    public void HandleCombat() => combatController.HandleCombatAbilities();

    public bool CheckIsGrounded() => movementController.CheckIsGrounded();

    public bool CheckCanQ() => combatController.CheckCanQ();

    public bool CheckCanE() => combatController.CheckCanE();

    public bool CheckCanBasicAttack() => combatController.CheckCanBasicAttack();

    public void SetupUI() => playerUIController.SetupUI();

    public void SetupIcons() => playerUIController.CreateAbilityIcons();

    public void ActivateCrosshairUIElement() => playerUIController.ActivateCrosshairUIElement();

    public List<MonsterActiveAbility> GetCurrentActiveAbilities() => combatController.GetListOfCurrentActiveAbilities();

    public void OnPlayerUsedAbility_Q() => soundController.OnPlayerUsedAbility_Q();

    public void OnPlayerUsedAbility_E() => soundController.OnPlayerUsedAbility_E();

    public void OnPlayerBasicAttack() => soundController.OnPlayerBasicAttack();

    public void PlayVictorySound() => soundController.PlayVictorySound();

    public CinemachineVirtualCamera GetCurrentVCam() => cameraController.GetCurrentVCam();

    public void ToggleCameraLocked(bool toggle) => cameraController.ToggleCameraLockPosition(toggle);

    public void ClearCooldown(MonsterActiveAbility abilityToClear) => playerUIController.ClearCooldown(abilityToClear);

    public void HideNamePlate() => playerNameTextController.HideNameText();

    public void ShowNamePlate() => playerNameTextController.ShowNameText();

    #endregion

    #region Get Controller Functions

    public MonsterController GetMonsterController()
    {
        return monsterController;
    }

    public PlayerMovementController GetMovementController()
    {
        return movementController;
    }

    public PlayerAnimationController GetAnimationController()
    {
        return animationController;
    }

    public PlayerCombatController GetCombatController()
    {
        return combatController;
    }

    public PhotonView GetPhotonView()
    {
        return PV;
    }

    public CharacterController GetCharacterController()
    {
        return characterController;
    }

    public PlayerCameraController GetCameraController()
    {
        return cameraController;
    }

    public PlayerUI GetUIController()
    {
        return playerUIController;
    }
    #endregion

    #region Get Booleans

    public bool CheckCanMove()
    {
        return canMove;
    }

    public bool CheckCanAct()
    {
        return canAct;
    }

    public bool CheckIsAlive()
    {
        return isAlive;
    }

    public bool CheckIsRanged()
    {
        return rangedCharacter;
    }

    #endregion

    #region 'Set' Functions

    public void SetCanAct(bool canAct)
    {
        this.canAct = canAct;
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public void SetPlayerUI(PlayerUI playerUI)
    {
        playerUIController = playerUI;
    }

    private void SetControllersPV(PhotonView PV)
    {
        movementController.SetPhotonView(PV);
        animationController.SetPhotonView(PV);
        combatController.SetPhotonView(PV);
        monsterController.SetPhotonView(PV);
        soundController.SetPhotonView(PV);
        cameraController.SetPhotonView(PV);
    }

    #endregion

    #region RPCs

    [PunRPC]
    private void RPC_OnPlayerDied()
    {
        isAlive = false;
        characterController.enabled = false;
        //We don't want character movement to be a thing anymore
    }

    [PunRPC]
    private void RPC_OnPlayerStunned(float stunTime)
    {
        combatController.DisableCurrentHitboxes();
        StartCoroutine("StunPlayer", stunTime);
        StartCoroutine("StunImmunity");
        animationController.HandleStunAnimation(stunTime);
    }

    #endregion

    public void OnPlayerDamaged()
    {
        soundController.OnPlayerDamaged();
        animationController.SetDamagedTrigger();
    }

    private void MonsterController_PlayerDied()
    {
        PV.RPC("RPC_OnPlayerDied", RpcTarget.All);
    }

    public void OnPlayerStunned(float stunTime)
    {
        if(canBeStunned)
        {
            PV.RPC("RPC_OnPlayerStunned", RpcTarget.All, stunTime);
        }
    }

    IEnumerator StunPlayer(float stunDuration)
    {
        canMove = false;
        canAct = false;

        yield return new WaitForSeconds(stunDuration);

        canMove = true;
        canAct = true;
    }

    IEnumerator StunImmunity()
    {
        canBeStunned = false;
        yield return new WaitForSeconds(stunImmunityTimer);
        canBeStunned = true;
    }

    public void SetCanBeStunned(bool canBeStunned)
    {
        PV.RPC("RPC_SetCanBeStunned", RpcTarget.All, canBeStunned);
    }

    [PunRPC]
    private void RPC_SetCanBeStunned(bool canBeStunned)
    {
        this.canBeStunned = canBeStunned;
    }

}

