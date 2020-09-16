using System.Collections.Generic;
using UnityEngine;

namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    public class CameraManager : MonoBehaviour
    {
        private const KeyCode OrbitCamera = KeyCode.Keypad5;
        private const KeyCode BackCamera = KeyCode.Keypad2;
        private const KeyCode BackLeftCamera = KeyCode.Keypad1;
        private const KeyCode LeftCamera = KeyCode.Keypad4;
        private const KeyCode FrontLeftCamera = KeyCode.Keypad7;
        private const KeyCode FrontCamera = KeyCode.Keypad8;
        private const KeyCode FrontRightCamera = KeyCode.Keypad9;
        private const KeyCode RightCamera = KeyCode.Keypad6;
        private const KeyCode BackRightCamera = KeyCode.Keypad3;

        [SerializeField]
        private List<UnityEngine.Camera> cameras;

        private UnityEngine.Camera activeCamera;

        private void Start()
        {
            foreach (UnityEngine.Camera camera in cameras)
            {
                camera.gameObject.SetActive(false);
            }

            activeCamera = activeCamera = cameras.Find(camera => camera.name == "BackCamera");
        }

        private void Update()
        {
            if (Input.GetKeyDown(BackCamera))
            {
                SwitchCamera(nameof(BackCamera));
            }
            else if (Input.GetKeyDown(BackLeftCamera))
            {
                SwitchCamera(nameof(BackLeftCamera));
            }
            else if (Input.GetKeyDown(LeftCamera))
            {
                SwitchCamera(nameof(LeftCamera));
            }
            else if (Input.GetKeyDown(FrontLeftCamera))
            {
                SwitchCamera(nameof(FrontLeftCamera));
            }
            else if (Input.GetKeyDown(FrontCamera))
            {
                SwitchCamera(nameof(FrontCamera));
            }
            else if (Input.GetKeyDown(FrontRightCamera))
            {
                SwitchCamera(nameof(FrontRightCamera));
            }
            else if (Input.GetKeyDown(RightCamera))
            {
                SwitchCamera(nameof(RightCamera));
            }
            else if (Input.GetKeyDown(BackRightCamera))
            {
                SwitchCamera(nameof(BackRightCamera));
            }
            else if (Input.GetKeyDown(OrbitCamera))
            {
                activeCamera.gameObject.SetActive(false);
            }
        }

        private void SwitchCamera(string cameraName)
        {
            activeCamera.gameObject.SetActive(false);
            activeCamera = cameras.Find(camera => camera.name == cameraName);
            activeCamera.gameObject.SetActive(true);
        }
    }
}