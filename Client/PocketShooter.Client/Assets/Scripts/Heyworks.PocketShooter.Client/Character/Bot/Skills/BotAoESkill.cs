using System.Collections.Generic;
using System.Linq;
using Collections.Pooled;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    public class BotAoESkill
    {
        private readonly SkillName skillName;
        private readonly BotCharacter character;
        private readonly AoEInfo area;

        public BotAoESkill(SkillName skillName, BotCharacter character, AoEInfo area)
        {
            this.skillName = skillName;
            this.character = character;
            this.area = area;
        }

        public void Execute()
        {
            Vector3 position = character.transform.position;
            var victimIds = GetCharactersInsideCylinder(
                    position.x,
                    position.y,
                    position.z,
                    area.RadiusSqr,
                    area.Height).Where(v => v.Team != character.Model.Team)
                .Select(v => v.Id);

            character.AddCommand(
                new ApplyAoECommandData(
                    character.Id,
                    skillName,
                    new PooledList<EntityId>(victimIds)));
        }

        public IEnumerable<RemotePlayer> GetCharactersInsideCylinder(float oX, float oY, float oZ, float radiusSqr, float height)
        {
            var selected = new List<RemotePlayer>();
            foreach (RemotePlayer remotePlayer in character.Simulation.Game.Players.Values)
            {
                if (remotePlayer.Transform.IsInsideCylinder(oX, oY, oZ, radiusSqr, height))
                {
                    selected.Add(remotePlayer);
                }
            }

            return selected;
        }
    }
}