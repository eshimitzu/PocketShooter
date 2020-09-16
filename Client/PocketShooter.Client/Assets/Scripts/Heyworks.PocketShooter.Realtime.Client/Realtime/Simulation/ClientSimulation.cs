using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;
using Heyworks.PocketShooter.Realtime.Service;
using Heyworks.PocketShooter.Realtime.Systems;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    public class ClientSimulation : IClientSimulation
    {
        private readonly ILocalStateProvider<PlayerState> localPlayerStateProvider;

        private readonly INetworkService networkService;
        private readonly Queue<IGameCommandData> gameCommandQueue;

        private readonly ITicker ticker;
        private readonly Game game;

        private readonly WeaponResetStateSystem weaponResetStateSystem;
        private readonly WeaponStartWarmingUpSystem weaponStartWarmingUpSystem;
        private readonly WeaponWarmingUpSystem weaponWarmingUpSystem;
        private readonly WeaponResetWarmingUpProgressSystem weaponResetWarmingUpProgressSystem;
        private readonly WeaponAttackSystem weaponAttackSystem;
        private readonly WeaponStartReloadSystem weaponStartReloadSystem;
        private readonly WeaponReloadSystem weaponReloadSystem;

        private readonly SkillExecutionSystem skillExecutionSystem;
        private readonly FinishCastSkillSystem finishCastSystem;
        private readonly StunSystem stunSystem;
        private readonly RootSystem rootSystem;
        private readonly InvisibleSystem invisibleSystem;
        private readonly JumpSystem jumpSystem;
        private readonly LuckyDiscardSystem luckyDiscardSystem;
        private readonly DashSystem dashSystem;

        private ClientPlayer LocalPlayer => game.LocalPlayer;

        public Game Game => game;

        public SimulationEvents Events { get; }

        public ClientSimulation(
            INetworkService networkService,
            ILocalStateProvider<PlayerState> localPlayerStateProvider,
            Game clientGameContext,
            ITicker ticker)
        {
            this.networkService = networkService;
            this.ticker = ticker;
            this.localPlayerStateProvider = localPlayerStateProvider;
            this.game = clientGameContext;
            this.Events = new SimulationEvents();

            gameCommandQueue = new Queue<IGameCommandData>();

            weaponStartReloadSystem = new WeaponStartReloadSystem(ticker);
            weaponReloadSystem = new WeaponReloadSystem(ticker);
            weaponAttackSystem = new WeaponAttackSystem(ticker);

            weaponResetStateSystem = new WeaponResetStateSystem(ticker);
            weaponStartWarmingUpSystem = new WeaponStartWarmingUpSystem();
            weaponWarmingUpSystem = new WeaponWarmingUpSystem();
            weaponResetWarmingUpProgressSystem = new WeaponResetWarmingUpProgressSystem();
            stunSystem = new StunSystem(ticker);
            rootSystem = new RootSystem(ticker);
            invisibleSystem = new InvisibleSystem(ticker);
            jumpSystem = new JumpSystem(ticker);
            luckyDiscardSystem = new LuckyDiscardSystem(ticker);
            dashSystem = new DashSystem(ticker);

            skillExecutionSystem = new SkillExecutionSystem(ticker);
            finishCastSystem = new FinishCastSkillSystem(ticker);
        }

        public void Start(int currentWorldTick, in PlayerState initState)
        {
            PlayerState playerState = default;
            initState.Clone(ref playerState);
            localPlayerStateProvider.Start(currentWorldTick, in playerState);

            // TODO: v.shimkovich redundant lines??
            initState.Clone(ref playerState);
            localPlayerStateProvider.Insert(ticker.Current, in playerState);
        }

        public void AddCommand(IGameCommandData gameCommand) => gameCommandQueue.Enqueue(gameCommand);

        public void AddCommand(IServiceCommandData serviceCommand) => networkService.QueueCommand(serviceCommand);

        public void Simulate(int tick)
        {
            // order of the systems must be the same as on the server

            jumpSystem.Execute(LocalPlayer);

            if (stunSystem.Execute(LocalPlayer))
            {
                Events.EnqueueClient((LocalPlayer.Events,
                    new StunChangeEvent(LocalPlayer.Effects.Stun.IsStunned)));
            }

            if (rootSystem.Execute(LocalPlayer))
            {
                Events.EnqueueClient((LocalPlayer.Events,
                    new RootChangeEvent(LocalPlayer.Effects.Root.IsRooted)));
            }

            if (invisibleSystem.Execute(LocalPlayer))
            {
                Events.EnqueueClient((LocalPlayer.Events,
                    new InvisibilityChangeEvent(LocalPlayer.Effects.Invisible.IsInvisible)));
            }

            if (luckyDiscardSystem.Execute(LocalPlayer))
            {
                Events.EnqueueClient((LocalPlayer.Events,
                    new LuckyChangeEvent(LocalPlayer.Effects.Lucky.IsLucky)));
            }

            if (dashSystem.Execute(LocalPlayer))
            {
                Events.EnqueueClient(
                    (LocalPlayer.Events, new DashChangeEvent(LocalPlayer.Effects.Dash.IsDashing)));
            }

            CastFinishSystem(LocalPlayer.Skill1);
            CastFinishSystem(LocalPlayer.Skill2);
            CastFinishSystem(LocalPlayer.Skill3);
            CastFinishSystem(LocalPlayer.Skill4);
            CastFinishSystem(LocalPlayer.Skill5);

            SkillExecutionSystem(LocalPlayer.Id, LocalPlayer.Skill1.State, LocalPlayer.Skill1, tick);
            SkillExecutionSystem(LocalPlayer.Id, LocalPlayer.Skill2.State, LocalPlayer.Skill2, tick);
            SkillExecutionSystem(LocalPlayer.Id, LocalPlayer.Skill3.State, LocalPlayer.Skill3, tick);
            SkillExecutionSystem(LocalPlayer.Id, LocalPlayer.Skill4.State, LocalPlayer.Skill4, tick);
            SkillExecutionSystem(LocalPlayer.Id, LocalPlayer.Skill5.State, LocalPlayer.Skill5, tick);

            var weapon = LocalPlayer.CurrentWeapon;
            var prevWeaponState = weapon.State;

            if (weaponReloadSystem.Execute(LocalPlayer))
            {
                Events.EnqueueClient((LocalPlayer.Events, new WeaponStateChangedEvent(LocalPlayer.Id, prevWeaponState, weapon.State)));

                if (weapon is IConsumableWeapon consumableWeapon)
                {
                    Events.EnqueueClient((LocalPlayer.Events, new AmmoChangeEvent(consumableWeapon.AmmoInClip)));
                }
            }

            prevWeaponState = weapon.State;
            if (weaponResetStateSystem.Execute(LocalPlayer))
            {
                Events.EnqueueClient((LocalPlayer.Events, new WeaponStateChangedEvent(LocalPlayer.Id, prevWeaponState, weapon.State)));
            }

            if (weaponWarmingUpSystem.Execute(LocalPlayer))
            {
                Events.EnqueueClient((
                    LocalPlayer.Events,
                    new WarmingUpProgressChangedEvent(((IWarmingUpWeapon)LocalPlayer.CurrentWeapon).WarmupProgress)));
            }

            while (gameCommandQueue.Count > 0)
            {
                var command = gameCommandQueue.Dequeue();
                var ok = ExecuteCommand(command);

                if (ok)
                {
                    networkService.QueueCommand(new SimulationCommandData(tick, command));
                }
                else
                {
                    SimulationLog.Log.LogTrace("Command {Command} was failed to execute", command.GetType());
                }
            }

            if (weaponResetWarmingUpProgressSystem.Execute(LocalPlayer))
            {
                Events.EnqueueClient((
                    LocalPlayer.Events,
                    new WarmingUpProgressChangedEvent(((IWarmingUpWeapon)LocalPlayer.CurrentWeapon).WarmupProgress)));
            }
        }

        public void UpdateWorldState(SimulationRef serverStateAtServerTick, Tick worldTick, Tick clientTick)
        {
            // ISSUE: v.shimkovich this is initialization of state provider. Can be better with LocalPLayerSpawned event
            if (Game.LocalPlayer == null)
            {
                var players = serverStateAtServerTick.Value.Players.Span;
                for (byte i = 0; i < players.Length; i++)
                {
                    ref var playerState = ref players[i];
                    if (playerState.Id == game.LocalPlayerId)
                    {
                        localPlayerStateProvider.Insert(clientTick, in playerState);
                    }
                }
            }

            var clientState = ClientState.Create(localPlayerStateProvider, worldTick, clientTick);
            game.SetState(serverStateAtServerTick, clientState, Events);
        }

        public void ProcessEvents()
        {
            Events.ProcessServerEvents();
            Events.ProcessClientEvents();
        }

        public void CreatePlayerStateForTheSimulationTick(int tick)
        {
            PlayerState playerState = default;
            LocalPlayer.StateRef.Value.Clone(ref playerState);
            localPlayerStateProvider.Insert(tick, in playerState);

            var playerRef = new LocalRef<PlayerState>(localPlayerStateProvider, tick);
            LocalPlayer.ApplyState(playerRef);
        }

        private void SkillExecutionSystem(EntityId actorId, SkillState prevState, OwnedSkill skill, Tick at)
        {
            if (skillExecutionSystem.Execute(skill))
            {
                Events.EnqueueClient((LocalPlayer.Events, new SkillStateChangedEvent(actorId, skill.Name, prevState, skill.State, at)));
            }
        }

        private void CastFinishSystem(OwnedSkill skill)
        {
            if (finishCastSystem.Execute(skill))
            {
                Events.EnqueueClient((LocalPlayer.Events, new SkillCastChangedEvent(skill.Name, false)));
            }
        }

        private bool ExecuteCommand(IGameCommandData command)
        {
            var weapon = LocalPlayer.CurrentWeapon;
            bool ok = false;
            switch (command)
            {
                case ReloadCommandData rcd:
                    var prevWeaponState = weapon.State;
                    ok = weaponStartReloadSystem.Execute(LocalPlayer);
                    if (ok)
                    {
                        Events.EnqueueClient((LocalPlayer.Events, new WeaponStateChangedEvent(rcd.ActorId, prevWeaponState, weapon.State)));
                    }

                    break;
                case AttackCommandData acd:
                    prevWeaponState = weapon.State;
                    ok = weaponAttackSystem.Execute(LocalPlayer);
                    if (ok)
                    {
                        Events.EnqueueClient((LocalPlayer.Events, new WeaponStateChangedEvent(acd.ActorId, prevWeaponState, weapon.State)));
                    }

                    if (ok && weapon is IConsumableWeapon consumable)
                    {
                        Events.EnqueueClient((LocalPlayer.Events, new AmmoChangeEvent(consumable.AmmoInClip)));
                    }

                    break;
                case MoveCommandData mcd:
                    var moveSystem = new MoveSystem(mcd);
                    ok = moveSystem.Execute(LocalPlayer);
                    break;
                case WarmingUpCommandData wcd:
                    ok = weaponStartWarmingUpSystem.Execute(LocalPlayer);
                    break;
                case UseSkillCommandData scd:
                    ActivateSkillSystem skillSystem = GetActivateSkillSystem(scd.SkillName);
                    ok = ActivateSkill(skillSystem, LocalPlayer, ticker.Current);

                    if (ok)
                    {
                        switch (skillSystem)
                        {
                            case ActivateInstantReloadSkillSystem _ when weapon is IConsumableWeapon consumableWeapon:
                                Events.EnqueueClient((LocalPlayer.Events, new AmmoChangeEvent(consumableWeapon.AmmoInClip)));
                                break;
                            case ActivateLuckySkillSystem _:
                                Events.EnqueueClient((LocalPlayer.Events, new LuckyChangeEvent(true)));
                                break;
                            case ActivateStealthDashSkillSystem _:
                                Events.EnqueueClient((LocalPlayer.Events, new InvisibilityChangeEvent(true)));
                                Events.EnqueueClient((LocalPlayer.Events, new DashChangeEvent(true)));
                                Events.EnqueueClient((LocalPlayer.Events, new SkillCastChangedEvent(scd.SkillName, true)));
                                break;
                            case ActivateInvisibleSkillSystem _:
                            case ActivateStealthSprintSkillSystem _:
                                Events.EnqueueClient((LocalPlayer.Events, new InvisibilityChangeEvent(true)));
                                break;
                            case ActivateMedKitSkillSystem _:
                                Events.EnqueueClient((LocalPlayer.Events, new MedKitUsedEvent(true)));
                                break;
                            case ActivateHealSkillSystem _:
                                Events.EnqueueClient((LocalPlayer.Events, new HealChangeEvent(true)));
                                break;
                            case ActivateCastingSkillSystem _:
                                Events.EnqueueClient((LocalPlayer.Events, new SkillCastChangedEvent(scd.SkillName, true)));
                                break;
                            default:
                                break;
                        }
                    }

                    break;
                case ApplyAoECommandData _:
                    ok = true;
                    break;
                case AimSkillCommandData asd:
                    ok = new AimSkillSystem(ticker, asd.SkillName, asd.IsAiming).Execute(LocalPlayer);
                    if (ok)
                    {
                        Events.EnqueueClient((LocalPlayer.Events, new SkillAimChangeEvent(asd.SkillName, asd.IsAiming)));
                    }

                    break;
                case GrenadeExplosionCommandData _:
                    ok = true;
                    break;
                case CheatCommandData _:
                    ok = true;
                    break;
                default:
                    throw new NotImplementedException($"Provide handler for {command.GetType()}");
            }

            return ok;
        }

        private ActivateSkillSystem GetActivateSkillSystem(SkillName skillName)
        {
            ActivateSkillSystem skillSystem;
            switch (skillName)
            {
                case SkillName.MedKit:
                    skillSystem = new ActivateMedKitSkillSystem(ticker, skillName);
                    break;
                case SkillName.Lifesteal:
                    skillSystem = new ActivateLifestealSkillSystem(ticker, skillName);
                    break;
                case SkillName.Invisibility:
                    skillSystem = new ActivateInvisibleSkillSystem(ticker, skillName);
                    break;
                case SkillName.Lucky:
                    skillSystem = new ActivateLuckySkillSystem(ticker, skillName);
                    break;
                case SkillName.InstantReload:
                    skillSystem = new ActivateInstantReloadSkillSystem(ticker, skillName);
                    break;
                case SkillName.RocketJump:
                    skillSystem = new ActivateJumpSkillSystem(ticker, skillName);
                    break;
                case SkillName.StealthDash:
                case SkillName.DoubleStealthDash:
                    skillSystem = new ActivateStealthDashSkillSystem(ticker, skillName);
                    break;
                case SkillName.Grenade:
                    skillSystem = new ActivateGrenadeSkillSystem(ticker, skillName);
                    break;
                case SkillName.StealthSprint:
                    skillSystem = new ActivateStealthSprintSkillSystem(ticker, skillName);
                    break;
                case SkillName.Heal:
                    skillSystem = new ActivateHealSkillSystem(ticker, skillName);
                    break;
                case SkillName.Dive:
                case SkillName.ShockWave:
                case SkillName.ShockWaveJump:
                    skillSystem = new ActivateCastingSkillSystem(ticker, skillName);
                    break;
                default:
                    skillSystem = new ActivateSkillSystem(ticker, skillName);
                    break;
            }

            return skillSystem;
        }

        private bool ActivateSkill(ActivateSkillSystem skillSystem, ClientPlayer player, Tick at)
        {
            var skill = player.GetFirstOwnedSkill(skillSystem.SkillName);
            var prevState = skill.State;
            if (skillSystem.Execute(player, game))
            {
                var eventData = new SkillStateChangedEvent(player.Id, skillSystem.SkillName, prevState, skill.State, at);
                Events.EnqueueClient((player.Events,eventData));

                SimulationLog.Trace("UseSkillCommandData {eventData}", eventData);

                return true;
            }

            return false;
        }
    }
}