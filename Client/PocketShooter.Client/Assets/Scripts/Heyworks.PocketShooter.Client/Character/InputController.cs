using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Camera;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Weapons.AimAssistant;
using Microsoft.Extensions.Logging;
using UniRx;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Zenject;

namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Represents input controller, which is responsible for reading inputs and setting them to the camera and character controllers.
    /// </summary>
    public class InputController : MonoBehaviour
    {
        public const string MouseXInput = "Mouse X";
        public const string MouseYInput = "Mouse Y";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        public OrbitCamera OrbitCamera => orbitCamera;

        [SerializeField]
        private CharacterCommon characterCommon;

        [SerializeField]
        private PocketCharacterController character;

        [SerializeField]
        private float mouseSensitivity = 0.01f;

        [Inject]
        private OrbitCamera orbitCamera;

        private readonly ISet<IFreezer> freezers = new HashSet<IFreezer>();
        private LocalCharacter player;
        private IInputSensetivityProvider[] inputSensetivityProviders;
        private LocalCharacter localCharacter;
        private IInputAxisValueProvider axisValueProvider;

        private float editorInputSpeedCoeficient = 1f;

        private bool FreezeMotion => freezers.Aggregate(false, (current, freezer) => current | freezer.FreezeMotion);

        private bool FreezeRotation =>
            freezers.Aggregate(false, (current, freezer) => current | freezer.FreezeRotation);

        public void SetEditorInputSpeedCoeficient(float sliderValue)
        {
            this.editorInputSpeedCoeficient = sliderValue;
        }

        private void Start()
        {
            orbitCamera.SetFollowTransform(characterCommon.CameraFollowPoint);
            localCharacter = GetComponent<LocalCharacter>();
            inputSensetivityProviders = GetComponents<IInputSensetivityProvider>();
            axisValueProvider = GetComponent<IInputAxisValueProvider>();
        }

        private void Update()
        {
            HandleCameraInput();
            HandleCharacterInput();
        }

        private void UpdateOnTick(TickEvent e)
        {
            if (transform && localCharacter)
            {
                var position = transform.position;
                var commandData = new MoveCommandData(
                    localCharacter.Model.Id,
                    new FpsTransformComponent(
                        position,
                        transform.rotation.eulerAngles.y,
                        orbitCamera.Transform.eulerAngles.x));
                localCharacter.AddCommand(commandData);
            }
            else
            {
                InputLog.Log.LogWarning(
                    "After LocalCharacter created, both Setup methods called, tick started. We are still not initialized.");
            }
        }

        internal void Setup(ITickEvents tickEvents) =>
            tickEvents.SimulationTick.TakeUntilDisable(this).Subscribe(UpdateOnTick).AddTo(this);

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = CrossPlatformInputManager.GetAxisRaw(MouseYInput) + axisValueProvider.MouseY;
            float mouseLookAxisRight = CrossPlatformInputManager.GetAxisRaw(MouseXInput) + axisValueProvider.MouseX;

            if (FreezeRotation)
            {
                mouseLookAxisRight = 0;
                mouseLookAxisUp = 0;
            }

            float multiplier = 1f;
            foreach (var inputSensetivityProvider in inputSensetivityProviders)
            {
                multiplier *= inputSensetivityProvider.InputSensetivityMultiplier;
            }

            Vector3 lookInputVector =
                new Vector3(
                    mouseLookAxisRight * mouseSensitivity * multiplier,
                    mouseLookAxisUp * mouseSensitivity * multiplier,
                    0f);

            orbitCamera.UpdateWithInput(Time.deltaTime, lookInputVector);
        }

        private void HandleCharacterInput()
        {
            float moveAxisForward = 0f;
            float moveAxisRight = 0f;

            if (!FreezeMotion)
            {
                // Build the CharacterInputs struct
                moveAxisForward = CrossPlatformInputManager.GetAxisRaw(VerticalInput);
                moveAxisRight = CrossPlatformInputManager.GetAxisRaw(HorizontalInput);
#if UNITY_EDITOR
                moveAxisForward = Input.GetAxisRaw(VerticalInput);
                moveAxisRight = Input.GetAxisRaw(HorizontalInput);

                moveAxisForward *= editorInputSpeedCoeficient;
                moveAxisRight *= editorInputSpeedCoeficient;
#endif
            }

            var cameraRotation = orbitCamera.Transform.rotation;

            var characterInputs = new PlayerCharacterInputs(moveAxisForward, moveAxisRight, cameraRotation);

            character.SetInputs(ref characterInputs);
        }

        public void AddFreezer(IFreezer freezer) => freezers.Add(freezer);

        public void RemoveFreezer(IFreezer freezer) => freezers.Remove(freezer);
    }
}