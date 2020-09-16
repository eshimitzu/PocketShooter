using System.Collections.Generic;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using UnityEngine;

namespace Heyworks.PocketShooter.Networking.Actors
{
    public class BotManager
    {
        private TrooperCreator trooperCreator;

        private List<BotCharacter> bots;

        public IEnumerable<BotCharacter> Bots => bots;

        public BotManager(TrooperCreator trooperCreator)
        {
            this.trooperCreator = trooperCreator;

            bots = new List<BotCharacter>();
        }

        public BotCharacter CreateBotTrooper(PlayerInfo botInfo, IClientPlayer botTrooper, IClientSimulation botClientSimulation, ITickProvider tickProvider, ITickEvents tickEvents, bool isEnemy)
        {
            BotCharacter bot = trooperCreator.CreateBotTrooperWithType(
                botInfo,
                botTrooper.TrooperClass,
                botTrooper,
                tickProvider,
                botClientSimulation,
                tickEvents,
                true);
            bots.Add(bot);

            return bot;
        }

        public void DeleteBot(int botId)
        {
            var leaver = bots.Find(bot => bot.Model.Id == botId);

            if (leaver)
            {
                bots.Remove(leaver);
                Object.Destroy(leaver.gameObject);
            }
        }

        public void Dispose()
        {
            foreach (var bot in bots)
            {
                Object.Destroy(bot.gameObject);
            }

            bots.Clear();
        }
    }
}
