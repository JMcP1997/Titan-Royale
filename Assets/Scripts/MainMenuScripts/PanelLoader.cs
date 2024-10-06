using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelLoader : MonoBehaviour
{

    [SerializeField] GameObject TutorialPanel;

    [SerializeField] GameObject ControlsPanel;

    [SerializeField] GameObject GameplayPanel;

    [SerializeField] GameObject RoomSetupPanel;
#nullable enable
    [SerializeField] GameObject? BestiaryPanel;
    [SerializeField] GameObject? LobbyPanel;
#nullable disable

    private void Awake()
    {
         
        TutorialPanel.SetActive(false);

        ControlsPanel.SetActive(false);

        GameplayPanel.SetActive(false);

        RoomSetupPanel.SetActive(false);

        if(BestiaryPanel != null)
        {
            BestiaryPanel.SetActive(false);
        }
    }

    public void ActivateTutorialPanel()
    {
        TutorialPanel.SetActive(true);
    }


    public void DeactivateTutorialPanel()
    {
        TutorialPanel.SetActive(false);
    }


    public void ActivateControlsPanel()
    {
        ControlsPanel.SetActive(true);
    }


    public void DeactivateControlsPanel()
    {
        ControlsPanel.SetActive(false);
    }


    public void ActivateGameplayPanel()
    {
        GameplayPanel.SetActive(true);
    }


    public void DeactivateGameplayPanel()
    {
        GameplayPanel.SetActive(false);
    }


    public void ActivateRoomSetupPanel()
    {
        RoomSetupPanel.SetActive(true);
    }


    public void DeactivateRoomSetupPanel()
    {
        RoomSetupPanel.SetActive(false);
    }

    public void ActivateBestiaryPanel()
    {
        BestiaryPanel.SetActive(true);
        LobbyPanel.SetActive(false);
    }

    public void DeactivateBestiaryPanel()
    {
        BestiaryPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }


}
