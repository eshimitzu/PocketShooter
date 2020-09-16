using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KinematicCharacterController;
using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Represents implementation of character controller.
    /// </summary>
    /// <seealso cref="KinematicCharacterController.BaseCharacterController" />
    public class PocketCharacterController : BaseCharacterController, IAnimationSpeedProvider
    {
        [SerializeField]
        private CharacterControllerSettings settings = null;

        private Vector3 moveInputVector3;
        private Vector3 lookInputVector;

        private bool dashRequested;
        private float dashDuration;
        private float dashSpeed;
        private Quaternion dashCameraRotation;
        private bool isDashing;
        private float dashTime;

        private bool allowJumpOrDashWhenSliding;
        private float jumpPreGroundingGraceTime;
        private float jumpPostGroundingGraceTime;

        private float jumpSpeed;
        private float jumpAngle;
        private bool jumpRequested;
        private bool jumpConsumed;
        private bool jumpedThisFrame;
        private float timeSinceJumpRequested = Mathf.Infinity;
        private float timeSinceLastAbleToJump;

        private float maxStableMoveSpeed;
        private HashSet<IMoveSpeedMultiplier> speedMultipliers = new HashSet<IMoveSpeedMultiplier>();

        public event Action OnLand;

        public float SpeedMultiplier
        {
            get
            {
                float speedMultiplier = 1;
                foreach (IMoveSpeedMultiplier provider in speedMultipliers)
                {
                    speedMultiplier = speedMultiplier + provider.SpeedMultiplier - 1;
                }

                return speedMultiplier;
            }
        }

        public float MaxStableMoveSpeed => isDashing ? dashSpeed : maxStableMoveSpeed * SpeedMultiplier;

        public void SetupMaxSpeed(float maxStableMoveSpeed)
        {
            this.maxStableMoveSpeed = maxStableMoveSpeed;
        }

        public void AddMoveSpeedMultiplier(IMoveSpeedMultiplier multiplier)
        {
            speedMultipliers.Add(multiplier);
        }

        public void RemoveMoveSpeedMultiplier(IMoveSpeedMultiplier multiplier)
        {
            speedMultipliers.Remove(multiplier);
        }

        /// <summary>
        /// Gets the current speed along forward axis.
        /// </summary>
        public float CurrentForwardSpeed
        {
            get
            {
                var fwdSpeed = Vector3.Dot(Motor.BaseVelocity, Motor.CharacterForward);

                var direction = transform.InverseTransformDirection(Motor.BaseVelocity);
                var scale = settings.GetSpeedScaleCoefficient(direction);

                return fwdSpeed / (scale * this.MaxStableMoveSpeed);
            }
        }

        /// <summary>
        /// Gets the current speed along right axis.
        /// </summary>
        public float CurrentRightSpeed
        {
            get
            {
                var rightSpeed = Vector3.Dot(Motor.BaseVelocity, Motor.CharacterRight);

                var direction = transform.InverseTransformDirection(Motor.BaseVelocity);
                var scale = settings.GetSpeedScaleCoefficient(direction);

                return rightSpeed / (scale * this.MaxStableMoveSpeed);
            }
        }

        /// <summary>
        /// Applies inputs to character.
        /// This is called every frame by in order to tell the character what its inputs are.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        [SuppressMessage(
            "StyleCop.CSharp.OrderingRules",
            "SA1202:ElementsMustBeOrderedByAccess",
            Justification = "Reviewed.")]
        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            // Clamp input
            Vector3 moveInputVector = Vector3.ClampMagnitude(
                new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward),
                1f);

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(
                inputs.CameraRotation * new Vector3(0f, 0f, 1f),
                Motor.CharacterUp).normalized;
            Vector3 cameraMovementDirection =
                Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (Math.Abs(cameraPlanarDirection.sqrMagnitude) < 0.00001f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp)
                    .normalized;
            }

            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraMovementDirection, Motor.CharacterUp);
            moveInputVector = ScaleInput(moveInputVector);

            // Move and look inputs
            moveInputVector3 = cameraPlanarRotation * moveInputVector;
            lookInputVector = cameraPlanarDirection;
        }

        public void Jump(float angle, float speed)
        {
            jumpSpeed = speed;
            jumpAngle = angle;
            jumpRequested = true;
            timeSinceJumpRequested = 0f;
        }

        public void Dash(float dashLength, float dashSpeed, Quaternion cameraRotation)
        {
            dashRequested = true;
            dashDuration = dashLength / dashSpeed;
            this.dashSpeed = dashSpeed;
            dashCameraRotation = cameraRotation;
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called before the character begins its movement update.
        /// </summary>
        /// <param name="deltaTime">Delta time.</param>
        public override void BeforeCharacterUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its rotation should be right now.
        /// This is the ONLY place where you should set the character's rotation.
        /// </summary>
        /// <param name="currentRotation">Current rotation.</param>
        /// <param name="deltaTime">Delta time.</param>
        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (lookInputVector != Vector3.zero && settings.OrientationSharpness > 0f)
            {
                // Smoothly interpolate from current to target look direction
                Vector3 smoothedLookInputDirection = Vector3.Slerp(
                    Motor.CharacterForward,
                    lookInputVector,
                    1 - Mathf.Exp(-settings.OrientationSharpness * deltaTime)).normalized;

                // Set the current rotation (which will be used by the KinematicCharacterMotor)
                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its velocity should be right now.
        /// This is the ONLY place where you can set the character's velocity.
        /// </summary>
        /// <param name="currentVelocity">Current velocity.</param>
        /// <param name="deltaTime">Delta time.</param>
        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            Vector3 targetMovementVelocity = Vector3.zero;

            if (isDashing)
            {
                if (dashTime < dashDuration)
                {
                    dashTime += deltaTime;
                    var characterInputs = new PlayerCharacterInputs(1f, 0f, dashCameraRotation);
                    SetInputs(ref characterInputs);
                }
                else
                {
                    dashTime = 0f;
                    isDashing = false;
                }
            }

            if (Motor.GroundingStatus.IsStableOnGround)
            {
                // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
                currentVelocity =
                    Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) *
                    currentVelocity.magnitude;

                if (dashRequested
                    && !isDashing
                    && (allowJumpOrDashWhenSliding
                        ? Motor.GroundingStatus.FoundAnyGround
                        : Motor.GroundingStatus.IsStableOnGround))
                {
                    dashRequested = false;
                    isDashing = true;
                }

                // Calculate target velocity
                Vector3 inputRight = Vector3.Cross(moveInputVector3, Motor.CharacterUp);
                Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized *
                                          moveInputVector3.magnitude;

                targetMovementVelocity = reorientedInput * this.MaxStableMoveSpeed;

                // Smooth movement Velocity
                currentVelocity = Vector3.Lerp(
                    currentVelocity,
                    targetMovementVelocity,
                    1 - Mathf.Exp(-settings.StableMovementSharpness * deltaTime));

                jumpedThisFrame = false;
                timeSinceJumpRequested += 0/*deltaTime*/; // https://heyworks.atlassian.net/browse/PSH-536
                if (jumpRequested)
                {
                    // See if we actually are allowed to jump
                    if (!jumpConsumed
                        && !isDashing
                        && ((allowJumpOrDashWhenSliding
                                ? Motor.GroundingStatus.FoundAnyGround
                                : Motor.GroundingStatus.IsStableOnGround)
                            || timeSinceLastAbleToJump <= jumpPostGroundingGraceTime))
                    {
                        float jumpDirectionY = Mathf.Sin(Mathf.Deg2Rad * jumpAngle);

                        float jumpDirectionProjectionZ = Mathf.Sqrt(1 - jumpDirectionY * jumpDirectionY);

                        float jumpDirectionX = Motor.CharacterForward.x * jumpDirectionProjectionZ;

                        float jumpDirectionZ = Motor.CharacterForward.z * jumpDirectionProjectionZ;

                        Vector3 jumpDirection = new Vector3(jumpDirectionX, jumpDirectionY, jumpDirectionZ);

                        // Makes the character skip ground probing/snapping on its next update.
                        // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                        Motor.ForceUnground();

                        // Add to the return velocity and reset jump state
                        currentVelocity += (jumpDirection * jumpSpeed) -
                                           Vector3.Project(currentVelocity, Motor.CharacterUp);
                        jumpRequested = false;
                        jumpConsumed = true;
                        jumpedThisFrame = true;
                    }
                }
            }
            else
            {
                // Add move input
                if (moveInputVector3.sqrMagnitude > 0f)
                {
                    targetMovementVelocity = moveInputVector3 * settings.MaxAirMoveSpeed;

                    // Prevent climbing on un-stable slopes with air movement
                    if (Motor.GroundingStatus.FoundAnyGround)
                    {
                        Vector3 perpenticularObstructionNormal = Vector3.Cross(
                            Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal),
                            Motor.CharacterUp).normalized;
                        targetMovementVelocity = Vector3.ProjectOnPlane(
                            targetMovementVelocity,
                            perpenticularObstructionNormal);
                    }

                    Vector3 velocityDiff = Vector3.ProjectOnPlane(
                        targetMovementVelocity - currentVelocity,
                        settings.Gravity);
                    currentVelocity += velocityDiff * settings.AirAccelerationSpeed * deltaTime;
                }

                // Gravity
                currentVelocity += settings.Gravity * deltaTime;

                if (currentVelocity.y < 0)
                {
                    currentVelocity *= settings.FallMultiplier;
                }

                // Drag
                currentVelocity *= 1f / (1f + (settings.Drag * deltaTime));
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called after the character has finished its movement update.
        /// </summary>
        /// <param name="deltaTime">Delta time.</param>
        public override void AfterCharacterUpdate(float deltaTime)
        {
            // Handle jumping pre-ground grace period
            if (jumpRequested && timeSinceJumpRequested > jumpPreGroundingGraceTime)
            {
                jumpRequested = false;
            }

            if (allowJumpOrDashWhenSliding
                ? Motor.GroundingStatus.FoundAnyGround
                : Motor.GroundingStatus.IsStableOnGround)
            {
                // If we're on a ground surface, reset jumping values
                if (!jumpedThisFrame)
                {
                    jumpConsumed = false;
                }

                timeSinceLastAbleToJump = 0f;
            }
            else
            {
                // Keep track of time since we were last able to jump (for grace period)
                timeSinceLastAbleToJump += deltaTime;
            }
        }

        /// <summary>
        /// Asks if a given collider should be considered for character collisions.
        /// Useful for ignoring specific colliders in specific situations.
        /// </summary>
        /// <param name="coll">Collider to test.</param>
        public override bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        /// <summary>
        /// Gives you a callback for ground probing hits.
        /// </summary>
        /// <param name="hitCollider">Hit collider.</param>
        /// <param name="hitNormal">Hit normal.</param>
        /// <param name="hitPoint">Hit point.</param>
        /// <param name="hitStabilityReport">Hit stability report.</param>
        public override void OnGroundHit(
            Collider hitCollider,
            Vector3 hitNormal,
            Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
            OnLand?.Invoke();
        }

        /// <summary>
        /// Gives you a callback for character movement hits.
        /// </summary>
        /// <param name="hitCollider">Hit collider.</param>
        /// <param name="hitNormal">Hit normal.</param>
        /// <param name="hitPoint">Hit point.</param>
        /// <param name="hitStabilityReport">Hit stability report.</param>
        public override void OnMovementHit(
            Collider hitCollider,
            Vector3 hitNormal,
            Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        /// <summary>
        /// Gives you a callback for when the character has finished evaluating its grounding status.
        /// </summary>
        /// <param name="deltaTime">Delta time.</param>
        public override void PostGroundingUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// Adds the velocity.
        /// </summary>
        /// <param name="velocity">The velocity.</param>
        public void AddVelocity(Vector3 velocity)
        {
        }

        /// <summary>
        /// Gives you a chance to modify the HitStabilityReport of every character movement hit before it is returned to the movement code.
        /// Use this for advanced customization of character hit stability.
        /// </summary>
        /// <param name="hitCollider">Hit collider.</param>
        /// <param name="hitNormal">Hit normal.</param>
        /// <param name="hitPoint">Hit point.</param>
        /// <param name="atCharacterPosition"> Character position.</param>
        /// <param name="atCharacterRotation">Character rotation.</param>
        /// <param name="hitStabilityReport">Hit stability report.</param>
        public override void ProcessHitStabilityReport(
            Collider hitCollider,
            Vector3 hitNormal,
            Vector3 hitPoint,
            Vector3 atCharacterPosition,
            Quaternion atCharacterRotation,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        /// <summary>
        /// Sets the position.
        /// </summary>
        /// <param name="pos">The position.</param>
        public void SetPosition(Vector3 pos) => Motor.SetPosition(pos);

        /// <summary>
        /// Sets the rotation.
        /// </summary>
        /// <param name="rot">The rot.</param>
        public void SetRotation(Quaternion rot) => Motor.SetRotation(rot);

        /// <summary>
        /// Scales the input.
        /// </summary>
        /// <param name="input">The input.</param>
        private Vector3 ScaleInput(Vector3 input)
        {
            var scale = settings.GetSpeedScaleCoefficient(input);
            return input * scale;
        }
    }

    /// <summary>
    /// Represents character inputs.
    /// </summary>
    public struct PlayerCharacterInputs
    {
        /// <summary>
        /// Gets the move forward input.
        /// </summary>
        public float MoveAxisForward { get; }

        /// <summary>
        /// Gets the move right input.
        /// </summary>
        public float MoveAxisRight { get; }

        /// <summary>
        /// Gets the camera rotation input.
        /// </summary>
        public Quaternion CameraRotation { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerCharacterInputs"/> struct.
        /// </summary>
        /// <param name="moveAxisForward">The move axis forward.</param>
        /// <param name="moveAxisRight">The move axis right.</param>
        /// <param name="cameraRotation">The camera rotation.</param>
        public PlayerCharacterInputs(float moveAxisForward, float moveAxisRight, Quaternion cameraRotation)
        {
            MoveAxisForward = moveAxisForward;
            MoveAxisRight = moveAxisRight;
            CameraRotation = cameraRotation;
        }
    }
}