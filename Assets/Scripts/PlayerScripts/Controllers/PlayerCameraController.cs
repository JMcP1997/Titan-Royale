using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    private PhotonView PV;

    [SerializeField] CinemachineVirtualCamera[] vCams;
    private CinemachineVirtualCamera vCam;
    //normally the vCam would be made private and unseen in the inspector, but the movement controller needs a reference to the Virtual camera on awake

    [SerializeField] GameObject[] cameraTargets;
    private GameObject currentCameraTarget;

    [SerializeField] float topClamp = 70.0f;
    [SerializeField] float bottomClamp = -30.0f;

    [SerializeField] private float lookSensitivity;
    private float normalSensitivity;
    //These values should PROBABLY stay between 0 and 1, unless someone is playing with some crazy sensitivity

    private bool invertY = false;

    private bool lockCameraPosition = false;

    private float vCamTargetYaw;
    private float vCamTargetPitch;

    private void Awake()
    {
        vCam = vCams[0];
        currentCameraTarget = cameraTargets[0];
        normalSensitivity = lookSensitivity;
    }

    private void Start()
    {
        if (!PV.IsMine)
        {
            foreach (CinemachineVirtualCamera vCam in vCams)
            {

                Destroy(vCam.gameObject);
                //When PlayerCameraController starts on a remote instance, it destroys its Cinemachine component
                //This ensures that the game cameras dont synchronize incorrectly

            }

            foreach (GameObject cameraTarget in cameraTargets)
            {
                Destroy(cameraTarget.gameObject);
            }

            return;
        }

        controller.GetMonsterController().OnPlayerEvolved += MonsterController_OnPlayerEvolved;

    }

    public void SetPhotonView(PhotonView PV)
    {
        this.PV = PV;
    }

    public void ToggleCameraLockPosition(bool toggle)
    {
        lockCameraPosition = toggle;
    }

    public void HandleCameraRotation()
    {
        if (!lockCameraPosition)
        {
            vCamTargetYaw += Input.GetAxis("Mouse X") * lookSensitivity;
            vCamTargetPitch += Input.GetAxis("Mouse Y") * lookSensitivity * (invertY ? 1 : -1);
        }

        vCamTargetYaw = ClampAngle(vCamTargetYaw, float.MinValue, float.MaxValue);
        vCamTargetPitch = ClampAngle(vCamTargetPitch, bottomClamp, topClamp);

        currentCameraTarget.transform.rotation = Quaternion.Euler(vCamTargetPitch, vCamTargetYaw, 0);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public CinemachineVirtualCamera GetCurrentVCam()
    {
        return vCam;
    }

    private void MonsterController_OnPlayerEvolved()
    {
        if (PV.IsMine)
        {
            vCam = vCams[controller.GetCurrentEvolutionLevel()];
            currentCameraTarget = cameraTargets[controller.GetCurrentEvolutionLevel()];
        }
    }

    public void ToggleInvertY()
    {
        invertY = !invertY;
    }

    public float GetCurrentSensitivity()
    {
        return lookSensitivity;
    }

    public float GetNormalSensitivity()
    {
        return normalSensitivity;
    }

    public void SetSensitivity(float sensitivity)
    {
        lookSensitivity = sensitivity;
    }
}
