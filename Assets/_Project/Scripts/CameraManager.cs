using System.Collections;
using Cinemachine;
using KBCore.Refs;
using Platformer._Project.Scripts.Input;
using UnityEngine;

namespace Platformer._Project.Scripts
{
    public class CameraManager : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Anywhere] private InputReader input;
        [SerializeField, Anywhere] private CinemachineFreeLook freeLookVCam;

        [Header("Settings")] [SerializeField, Range(0.5f, 3f)]
        private float speedMultiplier = 1f;

        private bool isRMBPressed;
        private bool cameraMovementLock;

        private void OnEnable()
        {
            input.Look += OnLook;
            input.EnableMouseControlCamera += OnEnableMouseControlCamera;
            input.DisableMouseControlCamera += OnDisableMouseControlCamera;
        }

        private void OnDisable()
        {
            input.Look -= OnLook;
            input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
        }
        
        private void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
        {
            if(cameraMovementLock) return;
            
            if(isDeviceMouse && !isRMBPressed) return;

            float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;
            
            // Set the camera axis values
            freeLookVCam.m_XAxis.m_InputAxisValue = cameraMovement.x * speedMultiplier * deviceMultiplier;
            freeLookVCam.m_YAxis.m_InputAxisValue = cameraMovement.y * speedMultiplier * deviceMultiplier;
        }
        
        private void OnDisableMouseControlCamera()
        {
            isRMBPressed = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // reset the camera axis to prevent jumping when re-enabling mouse control
            freeLookVCam.m_XAxis.m_InputAxisValue = 0;
            freeLookVCam.m_YAxis.m_InputAxisValue = 0;
            
        }

        private void OnEnableMouseControlCamera()
        {
            isRMBPressed = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            StartCoroutine(DisableMouseForFrame());
        }

        private IEnumerator DisableMouseForFrame()
        {
            cameraMovementLock = true;
            yield return new WaitForEndOfFrame();
            cameraMovementLock = false;
        }
    }
}