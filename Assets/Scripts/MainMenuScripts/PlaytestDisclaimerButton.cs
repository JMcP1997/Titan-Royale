using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaytestDisclaimerButton : MonoBehaviour
{
    public void OnClickContinue()
    {
        SceneManager.LoadScene("ConnectToServer");
    }
}
