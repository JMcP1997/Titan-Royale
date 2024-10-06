using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestiaryButtonController : MonoBehaviour
{
    [SerializeField] GameObject[] furballModels;

    [SerializeField] GameObject[] huskModels;

    [SerializeField] GameObject[] skullModels;

    private GameObject[] activeArray;
    private GameObject activeModel;

    private void Start()
    {
        activeModel = furballModels[0];
    }

    public void ViewFurball()
    {
        activeArray = furballModels;
        FirstEvolutionView();
    }

    public void ViewHusk()
    {
        activeArray = huskModels;
        FirstEvolutionView();
    }

    public void ViewSkull()
    {
        activeArray = skullModels;
        FirstEvolutionView();
    }

    public void FirstEvolutionView()
    {
        activeModel.SetActive(false);
        activeModel = activeArray[0];
        activeModel.SetActive(true);
        activeModel.GetComponent<BestiaryModel>().ModelSelected();
    }

    public void SecondEvolutionView() 
    {
        activeModel.SetActive(false);
        activeModel = activeArray[1];
        activeModel.SetActive(true);
        activeModel.GetComponent<BestiaryModel>().ModelSelected();
    }

    public void ThirdEvolutionView()
    {
        activeModel.SetActive(false);
        activeModel = activeArray[2];
        activeModel.SetActive(true);
        activeModel.GetComponent<BestiaryModel>().ModelSelected();
    }

    public void BackButton()
    {
        activeModel.SetActive(false);
    }
}
