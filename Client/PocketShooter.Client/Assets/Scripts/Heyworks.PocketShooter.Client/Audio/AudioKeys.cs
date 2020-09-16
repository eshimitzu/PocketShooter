#pragma warning disable SA1600
namespace Heyworks.PocketShooter.Audio
{
    /// <summary>
    /// Represents collection of audio keys.
    /// </summary>
    public static class AudioKeys
    {
        /// <summary>
        /// Names of events.
        /// </summary>
        public static class Event
        {
            public const string PlayRocketExplosion = "PlayRocketExplosion";
            public const string PlayButtonReload = "PlayButtonReload";
            public const string PlayStunEffect = "PlayStunEffect";
            public const string PlaySprint = "PlaySprint";
            public const string StopSprint = "StopSprint";
            public const string PlayTrooperImpact = "PlayTrooperImpact";
            public const string PlayWallImpact = "PlayWallImpact";
            public const string PlayGrenadeExplosion = "PlayGrenadeExplosion";
            public const string PlayFootsteps = "PlayFootsteps";
            public const string PlayHealing = "PlayHealing";
            public const string PlayRegeneration = "PlayRegeneration";
            public const string PlayTrooperSelect = "PlayTrooperSelect";
            public const string PlayWarmUpMinigun = "PlayWarmUpMinigun";
            public const string PlayWarmUpStopMinigun = "PlayWarmUpStopMinigun";
            public const string PlayInvisibility = "PlayInvisibility";
            public const string PlayShockwave = "PlayShockwave";
            public const string PlayShockwaveHorn = "PlayShockwaveHorn";
            public const string PlayReload = "PlayReload";
            public const string PlayDive = "PlayDive";
            public const string PlayDiveAim = "PlayDiveAim";
            public const string PlayDiveRocket = "PlayDiveRocket";
            public const string PlayDiveExplosion = "PlayDiveExplosion";
            public const string PlayLifestealPlus = "PlayLifestealPlus";
            public const string PlayLifestealMinus = "PlayLifestealMinus";
            public const string PlayLifesteal = "PlayLifesteal";
            public const string StopLifesteal = "StopLifesteal";
            public const string PlayStartMatchButton = "PlayStartMatchButton";
            public const string PlayButtonClick = "PlayButtonClick";
            public const string PlaySuffer = "PlaySuffer";
            public const string PlayDeath = "PlayDeath";
            public const string PlayNeoFocus = "PlayNeoFocus";
            public const string PlayNorrisFocus = "PlayNorrisFocus";
            public const string PlayRamboFocus = "PlayRamboFocus";
            public const string PlayRockFocus = "PlayRockFocus";
            public const string PlayScoutFocus = "PlayScoutFocus";
            public const string PlaySniperFocus = "PlaySniperFocus";
            public const string PlaySpyFocus = "PlaySpyFocus";
            public const string PlayStathamFocus = "PlayStathamFocus";
            public const string ResumeLobbyMusic = "ResumeLobbyMusic";
            public const string PlayImmortalitySphere = "PlayImmortalitySphere";
            public const string PlayImmortalityImpact = "PlayImmortalityImpact";
            public const string StopImmortalitySphere = "StopImmortalitySphere";
            public const string PlayTrap = "PlayTrap";
            public const string PlayInstantReload = "PlayInstantReload";
            public const string PlayLucky = "PlayLucky";
            public const string PlayRage = "PlayRage";
            public const string PlayZoneProgressEnemy = "PlayZoneProgressEnemy";
            public const string StopZoneProgressEnemy = "StopZoneProgressEnemy";
            public const string PlayZoneProgressFriend = "PlayZoneProgressFriend";
            public const string StopZoneProgressFriend = "StopZoneProgressFriend";
            public const string PlayDash = "PlayDash";

        }

        /// <summary>
        /// Names of states.
        /// </summary> 
        public static class State
        {
            public const string None = "None";
            public const string EffectStun = "EffectStun";
            public const string CharacterDead = "CharacterDead";
        }

        /// <summary>
        /// Names of state groups.
        /// </summary>
        public static class StateGroup
        {
            public const string EffectStun = "EffectStun";
            public const string CharacterLocal = "CharacterLocal";
        }

        public static class RTPC
        {
            public const string WarmupConsumableWeapon = "WarmupConsumableWeapon";
            public const string SprintActive = "SprintActive";
            public const string ZoneProgressEnemy = "ZoneProgressEnemy";
            public const string ZoneProgressFriend = "ZoneProgressFriend";


        }
    }
}
