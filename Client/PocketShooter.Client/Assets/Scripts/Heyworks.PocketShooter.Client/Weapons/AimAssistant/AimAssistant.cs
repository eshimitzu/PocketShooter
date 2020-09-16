using System.Collections;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Heyworks.PocketShooter.Weapons.AimAssistant
{
    /// <summary>
    /// Represents aim assistant which helps the player perfectly aim at an enemy body.
    /// </summary>
    /// <seealso cref="MonoBehaviour" />
    /// <seealso cref="IInputSensetivityProvider" />
    /// <seealso cref="IInputAxisValueProvider" />
    public class AimAssistant : MonoBehaviour, IInputSensetivityProvider, IInputAxisValueProvider
    {
        private const float DefaultMultiplier = 1f;
        private const int ExtraRaysCount = 8;
        private const string HeadTag = "Head";

        [SerializeField]
        private AimAssistantConfiguration aimAssistantConfiguration = null;

        [SerializeField]
        private LayerMask targetLayerMask;

        [SerializeField]
        private LayerMask aimAssistTargetLayerMask;

        [SerializeField]
        private LayerMask enemyLayerMask;

        [SerializeField]
        private Vector3 mainRayOrigin = new Vector3(0.5f, 0.5f, 0f);

        private float distance;
        private float currentSensetivityMultiplier = 1f;
        private UnityEngine.Camera mainCamera;
        private Coroutine mainRayCoroutine;
        private Coroutine extraRayCoroutine;
        private Coroutine defaultCoroutine;
        private float aspectRatio = 1f;

        /// <summary>
        /// Gets the input sensetivity multiplier.
        /// </summary>
        public float InputSensetivityMultiplier => currentSensetivityMultiplier;

        /// <summary>
        /// Gets the value on mouse X axis.
        /// </summary>
        public float MouseX { get; private set; }

        /// <summary>
        /// Gets the value on mouse Y axis.
        /// </summary>
        public float MouseY { get; private set; }

        protected LocalCharacter LocalCharacter { get; private set; }

        public bool CanAim()
        {
            var player = LocalCharacter.Model;
            return
                player.IsAlive
                && !player.Effects.Stun.IsStunned
                && player.CurrentWeapon.State != WeaponState.Reloading
                && !player.IsCastingAnySkill()
                && !player.IsAimingWithSkill();
        }

        public void Setup(WeaponInfo config, LocalCharacter localCharacter)
        {
            LocalCharacter = localCharacter;
            distance = config.MaxRange;
            enabled = config.AutoAim;
        }

        private void Start()
        {
            mainCamera = UnityEngine.Camera.main;
            aspectRatio = Screen.width / (float)Screen.height;
        }

        private void Update()
        {
            if (!CanAim())
            {
                SetInput(Vector2.zero);
                return;
            }

            Collider hitCollider;

            if (MainRayTest(aimAssistTargetLayerMask, out hitCollider))
            {
                if (mainRayCoroutine == null)
                {
                    ResetCoroutine(ref extraRayCoroutine);
                    ResetCoroutine(ref defaultCoroutine);

                    mainRayCoroutine = StartCoroutine(
                        ChangeSensetivityMultiplier(
                            aimAssistantConfiguration.MainRaycastSensivity,
                            aimAssistantConfiguration.AimAssistSensivityChangeTime));
                }

                MoveAim(mainRayOrigin);
            }
            else if (ExtraRaysTest(aimAssistTargetLayerMask, out hitCollider))
            {
                if (extraRayCoroutine == null)
                {
                    ResetCoroutine(ref mainRayCoroutine);
                    ResetCoroutine(ref defaultCoroutine);

                    extraRayCoroutine = StartCoroutine(
                        ChangeSensetivityMultiplier(
                            aimAssistantConfiguration.SecondaryRaycastSensivity,
                            aimAssistantConfiguration.AimAssistSensivityChangeTime));
                }

                ExtraMoveAim();
            }
            else if (defaultCoroutine == null)
            {
                ResetCoroutine(ref mainRayCoroutine);
                ResetCoroutine(ref extraRayCoroutine);

                defaultCoroutine = StartCoroutine(
                    ChangeSensetivityMultiplier(
                        DefaultMultiplier,
                        aimAssistantConfiguration.AimAssistSensivityChangeTime));

                SetInput(Vector2.zero);
            }
        }

        private bool MoveAim(Vector3 rayOrigin)
        {
            Ray mainRay = mainCamera.ViewportPointToRay(mainRayOrigin);
            Ray ray = mainCamera.ViewportPointToRay(rayOrigin);
            ray.direction = mainCamera.transform.forward;

            ray = MoveRayOrigin(ray);
            mainRay = MoveRayOrigin(mainRay);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, targetLayerMask | enemyLayerMask))
            {
                var layerMask = 1 << hitInfo.collider.gameObject.layer;
                if ((layerMask & enemyLayerMask) == layerMask)
                {
                    float distanceToTarget = Vector3.Distance(
                        mainCamera.gameObject.transform.position,
                        hitInfo.collider.gameObject.transform.position);

                    Vector3 delta = mainCamera.transform.InverseTransformDirection(
                        Vector3.ProjectOnPlane(
                            hitInfo.collider.transform.position - mainRay.GetPoint(distanceToTarget),
                            mainCamera.transform.forward));

                    if (!hitInfo.collider.CompareTag(HeadTag))
                    {
                        delta.y = 0;
                    }

                    UpdateInput(delta);

                    return true;
                }
            }

            if (Physics.Raycast(ray, out hitInfo, distance, targetLayerMask | aimAssistTargetLayerMask))
            {
                var layerMask = 1 << hitInfo.collider.gameObject.layer;
                if ((layerMask & aimAssistTargetLayerMask) == layerMask)
                {
                    var aimAssistTarget = hitInfo.collider.gameObject.GetComponent<AimAssistantTarget>();
                    var aimMask = 1 << aimAssistTarget.BodyCollider.gameObject.layer;

                    if (aimMask != enemyLayerMask)
                    {
                        return false;
                    }

                    var plane = new Plane(
                        Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up),
                        aimAssistTarget.SelfCollider.transform.position);
                    float dist = 0;
                    plane.Raycast(mainRay, out dist);
                    var rayPoint = mainRay.GetPoint(dist);

                    var distanceToHead = Vector3.Distance(rayPoint, aimAssistTarget.HeadCollider.transform.position);
                    var distanceToBody = Vector3.Distance(rayPoint, aimAssistTarget.BodyCollider.transform.position);
                    if (distanceToBody < distanceToHead)
                    {
                        var delta = mainCamera.transform.InverseTransformDirection(
                            Vector3.ProjectOnPlane(
                                aimAssistTarget.BodyCollider.transform.position - rayPoint,
                                mainCamera.transform.forward));
                        var colliderBottomBoundingBoxY = aimAssistTarget.BodyCollider.bounds.min.y;

                        if (colliderBottomBoundingBoxY > rayPoint.y)
                        {
                            delta.y = colliderBottomBoundingBoxY - rayPoint.y + (ray.origin - mainRay.origin).y;
                        }
                        else
                        {
                            delta.y = 0;
                        }

                        UpdateInput(delta);
                        return true;
                    }
                    else
                    {
                        var delta = mainCamera.transform.InverseTransformDirection(
                            Vector3.ProjectOnPlane(
                                aimAssistTarget.HeadCollider.transform.position - rayPoint,
                                mainCamera.transform.forward));
                        UpdateInput(delta);
                        return true;
                    }
                }
            }

            return false;
        }

        private void ExtraMoveAim()
        {
            var sectorAngle = 2 * Mathf.PI / ExtraRaysCount;
            var angle = 0f;

            for (int i = 0; i < ExtraRaysCount; i++)
            {
                angle += i * sectorAngle;

                var rayOrigin = mainRayOrigin +
                                Vector3.up * aimAssistantConfiguration.AimAssistActivationRadius * Mathf.Sin(angle) +
                                Vector3.right * (aimAssistantConfiguration.AimAssistActivationRadius / aspectRatio) *
                                Mathf.Cos(angle);
                if (MoveAim(rayOrigin))
                {
                    break;
                }
            }
        }

        private Vector2 SmoothInput(Vector2 input)
        {
            float magnitude = input.magnitude;

            if (magnitude > 0)
            {
                float speed = aimAssistantConfiguration.AimAssistMoveSpeed.Evaluate(magnitude);
                return input.normalized * speed;
            }

            return Vector2.zero;
        }

        private void UpdateInput(Vector2 delta)
        {
            delta = SmoothInput(delta);

            float mouseUp = CrossPlatformInputManager.GetAxisRaw(InputController.MouseYInput);
            float mouseRight = CrossPlatformInputManager.GetAxisRaw(InputController.MouseXInput);
            float magnitude = new Vector2(mouseUp, mouseRight).magnitude;
            float scale = magnitude > aimAssistantConfiguration.AimAssistActiveInputThreshold ? 0 : 1f;

            MouseX = delta.x * scale;
            MouseY = delta.y * scale;
        }

        private void SetInput(Vector2 delta)
        {
            MouseX = delta.x;
            MouseY = delta.y;
        }

        private void ResetCoroutine(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            coroutine = null;
        }

        private Ray MoveRayOrigin(Ray ray)
        {
            var plane = new Plane(Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up), transform.position);
            plane.Raycast(ray, out float offsetFromCamera);
            ray.origin = ray.GetPoint(offsetFromCamera);

            return ray;
        }

        private bool ExtraRaysTest(LayerMask referenceLayerMask, out Collider hitCollider)
        {
            var sectorAngle = 2 * Mathf.PI / ExtraRaysCount;
            var angle = 0f;

            for (int i = 0; i < ExtraRaysCount; i++)
            {
                angle += i * sectorAngle;
                var rayOrigin = mainRayOrigin +
                                Vector3.up * aimAssistantConfiguration.AimAssistActivationRadius * Mathf.Sin(angle) +
                                Vector3.right * (aimAssistantConfiguration.AimAssistActivationRadius / aspectRatio) *
                                Mathf.Cos(angle);
                var result = RayTest(rayOrigin, referenceLayerMask, out hitCollider);
                if (result)
                {
                    return true;
                }
            }

            hitCollider = null;
            return false;
        }

        private bool MainRayTest(LayerMask referenceLayerMask, out Collider hitCollider)
        {
            var result = RayTest(mainRayOrigin, referenceLayerMask, out hitCollider);
            return result;
        }

        private bool RayTest(Vector3 rayOrigin, LayerMask referenceLayerMask, out Collider hitCollider)
        {
            var ray = mainCamera.ViewportPointToRay(rayOrigin);
            ray.direction = mainCamera.transform.forward;
            ray = MoveRayOrigin(ray);

            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, distance, targetLayerMask | referenceLayerMask))
            {
                Debug.DrawLine(ray.origin, ray.GetPoint(distance), Color.black);
                var layerMask = 1 << hitInfo.collider.gameObject.layer;
                if ((layerMask & referenceLayerMask) == referenceLayerMask)
                {
                    hitCollider = hitInfo.collider;
                    return true;
                }
            }

            Debug.DrawLine(ray.origin, ray.GetPoint(distance), Color.magenta);
            hitCollider = null;
            return false;
        }

        private IEnumerator ChangeSensetivityMultiplier(float value, float time)
        {
            for (float i = 0f; i < time; i += Time.deltaTime)
            {
                currentSensetivityMultiplier = Mathf.Lerp(currentSensetivityMultiplier, value, i / time);
                yield return null;
            }

            currentSensetivityMultiplier = value;
        }
    }
}