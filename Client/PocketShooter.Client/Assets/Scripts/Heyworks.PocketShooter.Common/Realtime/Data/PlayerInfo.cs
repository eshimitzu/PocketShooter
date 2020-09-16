using System;
using System.Linq;

namespace Heyworks.PocketShooter.Realtime.Data
{
    public class PlayerInfo
    {
        public PlayerInfo(Guid id, string nickname, TrooperInfo[] troopers, ConsumablesInfo consumables)
        {
            this.Id = id;
            this.Nickname = nickname;
            this.Troopers = troopers;
            this.Consumables = consumables;
        }

        private PlayerInfo()
        { }

        public Guid Id { get; private set; }

        public string Nickname { get; private set; }

        public TrooperInfo[] Troopers { get; private set; }
        
        public ConsumablesInfo Consumables { get; private set; }

        public TrooperInfo GetTrooperInfo(TrooperClass trooperClass)
        {
            var info = Troopers.FirstOrDefault(t => t.Class == trooperClass);

            if (info == null)
            {
                throw new InvalidOperationException($"Can't find info for trooper with class {trooperClass}");
            }

            return info;
        }
    }
}
