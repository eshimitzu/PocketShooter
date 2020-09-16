using Heyworks.PocketShooter.Realtime.Configuration.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    // takes defaults from configuration
    public class GradesDefaultsData
    {
        public List<(TrooperClass name, Grade grade)> Troopers {get;set;}
        public List<(WeaponName name, Grade grade)> Weapons {get;set;}
        public List<(HelmetName name, Grade grade)> Helmets {get;set;}

        public List<(ArmorName name, Grade grade)> Armors {get;set;}

    }
}