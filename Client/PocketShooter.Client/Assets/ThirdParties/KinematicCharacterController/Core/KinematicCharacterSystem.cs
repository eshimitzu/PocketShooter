﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController
{
    /// <summary>
    /// The system that manages the simulation of KinematicCharacterMotor and PhysicsMover
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class KinematicCharacterSystem : MonoBehaviour
    {
        private static KinematicCharacterSystem _instance;

        /// <summary>
        /// All KinematicCharacterMotor currently being simulated
        /// </summary>
        public static List<KinematicCharacterMotor> CharacterMotors = new List<KinematicCharacterMotor>(CharacterMotorsBaseCapacity);
        /// <summary>
        /// All PhysicsMover currently being simulated
        /// </summary>
        public static List<PhysicsMover> PhysicsMovers = new List<PhysicsMover>(PhysicsMoversBaseCapacity);
        /// <summary>
        /// Determines if the system simulates automatically.
        /// If true, the simulation is done on FixedUpdate
        /// </summary>
        public static bool AutoSimulation = true;
        
        private static float _lastCustomInterpolationStartTime = -1f;
        private static float _lastCustomInterpolationDeltaTime = -1f;

        private const int CharacterMotorsBaseCapacity = 100;
        private const int PhysicsMoversBaseCapacity = 100;

        /// <summary>
        /// Should interpolation of characters and PhysicsMovers be handled
        /// </summary>
        public static bool Interpolate = true;

        /// <summary>
        /// Creates a KinematicCharacterSystem instance if there isn't already one
        /// </summary>
        public static void EnsureCreation()
        {
            if (_instance == null)
            {
                GameObject systemGameObject = new GameObject("KinematicCharacterSystem");
                _instance = systemGameObject.AddComponent<KinematicCharacterSystem>();

                systemGameObject.hideFlags = HideFlags.NotEditable;
                _instance.hideFlags = HideFlags.NotEditable;
            }
        }

        /// <summary>
        /// Gets the KinematicCharacterSystem instance if any
        /// </summary>
        /// <returns></returns>
        public static KinematicCharacterSystem GetInstance()
        {
            return _instance;
        }

        /// <summary>
        /// Sets the maximum capacity of the character motors list, to prevent allocations when adding characters
        /// </summary>
        /// <param name="capacity"></param>
        public static void SetCharacterMotorsCapacity(int capacity)
        {
            if(capacity < CharacterMotors.Count)
            {
                capacity = CharacterMotors.Count;
            }
            CharacterMotors.Capacity = capacity;
        }

        /// <summary>
        /// Registers a KinematicCharacterMotor into the system
        /// </summary>
        public static void RegisterCharacterMotor(KinematicCharacterMotor motor)
        {
            CharacterMotors.Add(motor);
        }

        /// <summary>
        /// Unregisters a KinematicCharacterMotor from the system
        /// </summary>
        public static void UnregisterCharacterMotor(KinematicCharacterMotor motor)
        {
            CharacterMotors.Remove(motor);
        }

        /// <summary>
        /// Sets the maximum capacity of the physics movers list, to prevent allocations when adding movers
        /// </summary>
        /// <param name="capacity"></param>
        public static void SetPhysicsMoversCapacity(int capacity)
        {
            if (capacity < PhysicsMovers.Count)
            {
                capacity = PhysicsMovers.Count;
            }
            PhysicsMovers.Capacity = capacity;
        }

        /// <summary>
        /// Registers a PhysicsMover into the system
        /// </summary>
        public static void RegisterPhysicsMover(PhysicsMover mover)
        {
            PhysicsMovers.Add(mover);

            mover.Rigidbody.interpolation = RigidbodyInterpolation.None;
        }

        /// <summary>
        /// Unregisters a PhysicsMover from the system
        /// </summary>
        public static void UnregisterPhysicsMover(PhysicsMover mover)
        {
            PhysicsMovers.Remove(mover);
        }

        // This is to prevent duplicating the singleton gameobject on script recompiles
        private void OnDisable()
        {
            Destroy(this.gameObject);
        }

        private void Awake()
        {
            _instance = this;
        }

        private void FixedUpdate()
        {
            if (AutoSimulation)
            {
                float deltaTime = Time.deltaTime;

                if (Interpolate)
                {
                    PreSimulationInterpolationUpdate(deltaTime);
                }

                Simulate(deltaTime, CharacterMotors, CharacterMotors.Count, PhysicsMovers, PhysicsMovers.Count);

                if (Interpolate)
                {
                    PostSimulationInterpolationUpdate(deltaTime);
                }
            }
        }

        private void Update()
        {
            if (Interpolate)
            {
                CustomInterpolationUpdate();
            }
        }

        /// <summary>
        /// Remembers the point to interpolate from for KinematicCharacterMotors and PhysicsMovers
        /// </summary>
        public static void PreSimulationInterpolationUpdate(float deltaTime)
        {
            // Save pre-simulation poses and place transform at transient pose
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                CharacterMotors[i].InitialTickPosition = CharacterMotors[i].TransientPosition;
                CharacterMotors[i].InitialTickRotation = CharacterMotors[i].TransientRotation;

                CharacterMotors[i].Transform.SetPositionAndRotation(CharacterMotors[i].TransientPosition, CharacterMotors[i].TransientRotation);
            }

            for (int i = 0; i < PhysicsMovers.Count; i++)
            {
                PhysicsMovers[i].InitialTickPosition = PhysicsMovers[i].TransientPosition;
                PhysicsMovers[i].InitialTickRotation = PhysicsMovers[i].TransientRotation;

                PhysicsMovers[i].Transform.SetPositionAndRotation(PhysicsMovers[i].TransientPosition, PhysicsMovers[i].TransientRotation);
                PhysicsMovers[i].Rigidbody.position = PhysicsMovers[i].TransientPosition;
                PhysicsMovers[i].Rigidbody.rotation = PhysicsMovers[i].TransientRotation;
            }
        }

        /// <summary>
        /// Ticks the character system (ticks all KinematicCharacterMotors and PhysicsMovers)
        /// </summary>
        public static void Simulate(float deltaTime, List<KinematicCharacterMotor> motors, int characterMotorsCount, List<PhysicsMover> movers, int physicsMoversCount)
        {
#pragma warning disable 0162
            // Update PhysicsMover velocities
            for (int i = 0; i < physicsMoversCount; i++)
            {
                movers[i].VelocityUpdate(deltaTime);
            }

            // Character controller update phase 1
            for (int i = 0; i < characterMotorsCount; i++)
            {
                motors[i].UpdatePhase1(deltaTime);
            }

            // Simulate PhysicsMover displacement
            for (int i = 0; i < physicsMoversCount; i++)
            {
                movers[i].Transform.SetPositionAndRotation(movers[i].TransientPosition, movers[i].TransientRotation);
                movers[i].Rigidbody.position = movers[i].TransientPosition;
                movers[i].Rigidbody.rotation = movers[i].TransientRotation;
            }

            // Character controller update phase 2 and move
            for (int i = 0; i < characterMotorsCount; i++)
            {
                motors[i].UpdatePhase2(deltaTime);

                motors[i].Transform.SetPositionAndRotation(motors[i].TransientPosition, motors[i].TransientRotation);
            }

            Physics.SyncTransforms();
#pragma warning restore 0162
        }

        /// <summary>
        /// Initiates the interpolation for KinematicCharacterMotors and PhysicsMovers
        /// </summary>
        public static void PostSimulationInterpolationUpdate(float deltaTime)
        {
            _lastCustomInterpolationStartTime = Time.time;
            _lastCustomInterpolationDeltaTime = deltaTime;

            // Return interpolated roots to their initial poses
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                CharacterMotors[i].Transform.SetPositionAndRotation(CharacterMotors[i].InitialTickPosition, CharacterMotors[i].InitialTickRotation);
            }

            for (int i = 0; i < PhysicsMovers.Count; i++)
            {
                PhysicsMovers[i].Rigidbody.position = PhysicsMovers[i].InitialTickPosition;
                PhysicsMovers[i].Rigidbody.rotation = PhysicsMovers[i].InitialTickRotation;

                PhysicsMovers[i].Rigidbody.MovePosition(PhysicsMovers[i].TransientPosition);
                PhysicsMovers[i].Rigidbody.MoveRotation(PhysicsMovers[i].TransientRotation);
            }
        }

        /// <summary>
        /// Handles per-frame interpolation
        /// </summary>
        private static void CustomInterpolationUpdate()
        {
            float interpolationFactor = Mathf.Clamp01((Time.time - _lastCustomInterpolationStartTime) / _lastCustomInterpolationDeltaTime);

            // Handle characters interpolation
            for (int i = 0; i < CharacterMotors.Count; i++)
            {
                CharacterMotors[i].Transform.SetPositionAndRotation(
                    Vector3.Lerp(CharacterMotors[i].InitialTickPosition, CharacterMotors[i].TransientPosition, interpolationFactor),
                    Quaternion.Slerp(CharacterMotors[i].InitialTickRotation, CharacterMotors[i].TransientRotation, interpolationFactor));
            }

            // Handle PhysicsMovers interpolation
            for (int i = 0; i < PhysicsMovers.Count; i++)
            {
                PhysicsMovers[i].Transform.SetPositionAndRotation(
                    Vector3.Lerp(PhysicsMovers[i].InitialTickPosition, PhysicsMovers[i].TransientPosition, interpolationFactor),
                    Quaternion.Slerp(PhysicsMovers[i].InitialTickRotation, PhysicsMovers[i].TransientRotation, interpolationFactor));
            }
        }
    }
}