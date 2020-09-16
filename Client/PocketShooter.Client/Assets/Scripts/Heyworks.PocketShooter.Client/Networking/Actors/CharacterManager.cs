using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using Object = UnityEngine.Object;

namespace Heyworks.PocketShooter.Networking.Actors
{
    /// <summary>
    /// Represents manager for characters. Manager controls spawning and destroying all characters in the game.
    /// </summary>
    public class CharacterManager : ICharacterContainer
    {
        private TrooperCreator trooperCreator;
        private List<NetworkCharacter> characters;
        private NetworkCharacter localCharacter;
        private List<TrooperClass> localCharacterTrooperClasses;

        /// <summary>
        /// Gets the characters list presented in the game.
        /// </summary>
        public IEnumerable<NetworkCharacter> Characters => characters;

        public IEnumerable<TrooperClass> LocalCharacterTrooperClasses => localCharacterTrooperClasses;

        public CharacterManager(TrooperCreator trooperCreator)
        {
            this.trooperCreator = trooperCreator;

            characters = new List<NetworkCharacter>();
            localCharacterTrooperClasses = new List<TrooperClass>(); 
        }

        /// <summary>
        /// Returns all characters inside the circle with radius.
        /// </summary>
        /// <param name="oX">X coord of the center.</param>
        /// <param name="oZ">Z coord of the center.</param>
        /// <param name="radiusSqr">Square radius of the zone.</param>
        public IEnumerable<NetworkCharacter> GetCharactersInsideCircle(float oX, float oZ, float radiusSqr)
        {
            var selected = new List<NetworkCharacter>();
            foreach (var networkCharacter in characters)
            {
                if (networkCharacter.Model.Transform.IsInside(oX, oZ, radiusSqr))
                {
                    selected.Add(networkCharacter);
                }
            }

            return selected;
        }

        /// <summary>
        /// Returns all characters inside the cylinder with radius and height.
        /// </summary>
        /// <param name="oX">X coord of the center.</param>
        /// <param name="oY">Y coord of the center.</param>
        /// <param name="oZ">Z coord of the center.</param>
        /// <param name="radiusSqr">Square radius of the zone.</param>
        /// <param name="height">Height of the zone.</param>
        public IEnumerable<NetworkCharacter> GetCharactersInsideCylinder(float oX, float oY, float oZ, float radiusSqr, float height)
        {
            var selected = new List<NetworkCharacter>();
            foreach (var networkCharacter in characters)
            {
                if (networkCharacter.Model.Transform.IsInsideCylinder(oX, oY, oZ, radiusSqr, height))
                {
                    selected.Add(networkCharacter);
                }
            }

            return selected;
        }

        public LocalCharacter CreateLocalCharacter(
            IClientPlayer player,
            IClientSimulation simulation,
            ITickEvents tickEvents)
        {
            LocalCharacter localCharacter =
                trooperCreator.CreateLocalTrooperWithType(
                    player,
                    simulation,
                    tickEvents,
                    this);
            characters.Add(localCharacter);
            this.localCharacter = localCharacter;

            localCharacterTrooperClasses.Add(player.TrooperClass);

            return localCharacter;
        }

        public RemoteCharacter CreateRemoteCharacter(
            IRemotePlayer remotePlayer,
            TrooperClass trooperClass,
            TeamNo localTeam,
            ITickProvider tickProvider,
            IClientSimulation simulation)
        {
            var character = trooperCreator.CreateRemoteTrooperWithType(
                trooperClass,
                remotePlayer.CurrentWeapon.Name,
                localTeam != remotePlayer.Team,
                remotePlayer.Id,
                remotePlayer,
                tickProvider,
                simulation,
                this); // TODO: fix to pass RemotePlayer only (?)

            characters.Add(character);

            return character;
        }

        public void DeleteCharacter(IPlayerEntity playerEntity)
        {
            DeleteCharacter(playerEntity.Id);
        }

        public void DeleteCharacter(int playerId)
        {
            NetworkCharacter leaver = GetCharacter(playerId);

            if (leaver)
            {
                characters.Remove(leaver);
                Object.Destroy(leaver.gameObject);
            }
        }

        public void DeleteRemoteCharacters()
        {
            foreach (var character in characters.ToList())
            {
                if (character != localCharacter)
                {
                    Object.Destroy(character.gameObject);
                    characters.Remove(character);
                }
            }
        }

        public void Dispose()
        {
            foreach (var networkCharacter in Characters)
            {
                Object.Destroy(networkCharacter.gameObject);
            }

            characters.Clear();
        }

        /// <summary>
        /// Gets the character by player id. Returns null if not found.
        /// </summary>
        /// <param name="playerId">The player identifier.</param>
        public NetworkCharacter GetCharacter(int playerId)
        {
            return characters.Find(c => c.Id == playerId);
        }
    }
}
