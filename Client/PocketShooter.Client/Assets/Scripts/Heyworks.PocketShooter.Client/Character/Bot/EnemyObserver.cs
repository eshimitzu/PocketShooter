using System.Collections.Generic;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using Heyworks.PocketShooter.Utils;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot
{
    // HOTFIX: v.shimkovich script execution order set to -5 to be before behaviour designer actions.
    // Because order was not guaranteed there were a bug, when AvailableTargets wasn't exist anymore but were used in BotAim action
    // before EnemyObserver remove them from the list.
    public class EnemyObserver : MonoBehaviour
    {
        [SerializeField]
        private LayerMask raycastLayerMask;

        private BotCharacter bot = null;

        // TODO: v.shimkovich better use id. References is unsafe, see comment on the class.

        /// <summary>
        /// Characters : visible, even behind.
        /// </summary>
        public List<IRemotePlayer> VisibleCharacters { get; } = new List<IRemotePlayer>();

        /// <summary>
        /// Characters : visible, in eye sight, in attack range.
        /// </summary>
        public List<IRemotePlayer> AttackableCharacters { get; } = new List<IRemotePlayer>();

        public EnemyTarget CurrentTarget { get; private set; } = null;

        public bool IsEnemyInCrosshair => EnemyInCrosshair != null;

        public NetworkCharacter EnemyInCrosshair { get; private set; }

        public Dictionary<int, DamageInput> DamageInputs = new Dictionary<int, DamageInput>(10);

        private float frustum = 0;

        public int _visibleCount;
        public int _atackableCount;
        public float _initialPriority;
        public List<float> _targetPriorities = new List<float>();
        public List<float> _damagePriorities = new List<float>();

        private void Start()
        {
            bot = GetComponent<BotCharacter>();
            frustum = Mathf.Cos(bot.EyeCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);

            bot.ModelEvents.Damaged.Subscribe(HandleDamages).AddTo(this);
        }

        private void Update()
        {
            EnemyInCrosshair = TestEnemy();

            UpdateVisibility();

            SelectPriorityTarget();
        }

        private void UpdateVisibility()
        {
            VisibleCharacters.Clear();
            AttackableCharacters.Clear();

            foreach (IRemotePlayer player in bot.Simulation.Game.Players.Values)
            {
                bool isEnemy = player.Team != bot.Model.Team;
                if (isEnemy && !player.Effects.Invisible.IsInvisible)
                {
                    FpsTransformComponent t = player.Transform;
                    if (bot.LocalPlayer != null && player.Id == bot.LocalPlayer.Id)
                    {
                        // physics != model for local character
                        t = bot.LocalPlayer.Transform;
                    }

                    Vector3 playerPosition = t.Position;

                    Vector3 pos = transform.position;
                    Vector3 origin = pos + (Vector3.up * 0.5f);
                    Vector3 direction = (playerPosition - pos).normalized;

                    if (Physics.Raycast(origin, direction, out RaycastHit hitInfo))
                    {
                        var target = hitInfo.collider.GetComponentInParent<NetworkCharacter>();
                        if (target && !target.IsDead && target.Id == player.Id)
                        {
                            VisibleCharacters.Add(player);

                            bool withinSight = WithinSight(bot.EyeCamera.transform, t.Position);
                            if (withinSight && hitInfo.distance < bot.Model.CurrentWeapon.Info.MaxRange)
                            {
                                AttackableCharacters.Add(player);
                            }
                        }
                    }
                }
            }

            _visibleCount = VisibleCharacters.Count;
            _atackableCount = AttackableCharacters.Count;
        }

        private bool WithinSight(Transform eye, Vector3 pos)
        {
            Vector3 look = (pos - eye.position).normalized;
            float dot = Vector3.Dot(eye.forward, look);
            bool canBeVisible = dot > frustum;

            return canBeVisible;
        }

        private bool WithinRange(Vector3 position)
        {
            float distance = Vector3.Distance(bot.transform.position, position);
            return distance < bot.Model.CurrentWeapon.Info.MaxRange;
        }

        private NetworkCharacter TestEnemy()
        {
            Vector3 origin = transform.position + (Vector3.up * 0.5f);
            Vector3 direction = bot.EyeCamera.transform.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, bot.Model.CurrentWeapon.Info.MaxRange))
            {
                var target = hitInfo.collider.GetComponentInParent<NetworkCharacter>();
                return target && target.IsDead ? null : target;
            }

            return null;
        }

        private void SelectPriorityTarget()
        {
            _initialPriority = 0;
            _targetPriorities.Clear();
            _damagePriorities.Clear();

            var lastPriority = GetPriority(CurrentTarget?.Player) * 2f;
            _initialPriority = lastPriority;
            foreach (IRemotePlayer player in AttackableCharacters)
            {
                float p = GetPriority(player);
                DamageWeight(player.Id, out float damage);
                p += damage;

                _targetPriorities.Add(p);

                if (CurrentTarget == null || p > CurrentTarget.Priority)
                {
                    CurrentTarget = new EnemyTarget
                    {
                        Player = player,
                        Priority = p,
                        IsVisible = true,
                        LastSeen = Time.time,
                        WithinRange = true,
                    };
                }
            }

            // handle input damage
            foreach (IRemotePlayer remotePlayer in bot.Simulation.Game.Players.Values)
            {
                if (DamageWeight(remotePlayer.Id, out float p))
                {
                    _damagePriorities.Add(p);

                    if (CurrentTarget == null || p > CurrentTarget.Priority)
                    {
                        CurrentTarget = new EnemyTarget
                        {
                            Player = remotePlayer,
                            Priority = p,
                            IsVisible = false,
                            LastSeen = Time.time,
                            WithinRange = false,
                        };
                    }
                }
            }

            // reset if dead or invisible
            if (CurrentTarget != null)
            {
                if (CurrentTarget.Player.IsDead ||
                    CurrentTarget.Player.Effects.Invisible.IsInvisible)
                {
                    CurrentTarget = null;
                }
            }

            // update target
            if (CurrentTarget != null)
            {
                CurrentTarget.IsVisible = VisibleCharacters.Contains(CurrentTarget.Player);
                CurrentTarget.WithinRange = WithinRange(CurrentTarget.Player.Transform.Position);

                if (CurrentTarget.IsVisible)
                {
                    CurrentTarget.LastSeen = Time.time;
                }
                else if (Time.time - CurrentTarget.LastSeen > 5f)
                {
                    CurrentTarget = null;
                }
            }

            // do other stuff if targets are insignificant //need test!!!!
//            if (CurrentTarget?.priority < 0.2f)
//            {
//                CurrentTarget = null;
//            }
        }

        private float GetPriority(IRemotePlayer player)
        {
            if (player == null)
            {
                return 0;
            }

            Position p = player.Transform.Position;
            float distance = Vector3.Distance(transform.position, new Vector3(p.X, p.Y, p.Z));

            float d = 1 - distance / bot.Model.CurrentWeapon.Info.MaxRange;
            float h = 1 - player.Health.Health / player.Health.MaxHealth;

            return (d + h) * 0.5f;
        }

        protected virtual void HandleDamages(DamagedServerEvent dse)
        {
            var damages = dse.Damages.Span;
            for (int i = 0; i < damages.Length; i++)
            {
                ref readonly DamageInfo damage = ref damages[i];
                if (!DamageInputs.TryGetValue(damage.AttackerId, out DamageInput input))
                {
                    input = new DamageInput();
                    DamageInputs.Add(damage.AttackerId, input);
                }

                input.LastTime = Time.time;
                input.Damage += damage.Damage;
            }
        }

        private bool DamageWeight(EntityId playerId, out float weight)
        {
            weight = 0;
            if (DamageInputs.TryGetValue(playerId, out DamageInput input))
            {
                if (Time.time - input.LastTime > 2f)
                {
                    DamageInputs.Remove(playerId);
                }
                else
                {
                    weight = input.Damage / bot.Model.Health.MaxHealth;
                    return true;
                }
            }

            return false;
        }

        public class EnemyTarget
        {
            public float Priority;
            public IRemotePlayer Player;
            public float LastSeen;
            public bool IsVisible;
            public bool WithinRange;
        }

        public class DamageInput
        {
            public float LastTime;
            public float Damage;
        }
    }
}