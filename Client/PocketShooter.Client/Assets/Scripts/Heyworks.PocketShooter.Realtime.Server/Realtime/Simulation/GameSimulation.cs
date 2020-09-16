using System;
using System.Collections.Generic;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Systems;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    public sealed class GameSimulation : ISimulation
    {
        private readonly IServerGame game;
        private readonly ILogger logger;
        private readonly SafeCyclicSequenceBuffer<PooledList<IGameCommandData>> commandSetProvider;
        private readonly ITicker ticker;

        private readonly CleanUpSystem cleanUpSystem;

        private readonly WeaponReloadSystem weaponReloadSystem;
        private readonly WeaponResetStateSystem weaponResetStateSystem;
        private readonly WeaponAttackSystem weaponAttackSystem;
        private readonly WeaponStartReloadSystem weaponStartReloadSystem;

        private readonly WeaponWarmingUpSystem weaponWarmingUpSystem;
        private readonly WeaponStartWarmingUpSystem weaponStartWarmingUpSystem;
        private readonly WeaponResetWarmingUpProgressSystem weaponResetWarmingUpProgressSystem;

        private readonly MedKitSystem medKitSystem;
        private readonly MedKitStopSystem medKitStopSystem;
        private readonly HealSystem healSystem;
        private readonly HealStopSystem healStopSystem;

        private readonly HealingSystem healingSystem;
        private readonly DamageSystem damageSystem;
        private readonly LuckyDiscardSystem luckyDiscardSystem;

        private readonly StunSystem stunSystem;
        private readonly StunningHitSkillSystem stunningHitSkillSystem;

        private readonly JumpSystem jumpSystem;
        private readonly RootSystem rootSystem;
        private readonly RootingHitSkillSystem rootingHitSkillSystem;
        private readonly ActivateImmortalitySkillSystem activateImmortalitySkillSystem;
        private readonly ExtraDamageSkillSystem extraDamageSystem;
        private readonly ControlFreakSkillSystem controlFreakSystem;
        private readonly InvisibleSystem invisibleSystem;
        private readonly ImmortalityStopSystem immortalityStopSystem;
        private readonly LifestealSkillSystem lifestealSystem;
        private readonly LifeDrainSkillSystem lifeDrainSystem;
        private readonly RageSkillSystem rageSystem;
        private readonly RegenerationOnKillSkillSystem regenerationOnKillSystem;
        private readonly DashSystem dashSystem;

        private readonly SkillExecutionSystem skillExecutionSystem;
        private readonly FinishCastSkillSystem finishCastSystem;

        private readonly EffectsInteractionSystem effectsInteractionSystem;

        private readonly ZoneCaptureSystem zoneCaptureSystem;
        private readonly EndGameSystem endGameSystem;

        public GameSimulation(
            IServerGame game,
            DominationModeInfo modeInfo,
            // TODO: mark this as readonly only
            SafeCyclicSequenceBuffer<PooledList<IGameCommandData>> commandSetProvider,
            ITicker ticker,
            ILogger logger)
        {
            this.game = game;
            this.logger = logger;
            this.commandSetProvider = commandSetProvider;
            this.ticker = ticker;

            zoneCaptureSystem = new ZoneCaptureSystem(ticker, modeInfo.CheckInterval, modeInfo.TimeplayersToCapture);
            endGameSystem = new EndGameSystem(ticker, modeInfo.WinScore, modeInfo.GameDuration);

            // weapon
            weaponReloadSystem = new WeaponReloadSystem(ticker);
            weaponResetStateSystem = new WeaponResetStateSystem(ticker);
            weaponAttackSystem = new WeaponAttackSystem(ticker);
            weaponStartReloadSystem = new WeaponStartReloadSystem(ticker);

            // warming up
            weaponStartWarmingUpSystem = new WeaponStartWarmingUpSystem();
            weaponWarmingUpSystem = new WeaponWarmingUpSystem();
            weaponResetWarmingUpProgressSystem = new WeaponResetWarmingUpProgressSystem();

            // healing
            medKitSystem = new MedKitSystem(ticker);
            medKitStopSystem = new MedKitStopSystem(ticker);
            healSystem = new HealSystem(ticker);
            healStopSystem = new HealStopSystem(ticker);

            healingSystem = new HealingSystem();

            cleanUpSystem = new CleanUpSystem();

            // skills
            skillExecutionSystem = new SkillExecutionSystem(ticker);
            finishCastSystem = new FinishCastSkillSystem(ticker);
            jumpSystem = new JumpSystem(ticker);
            stunSystem = new StunSystem(ticker);
            stunningHitSkillSystem = new StunningHitSkillSystem(ticker);
            rootSystem = new RootSystem(ticker);
            rootingHitSkillSystem = new RootingHitSkillSystem(ticker);
            invisibleSystem = new InvisibleSystem(ticker);
            lifestealSystem = new LifestealSkillSystem(ticker);
            extraDamageSystem = new ExtraDamageSkillSystem(ticker);
            activateImmortalitySkillSystem = new ActivateImmortalitySkillSystem(ticker);
            immortalityStopSystem = new ImmortalityStopSystem(ticker);
            lifeDrainSystem = new LifeDrainSkillSystem(ticker, modeInfo.GameArmorInfo);
            regenerationOnKillSystem = new RegenerationOnKillSkillSystem(ticker, modeInfo.GameArmorInfo);
            rageSystem = new RageSkillSystem(ticker);
            dashSystem = new DashSystem(ticker);
            controlFreakSystem = new ControlFreakSkillSystem(ticker);

            effectsInteractionSystem = new EffectsInteractionSystem();
            damageSystem = new DamageSystem(modeInfo.GameArmorInfo);
            luckyDiscardSystem = new LuckyDiscardSystem(ticker);

            // Run Initialize systems
            endGameSystem.Initialize(game);
        }

        public void Update()
        {
            // RESETTING/FINISHING SYSTEMS - systems which should depend only on current tick and timer.
            // They reset states of effects that ended by this tick. Should be executed first to not influence on other systems
            cleanUpSystem.Execute(game);

            jumpSystem.Execute(game);
            stunSystem.Execute(game);
            rootSystem.Execute(game);
            invisibleSystem.Execute(game);
            immortalityStopSystem.Execute(game);
            luckyDiscardSystem.Execute(game);
            dashSystem.Execute(game);

            weaponReloadSystem.Execute(game);
            weaponResetStateSystem.Execute(game);

            weaponWarmingUpSystem.Execute(game);

            medKitStopSystem.Execute(game);
            healStopSystem.Execute(game);

            finishCastSystem.Execute(game);

            skillExecutionSystem.Execute(game);
            // RESETTING/FINISHING SYSTEMS

            // TODO: v.shimkovich INVESTIGATION REQUIRED. some systems in this block probably should run after Affecting systems. MOST ARE NOT
            // EXECUTION OF PLAYER INPUT - applies player input taking into account current state of the game
            if (commandSetProvider.ContainsKey(ticker.Current))
            {
                var commandSet = commandSetProvider[ticker.Current];
                foreach (var command in commandSet)
                {
                    if (game.Players.TryGetValue(command.ActorId, out var player))
                    {
                        try
                        {
                            ExecuteCommand(command, player);
                        }
                        catch (NotImplementedException ex)
                        {
                            // should we allow execution of all others players commands here? and do only publish error into specific player channel?
                            // we could handle errors on on message by message basis - and send client errors to specific clients and handle other case
                            // e.g. like here https://github.com/dnikolovv/dev-adventures-realworld or https://github.com/Resultful/Resultful
                            // i.e. these works well on per client error handling (error as data) and of cheaper than throw error
                            throw new ClientException(ClientErrorCode.CommandNotSupported, command.ActorId, "Command is not here. Wrong client?", ex);
                        }
                    }
                    else
                    {
                        // TODO: do not add command for which there is no player, all commands which are send should act upon, so log warning
                        //logger.LogInformation("Player {Id} was removed before command executed", command.ActorId);
                    }
                }
            }
            // EXECUTION OF PLAYER INPUT

            // AFFECTING SYSTEMS - Passive and activated skills. They apply effects on players, calculate heals and damages
            medKitSystem.Execute(game);
            healSystem.Execute(game);
            weaponResetWarmingUpProgressSystem.Execute(game);
            activateImmortalitySkillSystem.Execute(game);
            stunningHitSkillSystem.Execute(game);
            rootingHitSkillSystem.Execute(game);
            lifestealSystem.Execute(game);
            lifeDrainSystem.Execute(game);
            regenerationOnKillSystem.Execute(game);
            extraDamageSystem.Execute(game);
            controlFreakSystem.Execute(game);
            rageSystem.Execute(game);

            // AFFECTING SYSTEMS

            // POST EFFECT SYSTEMS - logic depending on all applied effects, incoming heals and damages
            effectsInteractionSystem.Execute(game);
            // POST EFFECT SYSTEMS

            // HEAL AND DAMAGE CALCULATION - only now player health is calculated.
            healingSystem.Execute(game);
            damageSystem.Execute(game);
            // HEAL AND DAMAGE CALCULATION

            // GAME SYSTEMS - not impact on players but calculate game state depending on players state.
            zoneCaptureSystem.Execute(game);
            endGameSystem.Execute(game);
            // GAME SYSTEMS
        }

        private bool ExecuteCommand(IGameCommandData command, ServerPlayer player)
        {
            // TODO: Use command.Code which is faster than type switch
            bool ok = false;
            switch (command)
            {
                case ReloadCommandData rcd:
                    ok = weaponStartReloadSystem.Execute(player);
                    break;
                case AttackCommandData acd:
                    if (weaponAttackSystem.Execute(player))
                    {
                        ok = new WeaponDamageSystem(acd).Execute(player, game);
                    }

                    break;
                case MoveCommandData mcd:
                    ok = new MoveSystem(mcd).Execute(player);
                    break;
                case WarmingUpCommandData wcd:
                    ok = weaponStartWarmingUpSystem.Execute(player);
                    break;
                case UseSkillCommandData scd:
                    ok = UseSkill(player, scd);
                    break;
                case ApplyAoECommandData sws:
                    ok = ApplyAoE(player, sws);
                    break;
                case AimSkillCommandData csd:
                    ok = new AimSkillSystem(ticker, csd.SkillName, csd.IsAiming).Execute(player);
                    break;
                case GrenadeExplosionCommandData gcd:
                    ok = new GrenadeExplosionSystem(gcd).Execute(player, game);
                    break;
                case CheatCommandData ccd:
                    ok = new CheatSystem(ticker, game, ccd).Execute(player);
                    break;
                default:
                    throw new NotImplementedException($"Provide handler for {command.GetType()}");
            }

            return ok;
        }

        private bool UseSkill(ServerPlayer player, UseSkillCommandData scd)
        {
            bool ok;
            switch (scd.SkillName)
            {
                case SkillName.MedKit:
                    ok = new ActivateMedKitSkillSystem(ticker, scd.SkillName).Execute(player, game);
                    break;
                case SkillName.Lifesteal:
                    ok = new ActivateLifestealSkillSystem(ticker, scd.SkillName).Execute(player, game);
                    break;
                case SkillName.Invisibility:
                    ok = new ActivateInvisibleSkillSystem(ticker, scd.SkillName).Execute(player, game);
                    break;
                case SkillName.Lucky:
                    ok = new ActivateLuckySkillSystem(ticker, scd.SkillName).Execute(player, game);
                    break;
                case SkillName.InstantReload:
                    ok = new ActivateInstantReloadSkillSystem(ticker, scd.SkillName).Execute(player, game);
                    break;
                case SkillName.RocketJump:
                    ok = new ActivateJumpSkillSystem(ticker, scd.SkillName).Execute(player, game);
                    break;
                case SkillName.StealthDash:
                case SkillName.DoubleStealthDash:
                    ok = new ActivateStealthDashSkillSystem(ticker, scd.SkillName).Execute(player, game);
                    break;
                case SkillName.Grenade:
                    ok = new ActivateGrenadeSkillSystem(ticker, scd.SkillName).Execute(player, game);
                    break;
                case SkillName.StealthSprint:
                    ok = new ActivateStealthSprintSkillSystem(ticker, scd.SkillName).Execute(player, game);
                    break;
                case SkillName.Heal:
                    ok = new ActivateHealSkillSystem(ticker, scd.SkillName).Execute(player, game);
                    break;
                case SkillName.ShockWave:
                case SkillName.ShockWaveJump:
                case SkillName.Dive:
                    ok = new ActivateCastingSkillSystem(ticker, scd.SkillName).Execute(player, game);
                    break;
                default:
                    ok = new ActivateSkillSystem(ticker, scd.SkillName).Execute(player, game);
                    break;
            }

            return ok;
        }

        private bool ApplyAoE(ServerPlayer player, ApplyAoECommandData scd)
        {
            bool ok;
            switch (scd.Skill)
            {
                case SkillName.ShockWave:
                case SkillName.ShockWaveJump:
                    ok = new ShockWaveSystem(ticker, scd).Execute(player, game);
                    break;
                case SkillName.Dive:
                    ok = new DiveSystem(ticker, scd).Execute(player, game);
                    break;
                default:
                    throw new NotImplementedException($"UseAoESkill method for skill {scd.Skill} is not implemented.");
            }

            return ok;
        }
    }
}