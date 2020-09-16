using System;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using UniRx;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class Player : PlayerBase
    {
        private readonly Subject<string> nicknameChanged = new Subject<string>();
        private readonly Subject<int> cashChanged = new Subject<int>();
        private readonly Subject<int> goldChanged = new Subject<int>();
        private readonly Subject<int> levelChanged = new Subject<int>();
        private readonly Subject<int> experienceInLevelChanged = new Subject<int>();

        public Player(ClientPlayerState playerState, IDateTimeProvider timeProvider, IPlayerConfigurationBase playerConfiguration)
            : base(playerState, timeProvider, playerConfiguration)
        {
        }

        public IObservable<string> NicknameChanged => nicknameChanged;

        public IObservable<int> CashChanged => cashChanged;

        public IObservable<int> GoldChanged => goldChanged;

        public IObservable<int> LevelChanged => levelChanged;

        public IObservable<int> ExperienceInLevelChanged => experienceInLevelChanged;

        public override string Nickname
        {
            set
            {
                base.Nickname = value;
                nicknameChanged.OnNext(value);
            }
        }

        public override int Cash
        {
            protected set
            {
                if (base.Cash != value)
                {
                    base.Cash = value;
                    cashChanged.OnNext(value);
                }
            }
        }

        public override int Gold
        {
            protected set
            {
                if (base.Gold != value)
                {
                    base.Gold = value;
                    goldChanged.OnNext(value);
                }
            }
        }

        public override int Level
        {
            protected set
            {
                if (base.Level != value)
                {
                    base.Level = value;
                    levelChanged.OnNext(value);
                }
            }
        }

        public override int ExperienceInLevel
        {
            protected set
            {
                if (base.ExperienceInLevel != value)
                {
                    base.ExperienceInLevel = value;
                    experienceInLevelChanged.OnNext(value);
                }
            }
        }
    }
}
