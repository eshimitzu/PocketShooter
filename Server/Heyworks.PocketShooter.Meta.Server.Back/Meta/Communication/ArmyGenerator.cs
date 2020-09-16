using System;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Data;
using MoreLinq;

namespace Heyworks.PocketShooter.Meta.Communication
{
    public class ArmyGenerator
    {
        public ArmyGenerator()
        {
        }

        public ServerArmyState Generate(Guid playerId, int level, BotsTrainConfig config, GradesDefaultsData classGrades)
        {
            // we generate for now, as game will settle out - may save output and use it to speed up, but non essential for now

            // except class grades could filter by weapon-armor-helmet, it could be discussed so, 
            // but except it needs more complex error-data prone algorithm (chances cannot play of bad config)
            // it will make game more boring given our bots are quite silly (so this may be discussed)
            
            var army = new ServerArmyState(playerId);
            foreach (var item in classGrades.Armors.Where(x=> x.grade <= config.MaximalGrade))
                army.Armors.Add(new ArmorState { Grade = item.grade, Level = level, Name = item.name });
            foreach (var item in classGrades.Helmets.Where(x=> x.grade <= config.MaximalGrade))
                army.Helmets.Add(new HelmetState { Grade = item.grade, Level = level, Name = item.name });
            foreach (var item in classGrades.Weapons.Where(x=> x.grade <= config.MaximalGrade))
                army.Weapons.Add(new WeaponState { Grade = item.grade, Level = level, Name = item.name });

            for (var i = (int)Grade.Common; i <= (int)config.MaximalGrade; i++)
            {
                var troopers = classGrades.Troopers.Where(x => x.grade == (Grade)i);
                foreach (var trooper in troopers)
                {
                    var state = new TrooperState();
                    state.Class = trooper.name;
                    state.Level = level;
                    state.Grade = trooper.grade;
                    state.CurrentWeapon = army.Weapons.RandomSubset(1).Single().Name;
                    state.CurrentArmor = army.Armors.RandomSubset(1).Single().Name;
                    state.CurrentHelmet = army.Helmets.RandomSubset(1).Single().Name;
                    army.Troopers.Add(state);
                }
            }

            return army;
        }
    }
}