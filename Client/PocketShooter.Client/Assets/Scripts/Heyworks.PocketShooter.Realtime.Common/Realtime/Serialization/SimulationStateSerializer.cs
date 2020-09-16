using System;
using System.Collections.Generic;
using System.Diagnostics;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.Realtime.Serialization;
using Microsoft.Extensions.Logging;
using NetStack.Serialization;

namespace Heyworks.PocketShooter.Realtime.Serialization
{
    // TODO: propagate empty dictionary, array pool, buffer, dummy from above so these are reused
    public sealed class SimulationStateSerializer : IDeltaDataSerializer<SimulationState>
    {
        private const ushort maxPossiblePacketSizeInUint = 1000;
        private const int dummyTick = -1;

        private static readonly PlayerState dummyPlayer;
        private static readonly SimulationState dummyState;

        static SimulationStateSerializer()
        {
            dummyState = new SimulationState(dummyTick, default, default);
        }

        private TypedBitInStream<SimulationStateSerializer> inStream = new TypedBitInStream<SimulationStateSerializer>(maxPossiblePacketSizeInUint);
        private TypedBitOutStream<SimulationStateSerializer> outStream = new TypedBitOutStream<SimulationStateSerializer>(maxPossiblePacketSizeInUint);

        // TODO: connect size value with disconnect-timeout. where size = disconectTimeInTicks
        // TODO: v.shimkovich this field is workaround, simulationStateProvider could be used instead(but brokes).
        private SafeCyclicSequenceBuffer<SimulationState> untouchedClones;

        public SimulationStateSerializer(int serializationBufferSize)
        {
            untouchedClones = new SafeCyclicSequenceBuffer<SimulationState>(serializationBufferSize);
        }

        public byte[] Serialize(in SimulationState? baseline, in SimulationState updated)
        {
            SimulationState state = baseline ?? dummyState;
            outStream.WriteInt(state.Tick);
            outStream.WriteInt(updated.Tick);
            outStream.WriteInt(updated.ServerInputBufferSize);
            outStream.WriteDiff(in state.GameState.Team1, in updated.GameState.Team1);
            outStream.WriteDiff(in state.GameState.Team2, in updated.GameState.Team2);

            // TODO: diff zones or do one time delivered protocall (once, delivered, never updated)
            outStream.Write(new ReadOnlySpan<ZoneState>(updated.GameState.Zones), Constants.MaxZonesCount);

            outStream.WriteByte((byte)updated.GameState.Players.Count);
            EnsureUnique(updated.GameState.Players);
            var playerIndex = BuildEntityIndex(state.GameState.Players);

            for (int i = 0; i < updated.GameState.Players.Count; i++)
            {
                ref var updatedPlayer = ref updated.GameState.Players.Span[i];
                if (playerIndex.TryGetValue(updatedPlayer.Id, out var pIndex))
                {
                    NetLog.Trace("Found baseline player for {playerId} at {baselineTick} for {updateTick}  updated tick", updatedPlayer.Id, state.Tick, updated.Tick);
                    WritePlayerState(in state.GameState.Players.Span[pIndex], in updatedPlayer, outStream);
                }
                else
                {
                    NetLog.Trace("Using dummy baseline for {playerId} at {baselineTick} for {updateTick} updated tick", updatedPlayer.Id, state.Tick, updated.Tick);
                    WritePlayerState(in dummyPlayer, in updatedPlayer, outStream);
                }
            }

            var result = outStream.ToArray();
            if (result.Length > BitBufferLimits.MtuIeee802Dot3)
            {
                NetLog.Log.LogWarning("Will send {SendBytes} commands' bytes to server", result.Length);
            }

            return result;
        }

        public SimulationState? Deserialize(byte[] data)
        {
            inStream.FromArray(data);

            var baselineTick = inStream.ReadInt(); // check and throw if not has baseline
            var updatedServerTick = inStream.ReadInt();

            ref readonly var baseline = ref baselineTick != dummyTick && HasBaseline(baselineTick, updatedServerTick)
                ? ref untouchedClones[baselineTick]
                : ref dummyState; // server sends us full diff (hopefully)

            if (!untouchedClones.CanInsert(updatedServerTick))
            {
                NetLog.Log.LogWarning("{Tick} cannot be inserted. Is too old or exists", updatedServerTick);
                return null;
            }

            var serverInputBufferSize = inStream.ReadInt();
            var team1 = inStream.ReadDiff(in baseline.GameState.Team1);
            var team2 = inStream.ReadDiff(in baseline.GameState.Team2);
            // TODO: diff zones or do one time delivered protocol (once, delivered, never updated)
            // TODO: mark zones as small size collection and write/read byte of Length (performance) (but no so essential after diff)
            var zones = inStream.ReadArray<ZoneState>(Constants.MaxZonesCount);

            // players
            var playersIndex = BuildEntityIndex(baseline.GameState.Players);
            var playersCount = inStream.ReadByte();
            var players = new PooledList<PlayerState>(playersCount);
            for (int i = 0; i < playersCount; i++)
            {
                var playerId = inStream.ReadOne<EntityId>();

                if (playerId == default)
                {
                    Throw.InvalidOperation("Id cannot be default");
                }

                if (playersIndex.TryGetValue(playerId, out var pIndex))
                {
                    NetLog.Trace("Found baseline player for {playerId} at {baselineTick} for {updateTick} updated tick", playerId, baselineTick, updatedServerTick);
                    ref readonly var baselinePlayer = ref baseline.GameState.Players.Span[pIndex];
                    var deserialized = ReadPlayerState(playerId, in baselinePlayer, inStream);
                    EnsureValidPlayer(in deserialized);
                    players.Add(deserialized);
                }
                else
                {
                    NetLog.Trace("Using dummy baseline for {playerId} at {baselineTick} for {updateTick} updated tick", playerId, baselineTick, updatedServerTick);
                    var newPlayer = ReadPlayerState(playerId, in dummyPlayer, inStream);
                    EnsureValidPlayer(in newPlayer);
                    players.Add(newPlayer);
                }
            }

            EnsureUnique(players);

            // we clone here to avoid overwrite by other code above, but can optimize to reuse this buffer inside top item
            var newState = new SimulationState(updatedServerTick, serverInputBufferSize, new GameState(team1, team2, zones, players));
            if (untouchedClones.TryGetItemInPlace(updatedServerTick, out SimulationState state))
            {
                state.GameState.Dispose();
            }

            untouchedClones.Insert(updatedServerTick, newState);
            return newState.CloneWithNewTick(updatedServerTick);
        }

        [Conditional("DEBUG")]
        private static void EnsureValidPlayer(in PlayerState player)
        {
            if (!Enum.IsDefined(typeof(WeaponName), player.Weapon.Base.Name))
            {
                Throw.InvalidProgram($"Weapon {player.Weapon.Base.Name} is not defined");
            }
        }

        private bool HasBaseline(int baselineTick, int newTick)
        {
            Debug.Assert(baselineTick >= 0);

            var result = untouchedClones.ContainsKey(baselineTick); // server knows we should have that baseline if he sends us that

            if (!result)
            {
                Throw.NotImplemented(
                    $"Failed to find baseline tick {baselineTick} for new tick {newTick}. Looks like we can just skip the state. If this error appears, implement this logic");
            }

            return result;
        }

        [Conditional("DEBUG")]
        private void EnsureUnique(PooledList<PlayerState> players)
        {
            if (players != null && players.Count > 0)
            {
                var hashSet = new PooledSet<EntityId>();
                for (int i = 0; i < players.Count; i++)
                {
                    ref readonly var player = ref players.Span[i];
                    if (hashSet.Contains(player.Id))
                    {
                        Throw.InvalidProgram($"Duplicate entity {player.Id} ID in same world should never happen");
                    }
                    else
                        hashSet.Add(player.Id);
                }
            }
        }

        // players count is small and collection is pooled
        private static IReadOnlyDictionary<EntityId, int> BuildEntityIndex<TEntity>(IReadOnlyPooledList<TEntity> baselineEntities)
            where TEntity : IEntity<EntityId>
        {
            if (baselineEntities == null)
            {
                return new PooledDictionary<EntityId, int>();
            }

            var pooledHash = new PooledDictionary<EntityId, int>(baselineEntities.Count);
            for (var i = 0; i < baselineEntities.Count; i++)
            {
                ref readonly var entity = ref baselineEntities.Span[i];
                pooledHash[entity.Id] = i;
            }

            return pooledHash;
        }

        private static PlayerState ReadPlayerState(EntityId id, in PlayerState baseline, TypedBitInStream<SimulationStateSerializer> inStream)
        {
            var player = new PlayerState(
                id,
                inStream.ReadStringDiff(baseline.Nickname),
                inStream.ReadDiff(baseline.TrooperClass),
                inStream.ReadOne<TeamNo>(), // no reason to base line, better Enum.GetMax and compress that way
                inStream.ReadDiff(in baseline.Transform),
                inStream.ReadDiff(in baseline.Health),
                inStream.ReadDiff(in baseline.Armor),
                inStream.ReadDiff(in baseline.Weapon),
                inStream.ReadDiff(in baseline.Skill1),
                inStream.ReadDiff(in baseline.Skill2),
                inStream.ReadDiff(in baseline.Skill3),
                inStream.ReadDiff(in baseline.Skill4),
                inStream.ReadDiff(in baseline.Skill5),
                inStream.ReadDiff(in baseline.Effects),
                inStream.ReadDiff(in baseline.ServerEvents),

                new PooledList<ShotInfo>(inStream.ReadArray<ShotInfo>()),
                new PooledList<DamageInfo>(inStream.ReadArray<DamageInfo>()),
                new PooledList<HealInfo>(inStream.ReadArray<HealInfo>()));

            EnsureValidPlayer(in player);
            return player;
        }

        private static void WritePlayerState(in PlayerState baseline, in PlayerState updated, TypedBitOutStream<SimulationStateSerializer> outStream)
        {
            if (updated.Id == default) Throw.InvalidProgram("Must not send default entity id");

            outStream.WriteOne(updated.Id); // Id should not be baselined - add exception into serializer
            outStream.WriteStringDiff(baseline.Nickname, updated.Nickname);
            outStream.WriteDiff(baseline.TrooperClass, updated.TrooperClass);
            outStream.WriteOne(updated.Team);
            outStream.WriteDiff(in baseline.Transform, in updated.Transform);
            outStream.WriteDiff(in baseline.Health, in updated.Health);
            outStream.WriteDiff(in baseline.Armor, in updated.Armor);
            outStream.WriteDiff(in baseline.Weapon, in updated.Weapon);
            outStream.WriteDiff(in baseline.Skill1, in updated.Skill1);
            outStream.WriteDiff(in baseline.Skill2, in updated.Skill2);
            outStream.WriteDiff(in baseline.Skill3, in updated.Skill3);
            outStream.WriteDiff(in baseline.Skill4, in updated.Skill4);
            outStream.WriteDiff(in baseline.Skill5, in updated.Skill5);
            outStream.WriteDiff(in baseline.Effects, in updated.Effects);
            outStream.WriteDiff(in baseline.ServerEvents, in updated.ServerEvents);

            outStream.Write(updated.Shots.Span);
            outStream.Write(updated.Damages.Span);
            outStream.Write(updated.Heals.Span);
        }
    }
}
