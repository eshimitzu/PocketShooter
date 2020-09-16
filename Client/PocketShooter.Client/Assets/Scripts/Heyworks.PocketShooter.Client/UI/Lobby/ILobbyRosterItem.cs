using System;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Purchasing.Products;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.UI.Localization;
using UniRx.Async;
using UnityEngine.Assertions;

// TODO: a.dezhurko Split into files.
// TODO: v.shimkovich comment all interfaces purpose
namespace Heyworks.PocketShooter.UI
{
    public enum RosterType
    {
        Trooper = 1,
        Weapon = 2,
        Helmet = 3,
        Armor = 4,
    }

    public interface IRosterItem
    {
        int Id { get; }

        string ItemName { get; }

        int Level { get; }

        int MaxLevel { get; }

        int Power { get; }

        int MaxPower { get; }

        Grade Grade { get; }
    }

    public interface ILobbyRosterItem : IRosterItem
    {
        RosterProduct Product { get; }
    }

    public interface ITrooperItem : ILobbyRosterItem
    {
        TrooperClass Class { get; }

        float Attack { get; }

        float Health { get; }

        float Armor { get; }

        float Movement { get; }

        SkillName Skill1 { get; }

        SkillName Skill2 { get; }

        SkillName Skill3 { get; }
    }

    public interface IWeaponItem : ILobbyRosterItem
    {
        WeaponName Name { get; }

        float Attack { get; }

        float Distance { get; }

        float Capacity { get; }

        float Reload { get; }
    }

    public interface IHelmetItem : ILobbyRosterItem
    {
        HelmetName Name { get; }

        float Health { get; }
    }

    public interface IArmorItem : ILobbyRosterItem
    {
        ArmorName Name { get; }

        float Armor { get; }
    }

    public interface IArmyItem : ILobbyRosterItem
    {
        Price InstantLevelUpPrice { get; }

        Price RegularLevelUpPrice { get; }

        Price InstantGradeUpPrice { get; }

        TimeSpan RegularLevelUpDuration { get; }

        bool IsMaxLevel { get; }

        bool IsFirstLevelOnGrade { get; }

        bool IsMaxGrade { get; }

        ItemStats NextLevelStats { get; }

        ItemStats NextGradeStats { get; }

        int NexGradeMaxLevel { get; }

        bool CanLevelUp { get; }

        bool CanGradeUp { get; }

        void LevelUp();

        void GradeUp();

        IContentIdentity ToContentIdentity();

        UniTask LevelUpInstantRequest(IGameHubClient gameHubClient);

        UniTask LevelUpRegularRequest(IGameHubClient gameHubClient);

        UniTask GradeUpInstantRequest(IGameHubClient gameHubClient);
    }

    public interface IProductItem : ILobbyRosterItem
    {
        int MaxGradeMaxLevel { get; }

        Grade MaxGrade { get; }

        ItemStats MaxStats { get; }
    }

    public interface IBattleTrooperItem : ITrooperItem
    {
        WeaponName WeaponName { get; }

        int WeaponPower { get; }

        HelmetName HelmetName { get; }

        int HelmetPower { get; }

        ArmorName ArmorName { get; }

        int ArmorPower { get; }

        int ItemsTotalPower { get; }
    }

    public interface IArmyTrooperItem : IBattleTrooperItem, IArmyItem
    {
    }

    public interface IArmyWeaponItem : IWeaponItem, IArmyItem
    {
    }

    public interface IArmyHelmetItem : IHelmetItem, IArmyItem
    {
    }

    public interface IArmyArmorItem : IArmorItem, IArmyItem
    {
    }

    public interface IProductTrooperItem : ITrooperItem, IProductItem
    {
    }

    public interface IProductWeaponItem : IWeaponItem, IProductItem
    {
    }

    public interface IProductHelmetItem : IHelmetItem, IProductItem
    {
    }

    public interface IProductArmorItem : IArmorItem, IProductItem
    {
    }

    // TODO: a.dezhurko Move common properties to the base classes.
    public abstract class RosterItem
    {
        private readonly ArmyItem data;

        protected RosterItem(ArmyItem data)
        {
            this.data = data;
        }

        public int Id => data.Id;
    }

    public abstract class ProductRosterItem
    {
        // TODO: a.dezhurko Bad smell
        public int Id => -1;
    }

    public class TrooperRosterItem : RosterItem, IArmyTrooperItem
    {
        private readonly Trooper data;

        public TrooperRosterItem(Trooper data)
            : base(data)
        {
            Assert.IsTrue(data.Skills.Count >= 3);
            this.data = data;
        }

        public Trooper Entity => data;

        public string ItemName => LocKeys.GetTooperNameKey(Class);

        public int Level => data.Level;

        public int Power => data.Stats.Power;

        public int ItemsTotalPower => HelmetPower + ArmorPower + WeaponPower;

        public int MaxPower => data.MaxPotentialPower;

        public Grade Grade => data.Grade;

        public RosterProduct Product => null;

        public TrooperClass Class => data.Class;

        public float Attack => data.Stats.Attack;

        public float Health => data.Stats.Health;

        public float Armor => data.Stats.Armor;

        public float Movement => (float)data.Stats.Movement;

        public SkillName Skill1 => data.Skills[0].Name;

        public SkillName Skill2 => data.Skills[1].Name;

        public SkillName Skill3 => data.Skills[2].Name;

        public WeaponName WeaponName => data.CurrentWeapon.Name;

        public int WeaponPower => data.CurrentWeapon.Stats.Power;

        public HelmetName HelmetName => data.CurrentHelmet.Name;

        public int HelmetPower => data.CurrentHelmet.Stats.Power;

        public ArmorName ArmorName => data.CurrentArmor.Name;

        public int ArmorPower => data.CurrentArmor.Stats.Power;

        public Price InstantLevelUpPrice => data.InstantLevelUpPrice;

        public Price RegularLevelUpPrice => data.RegularLevelUpPrice;

        public Price InstantGradeUpPrice => data.InstantGradeUpPrice;

        public TimeSpan RegularLevelUpDuration => data.RegularLevelUpDuration;

        public int MaxLevel => data.MaxLevel;

        public bool IsMaxLevel => data.IsMaxLevel;

        public bool IsFirstLevelOnGrade => data.IsFirstLevelOnGrade;

        public bool IsMaxGrade => data.Grade.IsMax();

        public ItemStats NextLevelStats => data.NextLevelStats;

        public ItemStats NextGradeStats => data.NextGradeStats;

        public int NexGradeMaxLevel => data.NextGradeMaxLevel;

        public bool CanLevelUp => data.CanLevelUp();

        public bool CanGradeUp => data.CanGradeUp();

        public void LevelUp() => data.LevelUp();

        public void GradeUp() => data.GradeUp();

        public IContentIdentity ToContentIdentity() => new TrooperIdentity(Class, Grade, Level);

        public async UniTask LevelUpInstantRequest(IGameHubClient gameHubClient)
        {
            await gameHubClient.LevelUpTrooperInstant(Class);
        }

        public async UniTask LevelUpRegularRequest(IGameHubClient gameHubClient)
        {
            await gameHubClient.LevelUpTrooperRegular(Class);
        }

        public async UniTask GradeUpInstantRequest(IGameHubClient gameHubClient)
        {
            await gameHubClient.GradeUpTrooperInstant(Class);
        }
    }

    public class TrooperInfoRosterItem : IBattleTrooperItem
    {
        private readonly TrooperInfo data;

        public TrooperInfoRosterItem(TrooperInfo data)
        {
            this.data = data;
        }

        public int Id => -1;

        public string ItemName => LocKeys.GetTooperNameKey(Class);

        public int Level => data.MetaInfo.Level;

        public int MaxLevel => data.MetaInfo.MaxLevel;

        public int Power => data.MetaInfo.Power;

        public int MaxPower => data.MetaInfo.MaxPotentialPower;

        public Grade Grade => data.MetaInfo.Grade;

        public RosterProduct Product => null;

        public TrooperClass Class => data.Class;

        public float Attack => data.MetaInfo.Attack;

        public float Health => data.MetaInfo.Health;

        public float Armor => data.MetaInfo.Armor;

        public float Movement => (float)data.MetaInfo.Movement;

        public SkillName Skill1 => data.Skill1.Name;

        public SkillName Skill2 => data.Skill2.Name;

        public SkillName Skill3 => data.Skill3.Name;

        public WeaponName WeaponName => data.MetaInfo.ItemsInfo.WeaponName;

        public int WeaponPower => data.MetaInfo.ItemsInfo.WeaponPower;

        public HelmetName HelmetName => data.MetaInfo.ItemsInfo.HelmetName;

        public int HelmetPower => data.MetaInfo.ItemsInfo.HelmetPower;

        public ArmorName ArmorName => data.MetaInfo.ItemsInfo.ArmorName;

        public int ArmorPower => data.MetaInfo.ItemsInfo.ArmorPower;

        public int ItemsTotalPower => HelmetPower + ArmorPower + WeaponPower;

    }

    public class TrooperProductRosterItem : ProductRosterItem, IProductTrooperItem
    {
        private readonly RosterTrooperProduct data;

        public TrooperProductRosterItem(RosterTrooperProduct data)
        {
            this.data = data;
        }

        public string ItemName => LocKeys.GetTooperNameKey(Class);

        public int Level => data.Level;

        public int MaxLevel => data.MaxLevel;

        public int Power => data.Stats.Power;

        public int MaxPower => data.MaxPotentialPower;

        public Grade Grade => data.Grade;

        RosterProduct ILobbyRosterItem.Product => data.RosterProduct;

        public RosterProduct Product => data.RosterProduct;

        public TrooperClass Class => data.Class;

        public float Attack => data.Stats.Attack;

        public float Health => data.Stats.Health;

        public float Armor => data.Stats.Armor;

        public float Movement => (float)data.Stats.Movement;

        public SkillName Skill1 => data.Skills[0].Name;

        public SkillName Skill2 => data.Skills[1].Name;

        public SkillName Skill3 => data.Skills[2].Name;

        public int MaxGradeMaxLevel => data.MaxGradeMaxLevel;

        public Grade MaxGrade => data.MaxGrade;

        public ItemStats MaxStats => data.MaxStats;
    }

    public class WeaponRosterItem : RosterItem, IArmyWeaponItem
    {
        private readonly Weapon data;

        public WeaponRosterItem(Weapon data)
            : base(data)
        {
            this.data = data;
        }

        public Weapon Entity => data;

        public string ItemName => LocKeys.GetWeaponNameKey(Name);

        public int Level => data.Level;

        public int Power => data.Stats.Power;

        public int MaxPower => data.MaxPotentialPower;

        public Grade Grade => data.Grade;

        public RosterProduct Product => null;

        public WeaponName Name => data.Name;

        public float Attack => data.Stats.Attack;

        public float Reload => (float)data.Stats.Reload;

        public float Capacity => data.Stats.Capacity;

        public float Distance => data.Stats.Distance;

        public Price InstantLevelUpPrice => data.InstantLevelUpPrice;

        public Price RegularLevelUpPrice => data.RegularLevelUpPrice;

        public Price InstantGradeUpPrice => data.InstantGradeUpPrice;

        public TimeSpan RegularLevelUpDuration => data.RegularLevelUpDuration;

        public int MaxLevel => data.MaxLevel;

        public bool IsMaxLevel => data.IsMaxLevel;

        public bool IsFirstLevelOnGrade => data.IsFirstLevelOnGrade;

        public bool IsMaxGrade => data.Grade.IsMax();

        public ItemStats NextLevelStats => data.NextLevelStats;

        public ItemStats NextGradeStats => data.NextGradeStats;

        public int NexGradeMaxLevel => data.NextGradeMaxLevel;

        public bool CanLevelUp => data.CanLevelUp();

        public bool CanGradeUp => data.CanGradeUp();

        public void LevelUp() => data.LevelUp();

        public void GradeUp() => data.GradeUp();

        public IContentIdentity ToContentIdentity() => new WeaponIdentity(Name, Grade, Level);

        public async UniTask LevelUpInstantRequest(IGameHubClient gameHubClient)
        {
            await gameHubClient.LevelUpWeaponInstant(Name);
        }

        public async UniTask LevelUpRegularRequest(IGameHubClient gameHubClient)
        {
            await gameHubClient.LevelUpWeaponRegular(Name);
        }

        public async UniTask GradeUpInstantRequest(IGameHubClient gameHubClient)
        {
            await gameHubClient.GradeUpWeaponInstant(Name);
        }
    }

    public class WeaponProductRosterItem : ProductRosterItem, IProductWeaponItem
    {
        private readonly RosterWeaponProduct data;

        public WeaponProductRosterItem(RosterWeaponProduct data)
        {
            this.data = data;
        }

        public string ItemName => LocKeys.GetWeaponNameKey(Name);

        public int Level => data.Level;

        public int MaxLevel => data.MaxLevel;

        public int Power => data.Stats.Power;

        public int MaxPower => data.MaxPotentialPower;

        public Grade Grade => data.Grade;

        RosterProduct ILobbyRosterItem.Product => data.RosterProduct;

        public RosterProduct Product => data.RosterProduct;

        public WeaponName Name => data.Name;

        public float Attack => data.Stats.Attack;

        public float Distance => data.Stats.Distance;

        public float Capacity => data.Stats.Capacity;

        public float Reload => (float)data.Stats.Reload;

        public int MaxGradeMaxLevel => data.MaxGradeMaxLevel;

        public Grade MaxGrade => data.MaxGrade;

        public ItemStats MaxStats => data.MaxStats;
    }

    public class HelmetRosterItem : RosterItem, IArmyHelmetItem
    {
        private readonly Helmet data;

        public HelmetRosterItem(Helmet data)
            : base(data)
        {
            this.data = data;
        }

        public Helmet Entity => data;

        public string ItemName => LocKeys.GetHelmetNameKey(Name);

        public int Level => data.Level;

        public int Power => data.Stats.Power;

        public int MaxPower => data.MaxPotentialPower;

        public Grade Grade => data.Grade;

        public RosterProduct Product => null;

        public HelmetName Name => data.Name;

        public float Health => data.Stats.Health;

        public Price InstantLevelUpPrice => data.InstantLevelUpPrice;

        public Price RegularLevelUpPrice => data.RegularLevelUpPrice;

        public Price InstantGradeUpPrice => data.InstantGradeUpPrice;

        public TimeSpan RegularLevelUpDuration => data.RegularLevelUpDuration;

        public int MaxLevel => data.MaxLevel;

        public bool IsFirstLevelOnGrade => data.IsFirstLevelOnGrade;

        public bool IsMaxLevel => data.IsMaxLevel;

        public bool IsMaxGrade => data.Grade.IsMax();

        public ItemStats NextLevelStats => data.NextLevelStats;

        public ItemStats NextGradeStats => data.NextGradeStats;

        public int NexGradeMaxLevel => data.NextGradeMaxLevel;

        public bool CanLevelUp => data.CanLevelUp();

        public bool CanGradeUp => data.CanGradeUp();

        public void LevelUp() => data.LevelUp();

        public void GradeUp() => data.GradeUp();

        public IContentIdentity ToContentIdentity() => new HelmetIdentity(Name, Grade, Level);

        public async UniTask LevelUpInstantRequest(IGameHubClient gameHubClient)
        {
            await gameHubClient.LevelUpHelmetInstant(Name);
        }

        public async UniTask LevelUpRegularRequest(IGameHubClient gameHubClient)
        {
            await gameHubClient.LevelUpHelmetRegular(Name);
        }

        public async UniTask GradeUpInstantRequest(IGameHubClient gameHubClient)
        {
            await gameHubClient.GradeUpHelmetInstant(Name);
        }
    }

    public class HelmetProductRosterItem : ProductRosterItem, IProductHelmetItem
    {
        private readonly RosterHelmetProduct data;

        public HelmetProductRosterItem(RosterHelmetProduct data)
        {
            this.data = data;
        }

        public string ItemName => LocKeys.GetHelmetNameKey(Name);

        public int Level => data.Level;

        public int MaxLevel => data.MaxLevel;

        public int Power => data.Stats.Power;

        public int MaxPower => data.MaxPotentialPower;

        public Grade Grade => data.Grade;

        RosterProduct ILobbyRosterItem.Product => data.RosterProduct;

        public RosterProduct Product => data.RosterProduct;

        public HelmetName Name => data.Name;

        public float Health => data.Stats.Health;

        public int MaxGradeMaxLevel => data.MaxGradeMaxLevel;

        public Grade MaxGrade => data.MaxGrade;

        public ItemStats MaxStats => data.MaxStats;
    }

    public class ArmorRosterItem : RosterItem, IArmyArmorItem
    {
        private readonly Armor data;

        public ArmorRosterItem(Armor data)
            : base(data)
        {
            this.data = data;
        }

        public Armor Entity => data;

        public string ItemName => LocKeys.GetArmorNameKey(Name);

        public int Level => data.Level;

        public int Power => data.Stats.Power;

        public int MaxPower => data.MaxPotentialPower;

        public Grade Grade => data.Grade;

        public RosterProduct Product => null;

        public ArmorName Name => data.Name;

        public float Armor => data.Stats.Armor;

        public Price InstantLevelUpPrice => data.InstantLevelUpPrice;

        public Price RegularLevelUpPrice => data.RegularLevelUpPrice;

        public Price InstantGradeUpPrice => data.InstantGradeUpPrice;

        public TimeSpan RegularLevelUpDuration => data.RegularLevelUpDuration;

        public int MaxLevel => data.MaxLevel;

        public bool IsMaxLevel => data.IsMaxLevel;

        public bool IsFirstLevelOnGrade => data.IsFirstLevelOnGrade;

        public bool IsMaxGrade => data.Grade.IsMax();

        public ItemStats NextLevelStats => data.NextLevelStats;

        public ItemStats NextGradeStats => data.NextGradeStats;

        public int NexGradeMaxLevel => data.NextGradeMaxLevel;

        public bool CanLevelUp => data.CanLevelUp();

        public bool CanGradeUp => data.CanGradeUp();

        public void LevelUp() => data.LevelUp();

        public void GradeUp() => data.GradeUp();

        public IContentIdentity ToContentIdentity() => new ArmorIdentity(Name, Grade, Level);

        public async UniTask LevelUpInstantRequest(IGameHubClient gameHubClient)
        {
            await gameHubClient.LevelUpArmorInstant(Name);
        }

        public async UniTask LevelUpRegularRequest(IGameHubClient gameHubClient)
        {
            await gameHubClient.LevelUpArmorRegular(Name);
        }

        public async UniTask GradeUpInstantRequest(IGameHubClient gameHubClient)
        {
            await gameHubClient.GradeUpArmorInstant(Name);
        }
    }

    public class ArmorProductRosterItem : ProductRosterItem, IProductArmorItem
    {
        private readonly RosterArmorProduct data;

        public ArmorProductRosterItem(RosterArmorProduct data)
        {
            this.data = data;
        }

        public string ItemName => LocKeys.GetArmorNameKey(Name);

        public int Level => data.Level;

        public int MaxLevel => data.MaxLevel;

        public int Power => data.Stats.Power;

        public int MaxPower => data.MaxPotentialPower;

        public Grade Grade => data.Grade;

        RosterProduct ILobbyRosterItem.Product => data.RosterProduct;

        public RosterProduct Product => data.RosterProduct;

        public ArmorName Name => data.Name;

        public float Armor => data.Stats.Armor;

        public int MaxGradeMaxLevel => data.MaxGradeMaxLevel;

        public Grade MaxGrade => data.MaxGrade;

        public ItemStats MaxStats => data.MaxStats;
    }
}