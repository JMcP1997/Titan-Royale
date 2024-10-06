using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip[] deathSounds;
    private AudioClip currentDeathSound;

    [SerializeField] AudioClip[] basicAttackSounds_Stage1;
    [SerializeField] AudioClip[] basicAttackSounds_Stage2;
    [SerializeField] AudioClip[] basicAttackSounds_Stage3;

    private AudioClip[] currentActiveBasicAttackSoundArray;

    private AudioClip currentBasicAttackSound;
    private int basicAttackSoundIndex = 0;

#nullable enable
    [SerializeField] AudioClip[]? AbilitySounds_Q;
    private AudioClip? currentAbilitySound_Q;

    [SerializeField] AudioClip[]? AbilitySounds_E;
    private AudioClip? currentAbilitySound_E;
#nullable disable

    [SerializeField] AudioClip damagedSound;

    [SerializeField] AudioClip victorySound;

    private PhotonView PV;
    private PlayerController playerController;

    private void Awake()
    {
        currentActiveBasicAttackSoundArray = basicAttackSounds_Stage1;
        currentBasicAttackSound = currentActiveBasicAttackSoundArray[0];
        currentDeathSound = deathSounds[0];
    }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        playerController.GetMonsterController().OnPlayerDied += MonsterController_OnPlayerDied;

        playerController.GetMonsterController().OnPlayerEvolved += MonsterController_OnPlayerEvolved;

        if (AbilitySounds_E[0] != null)
        {
            currentAbilitySound_E = AbilitySounds_E[0];
        }

        if (AbilitySounds_Q[0] != null)
        {
            currentAbilitySound_Q = AbilitySounds_Q[0];
        }

    }

    public void SetPhotonView(PhotonView PV)
    {
        this.PV = PV;
    }

    public void PlayVictorySound()
    {
        audioSource.clip = victorySound;
        audioSource.Play();
    }

    #region RPC Calls

    public void OnPlayerDamaged()
    {
        PV.RPC("RPC_PlayDamagedSound", RpcTarget.All);
    }

    private void MonsterController_OnPlayerEvolved()
    {
        PV.RPC("RPC_UpdateSoundClips", RpcTarget.All);
    }

    private void MonsterController_OnPlayerDied()
    {
        PV.RPC("RPC_PlayDeathSound", RpcTarget.All);
    }

    public void OnPlayerUsedAbility_Q()
    {
        PV.RPC("RPC_PlayAbilitySound_Q", RpcTarget.All);
    }

    public void OnPlayerUsedAbility_E()
    {
        PV.RPC("RPC_PlayAbilitySound_E", RpcTarget.All);
    }

    public void OnPlayerBasicAttack()
    {
        PV.RPC("RPC_OnPlayerBasicAttack", RpcTarget.All);
    }

    #endregion

    #region RPCs

    [PunRPC]
    private void RPC_PlayDamagedSound()
    {
        audioSource.clip = damagedSound;
        audioSource.Play();
    }

    [PunRPC]
    void RPC_UpdateSoundClips()
    {
        int index = playerController.GetCurrentEvolutionLevel();

        if (AbilitySounds_E[index] != null)
        {
            currentAbilitySound_E = AbilitySounds_E[index];
        }
        else currentAbilitySound_E = null;
        if (AbilitySounds_Q[index] != null)
        {
            currentAbilitySound_Q = AbilitySounds_Q[index];
        }
        else currentAbilitySound_Q = null;
        switch(index)
        {
            case 1:
                if(basicAttackSounds_Stage2 != null)
                {
                    currentActiveBasicAttackSoundArray = basicAttackSounds_Stage2;
                    break;
                }
                else break;


            case 2:
                if(basicAttackSounds_Stage3 != null)
                {
                    currentActiveBasicAttackSoundArray = basicAttackSounds_Stage3;
                    break;
                }
                else break;


            default: 
                Debug.Log("MISTAKE AT LINE 137 ON PLAYER SOUND CONTROLLER");
                break;
        }

        currentDeathSound = deathSounds[index];
    }

    [PunRPC]
    void RPC_PlayDeathSound()
    {
        audioSource.clip = currentDeathSound;
        audioSource.Play();
    }

    [PunRPC]
    void RPC_OnPlayerBasicAttack()
    {
        currentBasicAttackSound = currentActiveBasicAttackSoundArray[basicAttackSoundIndex];
        audioSource.clip = currentBasicAttackSound;

        audioSource.Play();

        basicAttackSoundIndex++;
        if (basicAttackSoundIndex >= currentActiveBasicAttackSoundArray.Length)
        {
            basicAttackSoundIndex = 0;
        }
    }

    [PunRPC]
    void RPC_PlayAbilitySound_Q()
    {
        if(currentAbilitySound_Q != null)
        {
            audioSource.clip = currentAbilitySound_Q;
            audioSource.Play();
        }
    }

    [PunRPC]
    void RPC_PlayAbilitySound_E()
    {
        if (currentAbilitySound_E != null)
        {
            audioSource.clip = currentAbilitySound_E;
            audioSource.Play();
        }
    }

    #endregion
}
