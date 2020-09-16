using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.Realtime.Serialization;
using Microsoft.Extensions.Logging;
using static System.Except;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    public sealed class SimulationCommandsDataSerializer
    {
        // logically-api correct version, but seems not of best performance
        private SafeCyclicSequenceBuffer<IDictionary<CommandHeader, IList<SimulationCommandData>>> simulationsCommandsBuffer =
            new SafeCyclicSequenceBuffer<IDictionary<CommandHeader, IList<SimulationCommandData>>>(Constants.BufferSize);

        private SafeCyclicSequenceBuffer<SimulationMetaCommandData> simulationsMetaCommands = new SafeCyclicSequenceBuffer<SimulationMetaCommandData>(Constants.BufferSize);
        private int possiblyStillAcceptedTick;
        private int lastClientTick = -1; // TODO: do not store, but get from simulationsMetaCommands.
        private int latestConfirmedTick = -1;

        private const ushort maxPossiblePacketSizeInUint = 3000; // very large buffer of 6000 bytes, to large for send commands
        private TypedBitInStream<SimulationStateSerializer> inStream = new TypedBitInStream<SimulationStateSerializer>(maxPossiblePacketSizeInUint);
        private TypedBitOutStream<SimulationStateSerializer> outStream = new TypedBitOutStream<SimulationStateSerializer>(maxPossiblePacketSizeInUint);

        public (SimulationMetaCommandData, PooledList<SimulationCommandData>) Deserialize(byte[] data)
        {
            inStream.FromArray(data);

            var latestPing = inStream.ReadOne<SimulationMetaCommandData>();
            var commands = new PooledList<SimulationCommandData>();
            var tickRange = inStream.ReadIntCount(Constants.CommandsResendCountMax);
            for (int tickDelta = 0; tickDelta < tickRange; tickDelta++)
            {
                var tick = latestPing.Tick - tickDelta;
                var commandsCount = inStream.ReadIntCount(Constants.BufferSize);
                for (int i = 0; i < commandsCount; i++)
                {
                    // TODO: group by actor and send actor id once
                    var code = inStream.ReadOne<SimulationDataCode>();
                    var actorId = inStream.ReadOne<EntityId>();
                    DeserializeCommand(commands, tick, code, actorId);
                }
            }

            return (latestPing, commands);
        }

        // TODO: move switch out to allow independent addition
        private void DeserializeCommand(PooledList<SimulationCommandData> commands, int tick, SimulationDataCode code, EntityId actorId)
        {
            switch (code)
            {
                case SimulationDataCode.Attack:
                    var shotInfo = inStream.ReadArray<ShotInfo>();
                    commands.Add(new SimulationCommandData(tick, new AttackCommandData(actorId, new PooledList<ShotInfo>(shotInfo))));
                    break;

                case SimulationDataCode.UseAoESkill:
                    var shockWaveSkill = inStream.ReadOne<SkillName>();
                    var shockWaveVictims = inStream.ReadArray<EntityId>();
                    commands.Add(new SimulationCommandData(tick, new ApplyAoECommandData(actorId, shockWaveSkill, new PooledList<EntityId>(shockWaveVictims))));
                    break;

                case SimulationDataCode.GrenadeExplosion:
                    var directVictims = inStream.ReadArray<EntityId>();
                    var splashVictims = inStream.ReadArray<EntityId>();
                    commands.Add(new SimulationCommandData(tick, new GrenadeExplosionCommandData(actorId, new PooledList<EntityId>(directVictims), new PooledList<EntityId>(splashVictims))));
                    break;

                case SimulationDataCode.Move:
                    var transform = inStream.ReadOne<FpsTransformComponent>();
                    commands.Add(new SimulationCommandData(tick, new MoveCommandData(actorId, transform)));
                    break;

                case SimulationDataCode.UseSkill:
                    var skillName = inStream.ReadOne<SkillName>();
                    commands.Add(new SimulationCommandData(tick, new UseSkillCommandData(actorId, skillName)));
                    break;

                case SimulationDataCode.WarmingUp:
                    commands.Add(new SimulationCommandData(tick, new WarmingUpCommandData(actorId)));
                    break;

                case SimulationDataCode.Reload:
                    commands.Add(new SimulationCommandData(tick, new ReloadCommandData(actorId)));
                    break;

                case SimulationDataCode.AimSkill:
                    var aimingSkillName = inStream.ReadOne<SkillName>();
                    var isAiming = inStream.ReadBool();
                    commands.Add(new SimulationCommandData(tick, new AimSkillCommandData(actorId, aimingSkillName, isAiming)));
                    break;

                case SimulationDataCode.Cheat:
                    var cheat = inStream.ReadOne<CheatType>();
                    commands.Add(new SimulationCommandData(tick, new CheatCommandData(actorId, cheat)));
                    break;

                default:
                    throw new NotImplementedException($"Not implemented command handler for {code} command");
            }
        }

        public void Add(SimulationCommandData data)
        {
            if (!simulationsMetaCommands.ContainsKey(data.Tick))
            {
                throw InvalidOperation("Should add command's tick.");
            }

            if (data.Tick != lastClientTick)
            {
                throw InvalidOperation("Should add commands only for latest tick.");
            }

            if (!simulationsCommandsBuffer.ContainsKey(data.Tick))
            {
                SafeDispose(data.Tick);
                simulationsCommandsBuffer.Insert(data.Tick, new PooledDictionary<CommandHeader, IList<SimulationCommandData>>(10));
            }

            var commands = simulationsCommandsBuffer[data.Tick];

            var command = data.GameCommandData;

            if (!commands.TryGetValue((command.ActorId, command.Code), out var sameCommandType))
            {
                sameCommandType = new PooledList<SimulationCommandData>(10);
                commands[(command.ActorId, command.Code)] = sameCommandType;
            }

            // NetLog.Log.LogWarning(data.GameCommandData + " for " + data.Tick + " at latest confirmed " + latestConfirmedTick);

            sameCommandType.Add(data);
        }

        public bool HasData => simulationsMetaCommands.ContainsKey(lastClientTick);

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        // cannot use confirmation(mask) to send less or delta, as because when we get these - commands will be stale
        // LZ4 from MessagePack or Gzip from SharpZipLive compression after bit packeting gives same or even larger output
        public byte[] Serialize()
        {
            // TODO: send only client ping as whole, ConfirmedTick could be delta from it.
            // out of scope:
            // 0. proper and better match making and bot balancing
            //
            // options with no user experience degradation:
            // 0. unite actorId into header of all its commands (so will be visible as bots will do casts)
            // 1. delta inside one packet (first packet still full), mostly move - see Gaffer on Games
            // 2. different encoding of counts-is there next the bool true for small collections

            // next may negatively impact user experience:
            // 0. send commands in several packets
            // 1: add notion of only latest commands - e.g. resend 2 last moves instead of 4
            //
            // next will improve state either, no user packets degradation:
            // 0. limit size of collection in call
            // 1. draw 3d plot - mean value, limit, output size and choose limit or not out of stats
            // 1. https://github.com/Unity-Technologies/FPSSample/issues/63 (why these persons collect stats?)
            // 3. encode actorId as shift from total actors
            if (!simulationsMetaCommands.ContainsKey(lastClientTick))
            {
                throw InvalidOperation("Should add command's tick.");
            }

            var lasestPingCommandData = simulationsMetaCommands[lastClientTick];
            outStream.WriteOne(lasestPingCommandData);
            outStream.WriteIntCount(TickRange, Constants.CommandsResendCountMax);

            for (var tick = lastClientTick; tick >= possiblyStillAcceptedTick; tick--)
            {
                if (!simulationsCommandsBuffer.ContainsKey(tick))
                {
                    outStream.WriteIntCount(0, Constants.BufferSize);
                }
                else
                {
                    var simulationsCommands = simulationsCommandsBuffer[tick];
                    if (simulationsCommands == null || simulationsCommands.Count == 0)
                    {
                        outStream.WriteIntCount(0, Constants.BufferSize);
                    }
                    else
                    {
                        CleanUpEvents(tick, simulationsCommands);
                        var toSend = GetCommandsToSend(simulationsCommands);

                        var length = toSend.Count;
                        if (toSend.Count > Constants.BufferSize)
                        {
                            NetLog.Log.LogWarning("Trying to send {commandsCount} more than limit", toSend.Count);
                            length = Constants.BufferSize;
                        }

                        outStream.WriteIntCount(length, Constants.BufferSize);

                        for (var i = 0; i < length; i++)
                        {
                            var command = toSend[i];
                            SerializeCommandData(command);
                        }

                        toSend.Dispose();
                    }
                }
            }

            return outStream.ToArray();
        }

        private void SafeDispose(int tick)
        {
            if (simulationsCommandsBuffer.TryGetItemInPlace(tick, out IDictionary<CommandHeader, IList<SimulationCommandData>> item))
            {
                if (item is PooledDictionary<CommandHeader, IList<SimulationCommandData>> dict)
                {
                    foreach (var simulationCommands in dict.Values)
                    {
                        if (simulationCommands is PooledList<SimulationCommandData> list)
                        {
                            list.Dispose();
                        }
                    }

                    dict.Dispose();
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private void SerializeCommandData(IGameCommandData command)
        {
            outStream.WriteOne(command.Code);
            outStream.WriteOne(command.ActorId);
            SerializeCommand(command);
        }

        private void CleanUpEvents(int tick, IDictionary<CommandHeader, IList<SimulationCommandData>> simulationsCommands)
        {
            // TODO: should fix UI to prevent sending duplicates?
            foreach (var actorCommand in simulationsCommands)
            {
                var command = actorCommand.Key;
                // TODO: automate marking commands as possible to send 2 or not in same tick
                if (command.Code == SimulationDataCode.Move || command.Code == SimulationDataCode.WarmingUp)
                {
                    var uniquCommand = actorCommand.Value;
                    if (uniquCommand.Count > 1)
                    {
                        NetLog.Information("More than one {command} command in one tick", command.Code);
                        var lastFromDublicates = uniquCommand[uniquCommand.Count - 1];
                        uniquCommand.Clear();
                        uniquCommand.Add(lastFromDublicates);
                    }
                }

                if (actorCommand.Key.Code == SimulationDataCode.Move && simulationsCommandsBuffer.ContainsKey(tick - 1))
                {
                    var prev = simulationsCommandsBuffer[tick - 1];
                    if (prev.TryGetValue((command.Id, command.Code), out var previousMoves))
                    {
                        // TODO: simplify this check
                        var previousMove = previousMoves.FirstOrDefault();
                        if (previousMove != null
                            &&
                            previousMove.GameCommandData is MoveCommandData pre)
                        {
                            if (actorCommand.Value.Count > 0 && // why it can be empty?
                                actorCommand.Value.Single().GameCommandData is MoveCommandData curr
                                && pre.Transform.Equals(curr.Transform))
                            {
                                NetLog.Trace("Skip command");
                                actorCommand.Value.Clear();
                            }
                        }
                    }
                }
            }
        }

        private static PooledList<IGameCommandData> GetCommandsToSend(IDictionary<CommandHeader, IList<SimulationCommandData>> simulationsCommands)
        {
            PooledList<IGameCommandData> flat = new PooledList<IGameCommandData>(100);
            foreach (var a in simulationsCommands.Values)
            {
                for (int i = 0; i < a.Count; i++)
                {
                    flat.Add(a[i].GameCommandData);
                }
            }

            if (flat.Count > Constants.BufferSize)
            {
                throw InvalidOperation("Too many commands");
            }

            return flat;
        }

        // TODO: move switch out to allow independent addition
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private void SerializeCommand(IGameCommandData command)
        {
            switch (command.Code)
            {
                case SimulationDataCode.Attack when command is AttackCommandData attack:
                    outStream.Write(attack.Shots.Span);
                    break;

                case SimulationDataCode.UseAoESkill when command is ApplyAoECommandData shockWaveSkill:
                    outStream.WriteOne(shockWaveSkill.Skill);
                    outStream.Write(shockWaveSkill.Victims.Span);
                    break;

                case SimulationDataCode.GrenadeExplosion when command is GrenadeExplosionCommandData grenade:
                    outStream.Write(grenade.DirectVictims.Span);
                    outStream.Write(grenade.SplashVictims.Span);
                    break;

                case SimulationDataCode.Move when command is MoveCommandData move:
                    outStream.WriteOne(in move.Transform);
                    break;

                case SimulationDataCode.UseSkill when command is UseSkillCommandData useSkill:
                    outStream.WriteOne(useSkill.SkillName);
                    break;

                case SimulationDataCode.WarmingUp when command is WarmingUpCommandData warmingUp:
                    break;

                case SimulationDataCode.Reload when command is ReloadCommandData reload:
                    break;

                case SimulationDataCode.AimSkill when command is AimSkillCommandData aimingSkill:
                    outStream.WriteOne(aimingSkill.SkillName);
                    outStream.WriteBool(aimingSkill.IsAiming);
                    break;

                case SimulationDataCode.Cheat when command is CheatCommandData cheat:
                    outStream.WriteOne(cheat.CheatType);
                    break;

                default:
                    throw new NotImplementedException($"Not implemented command handler for {command.Code} command");
            }
        }

        private byte TickRange
        {
            get
            {
                var result = lastClientTick - possiblyStillAcceptedTick + 1;
                if (result > byte.MaxValue)
                    throw InvalidProgram("There is no reason to resend so much ticks");
                if (result < 0)
                    throw InvalidProgram($"{nameof(TickRange)} should be non negative");
                var range = (byte)result;
                if (range > Constants.CommandsResendCountMax)
                    throw InvalidProgram($"{range} is larger than {Constants.CommandsResendCountMax} - should never happen");

                return range;
            }
        }

        public void AddPing(SimulationMetaCommandData pingCommandData, int possiblyStillAcceptedTick)
        {
            if (pingCommandData.ConfirmedTick < latestConfirmedTick)
                throw InvalidOperation($"Trying to confirm {pingCommandData.ConfirmedTick } tick, while sent ${latestConfirmedTick} larger before");

            if (pingCommandData.Tick <= lastClientTick)
                throw InvalidOperation($"Try to add ping for {pingCommandData.Tick} tick, while sent {lastClientTick} before");

            latestConfirmedTick = pingCommandData.ConfirmedTick;
            simulationsMetaCommands.Insert(pingCommandData.Tick, pingCommandData);
            this.possiblyStillAcceptedTick = possiblyStillAcceptedTick;
            lastClientTick = pingCommandData.Tick;
            if (lastClientTick - possiblyStillAcceptedTick >= Constants.CommandsResendCountMax)
            {
                // 3 - 2 >= 2 // false
                // 3 - 1 >= 2 // true
                // 3 - 2 + 1 = 1 + 1 = 2
                this.possiblyStillAcceptedTick = this.lastClientTick - Constants.CommandsResendCountMax + 1;
                // 10 - 4 + 1 = 7 (10 9 8 7)
            }
        }
    }
}
