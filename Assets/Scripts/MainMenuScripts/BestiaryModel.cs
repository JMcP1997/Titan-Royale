using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestiaryModel : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] Quaternion startRotation;

    bool isActive = false;

    private void Start()
    {
        startRotation = transform.rotation;
    }
    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

    }

    public void ModelSelected()
    {
        transform.rotation = startRotation;
        isActive = true;
    }
}
