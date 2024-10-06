using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

public class PlayerUI : MonoBehaviour, IOnEventCallback
{


    [SerializeField] Slider playerHealthSlider;
    [SerializeField] TMP_Text playerHealthText;

    //[SerializeField] Slider playerStaminaSlider;
    //Re-enable if/when stamina is implemented

    [SerializeField] Slider playerEvolutionSlider;
    [SerializeField] TMP_Text evolutionReadyText;

    [SerializeField] Transform abilityIconContainer;
    [SerializeField] GameObject abilityIconPrefab;
    private List<AbilityIcon> icons = new List<AbilityIcon>();
    List<MonsterActiveAbility> currentMonsterAbilities = new List<MonsterActiveAbility>();

    [SerializeField] GameObject gameOverPanel;
    public Button gameOverButton;

    [SerializeField] GameObject victoryPanel;
    public Button victoryButton;

    [SerializeField] GameObject killFeedContainer;
    [SerializeField] TextMeshProUGUI killFeedText;

    [SerializeField] GameObject pauseGamePanel;
    public Button leaveGameButton;
    private bool pauseGamePanelOpen = false;

    [SerializeField] GameObject uiCrosshair;

    private MonsterController targetMonsterController;
    private PlayerController targetPlayerController;

    public delegate void setButtonFunctionDelegate();
    private setButtonFunctionDelegate buttonListener;

    private CinemachineVirtualCamera vcam;

    private bool canSwitchMenus = true;

    [SerializeField] AudioSource gameMusic;
    [SerializeField] GameObject gameMusicSlider;
    [SerializeField] GameObject mouseSensitivitySlider;

    [SerializeField] GameObject controlsContainer;
    bool controlPanelEnabled = false;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && canSwitchMenus)
        {
            SwitchPauseMenu();
            targetPlayerController.SetCanAct(!targetPlayerController.CheckCanAct());
            targetPlayerController.SetCanMove(!targetPlayerController.CheckCanMove());
        }

    }

    public void SetTarget(PlayerController targetPlayer)
    {
        if (targetPlayer == null)
        {
            Debug.Log("Missing target player");
            return;
        }

        targetPlayerController = targetPlayer;
        targetMonsterController = targetPlayer.GetMonsterController();

        targetPlayerController.SetPlayerUI(this);

        targetMonsterController.OnPlayerHealthChanged += UpdateHealthUI;
        targetMonsterController.OnPlayerExperienceChanged += UpdateExperienceUI;
        targetMonsterController.OnPlayerEvolved += UpdateSliders;
        targetMonsterController.OnPlayerEvolved += MonsterController_OnPlayerEvolved;

        targetPlayerController.GetCombatController().onAbilityUsed += UpdateIconCooldownIndicator;



    }

    /// <summary>
    /// ^ This is the most important function in this class. When a player gets instantiated into the level, their UI gets instantiated locally alongside them.
    /// The UI then immediately designates the player whose stats they need to reflect, and changes the UI values to be the initial stat values specified by that class
    /// After that, it subscribes all the specific update functions to the corresponding stat-change event.
    /// 
    /// NOTE: IF ANY NEW STATS GET ADDED THAT NEED TO BE REFLECTED IN THE UI, A NEW EVENT MUST BE MADE AND A NEW FUNCTION TO SUBSCRIBE TO IT.
    /// </summary>

    public void SetupUI()
    {
        playerHealthSlider.maxValue = targetMonsterController.GetCurrentMonsterStats().maxHealth;
        playerHealthSlider.value = targetMonsterController.CheckCurrentHealth();
        playerHealthText.text = playerHealthSlider.value.ToString();

        playerEvolutionSlider.maxValue = targetMonsterController.GetCurrentMonsterStats().evolutionCost;
        playerEvolutionSlider.value = targetMonsterController.CheckEvolutionExperience();

    }

    public void ActivateCrosshairUIElement()
    {
        uiCrosshair.SetActive(true);
    }

    private void UpdateExperienceUI()
    {
        playerEvolutionSlider.value = targetMonsterController.CheckEvolutionExperience();
        if(targetMonsterController.CheckEvolutionExperience() >= targetMonsterController.GetCurrentMonsterStats().evolutionCost)
        {
            evolutionReadyText.gameObject.SetActive(true);
            Debug.Log("Evolution Activated");
        }
        else
        {
            evolutionReadyText.gameObject.SetActive(false);
        }

        // thing to deactivate the text
        if((targetMonsterController.GetCurrentMonsterStats().currentEvolutionStage >= 2))
        {
            evolutionReadyText.gameObject.SetActive(false);
            playerEvolutionSlider.gameObject.SetActive(false);
        }

    }

    private void UpdateHealthUI()
    {
        playerHealthSlider.value = targetMonsterController.CheckCurrentHealth();
        playerHealthText.text = targetMonsterController.CheckCurrentHealth().ToString();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        targetMonsterController.OnPlayerHealthChanged -= UpdateHealthUI;
        targetMonsterController.OnPlayerExperienceChanged -= UpdateExperienceUI;

        //Always remember to unsubscribe from events. In the event of a player death, this gameObject will likely be destroyed, but if for some reason it is not
        //We dont want unusuable subscribers

        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void UpdateSliders()
    {
        playerHealthSlider.maxValue = targetMonsterController.GetCurrentMonsterStats().maxHealth;
        playerEvolutionSlider.maxValue = targetMonsterController.GetCurrentMonsterStats().evolutionCost;
        UpdateHealthUI();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        canSwitchMenus = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        targetPlayerController.SetCanAct(!targetPlayerController.CheckCanAct());
        targetPlayerController.SetCanMove(!targetPlayerController.CheckCanMove());
    }

    private void VictoryScreen()
    {
        if(targetPlayerController.deathRegistered)
        {
            return;
        }
        victoryPanel.SetActive(true);
        canSwitchMenus = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        targetPlayerController.SetCanAct(!targetPlayerController.CheckCanAct());
        targetPlayerController.SetCanMove(!targetPlayerController.CheckCanMove());
    }

    private void SwitchPauseMenu()
    {
        pauseGamePanelOpen = !pauseGamePanelOpen;
        pauseGamePanel.SetActive(pauseGamePanelOpen);
        Cursor.visible = pauseGamePanelOpen;

        if (pauseGamePanelOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            StartCoroutine(WaitForGrounded());
            Debug.Log("GamePaused");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            targetPlayerController.ToggleCameraLocked(false);
            Debug.Log("GameUnPause");
        }
    }

    public void CreateAbilityIcons()
    {

        currentMonsterAbilities = targetPlayerController.GetCurrentActiveAbilities();

        foreach (AbilityIcon icon in icons)
        {
            Destroy(icon.gameObject);
        }

        icons.Clear();


        foreach (MonsterActiveAbility ability in currentMonsterAbilities)
        {
            AbilityIcon icon = Instantiate(abilityIconPrefab, abilityIconContainer).GetComponent<AbilityIcon>();
            icon.SetUpIcon(ability);
            icons.Add(icon);
        }
    }

    public void ClearCooldown(MonsterActiveAbility abilityToClear)
    {
        foreach(AbilityIcon icon in icons)
        {
            if(icon.GetSourceAbility() == abilityToClear)
            {
                icon.ClearCooldown();
                break;
            }
        }
    }

    private void UpdateIconCooldownIndicator(object sender, MonsterActiveAbility ability)
    {
        foreach(AbilityIcon icon in icons)
        {
            if(icon.GetSourceAbility() == ability)
            {
                if(!ability.CheckToggleable())
                {
                    icon.PutAbilityOnCooldown();
                }
            }
        }
    }

    public void SetSensitivity(float sensitivity)
    {
        targetPlayerController.GetCameraController().SetSensitivity(sensitivity);
    }

    public void SetMusicVolume(float volume)
    {
        gameMusic.volume = volume;
    }

    public void ToggleControlsPanel()
    {
        controlPanelEnabled = !controlPanelEnabled;
        mouseSensitivitySlider.SetActive(!controlPanelEnabled);
        gameMusicSlider.SetActive(!controlPanelEnabled);
        controlsContainer.SetActive(controlPanelEnabled);
    }

    public List<AbilityIcon> GetAbilityIcons()
    {
        return icons;
    }

    private void MonsterController_OnPlayerEvolved()
    {
        CreateAbilityIcons();
    }

    public void SetButtonFunction(setButtonFunctionDelegate buttonFunction)
    {
        buttonListener = buttonFunction;

        leaveGameButton.onClick.AddListener(OnButtonClicked);
        gameOverButton.onClick.AddListener(OnButtonClicked);
        victoryButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        buttonListener?.Invoke();
    }

    IEnumerator WaitForGrounded()
    {
        yield return new WaitUntil(targetPlayerController.CheckIsGrounded);
        if(pauseGamePanelOpen)
        {
            targetPlayerController.ToggleCameraLocked(true);
        }

    }

    IEnumerator UpdateKillFeed(string playerName)
    {
        killFeedContainer.SetActive(true);
        killFeedText.text = playerName.ToUpper() + " HAS BEEN KILLED";

        yield return new WaitForSeconds(2.5f);

        killFeedText.text = "";
        killFeedContainer.SetActive(false);
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == EventCodes.PlayerDiedEventCode)
        {
            int receievedPhotonView = (int)photonEvent.CustomData;

            if(receievedPhotonView == targetPlayerController.GetPhotonView().ViewID)
            {
                canSwitchMenus = false;

                GameOver();

                targetPlayerController.deathRegistered = true;
            }

            else
            {
                StartCoroutine(UpdateKillFeed(PhotonView.Find(receievedPhotonView).Owner.NickName));

                if (GameManager.Instance.CheckPlayerVictory())
                {
                    VictoryScreen();
                    targetPlayerController.PlayVictorySound();
                }
            }
        }
    }
}
