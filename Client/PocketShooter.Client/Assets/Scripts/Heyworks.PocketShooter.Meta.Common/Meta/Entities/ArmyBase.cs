using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Runnables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public abstract class ArmyBase : IHasRunnables
    {
        private ArmyState armyState;

        protected ArmyBase(ArmyState armyState, IDateTimeProvider dateTimeProvider)
        {
            this.armyState = armyState;
            this.DateTimeProvider = dateTimeProvider;
        }

        public OffensiveName SelectedOffensive => OffensiveName.Grenade;

        public SupportName SelectedSupport => SupportName.MedKit;

        public virtual void UpdateState(ArmyState armyState)
        {
            this.armyState = armyState;
        }

        public void AddTrooper(TrooperIdentity trooper)
        {
            var existingTrooper = armyState.Troopers.FirstOrDefault(_ => _.Class == trooper.Class);
            if (existingTrooper == null)
            {
                armyState.Troopers.Add(new TrooperState
                {
                    Id = GetNextItemId(),
                    Class = trooper.Class,
                    Grade = trooper.Grade,
                    Level = trooper.Level,
                    CurrentWeapon = armyState.Weapons.FirstOrDefault()?.Name,
                    CurrentHelmet = armyState.Helmets.FirstOrDefault()?.Name,
                    CurrentArmor = armyState.Armors.FirstOrDefault()?.Name,
                });
            }
            else
            {
                existingTrooper.Level = Math.Max(existingTrooper.Level, trooper.Level);
                existingTrooper.Grade = (Grade)Math.Max((int)existingTrooper.Grade, (int)trooper.Grade);
            }
        }

        public void AddWeapon(WeaponIdentity weapon)
        {
            var existingWeapon = armyState.Weapons.FirstOrDefault(_ => _.Name == weapon.Name);
            if (existingWeapon == null)
            {
                armyState.Weapons.Add(new WeaponState
                {
                    Id = GetNextItemId(),
                    Name = weapon.Name,
                    Grade = weapon.Grade,
                    Level = weapon.Level,
                });
            }
            else
            {
                existingWeapon.Level = Math.Max(existingWeapon.Level, weapon.Level);
                existingWeapon.Grade = (Grade)Math.Max((int)existingWeapon.Grade, (int)weapon.Grade);
            }
        }

        public void AddHelmet(HelmetIdentity equipment)
        {
            var existing = armyState.Helmets.FirstOrDefault(_ => _.Name == equipment.Name);
            if (existing == null)
            {
                armyState.Helmets.Add(new HelmetState
                {
                    Id = GetNextItemId(),
                    Name = equipment.Name,
                    Grade = equipment.Grade,
                    Level = equipment.Level,
                });
            }
            else
            {
                existing.Level = Math.Max(existing.Level, equipment.Level);
                existing.Grade = (Grade)Math.Max((int)existing.Grade, (int)equipment.Grade);
            }
        }

        public void AddArmor(ArmorIdentity armor)
        {
            var existing = armyState.Armors.FirstOrDefault(_ => _.Name == armor.Name);
            if (existing == null)
            {
                armyState.Armors.Add(new ArmorState
                {
                    Id = GetNextItemId(),
                    Name = armor.Name,
                    Grade = armor.Grade,
                    Level = armor.Level,
                });
            }
            else
            {
                existing.Level = Math.Max(existing.Level, armor.Level);
                existing.Grade = (Grade)Math.Max((int)existing.Grade, (int)armor.Grade);
            }
        }

        public void AddOffensive(OffensiveIdentity offensive)
        {
            var existingOffensive = armyState.Offensives.FirstOrDefault(_ => _.Name == offensive.Name);
            if (existingOffensive == null)
            {
                armyState.Offensives.Add(new OffensiveState
                {
                    Name = offensive.Name,
                    Count = offensive.Count,
                });
            }
            else
            {
                existingOffensive.Count += offensive.Count;
            }
        }

        public void RemoveSelectedOffensive(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "The value must be non-negative.");

            OffensiveState selectedOffensive = armyState.Offensives.FirstOrDefault(_ => _.Name == SelectedOffensive);
            if (selectedOffensive != null)
            {
                selectedOffensive.Count = Math.Max(selectedOffensive.Count - count, 0);
            }
        }

        public void AddSupport(SupportIdentity support)
        {
            var existingOffensive = armyState.Supports.FirstOrDefault(_ => _.Name == support.Name);
            if (existingOffensive == null)
            {
                armyState.Supports.Add(new SupportState
                {
                    Name = support.Name,
                    Count = support.Count,
                });
            }
            else
            {
                existingOffensive.Count += support.Count;
            }
        }

        public void RemoveSelectedSupport(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "The value must be non-negative.");

            SupportState selectedSupport = armyState.Supports.FirstOrDefault(_ => _.Name == SelectedSupport);
            if (selectedSupport != null)
            {
                selectedSupport.Count = Math.Max(selectedSupport.Count - count, 0);
            }
        }

        public int GetSelectedOffensivesCount() =>
            armyState.Offensives.FirstOrDefault(_ => _.Name == SelectedOffensive)?.Count ?? 0;

        public int GetSelectedSupportsCount() =>
            armyState.Supports.FirstOrDefault(_ => _.Name == SelectedSupport)?.Count ?? 0;

        public bool CanEquipWeapon(TrooperClass trooperClass, WeaponName weaponName)
        {
            var existingTrooper = armyState.Troopers.FirstOrDefault(_ => _.Class == trooperClass);
            var existingWeapon = armyState.Weapons.FirstOrDefault(_ => _.Name == weaponName);

            return
                existingTrooper != null &&
                existingWeapon != null &&
                existingTrooper.CurrentWeapon != weaponName;
        }

        public void EquipWeapon(TrooperClass trooperClass, WeaponName weaponName)
        {
            if (!CanEquipWeapon(trooperClass, weaponName))
            {
                throw new InvalidOperationException("Provided trooper can't equip this weapon.");
            }

            var trooper = armyState.Troopers.First(_ => _.Class == trooperClass);
            trooper.CurrentWeapon = weaponName;
        }

        public bool CanEquipHelmet(TrooperClass trooperClass, HelmetName helmetName)
        {
            var existingTrooper = armyState.Troopers.FirstOrDefault(_ => _.Class == trooperClass);
            var existingHelmet = armyState.Helmets.FirstOrDefault(_ => _.Name == helmetName);

            return
                existingTrooper != null &&
                existingHelmet != null &&
                existingTrooper.CurrentHelmet != helmetName;
        }

        public void EquipHelmet(TrooperClass trooperClass, HelmetName helmetName)
        {
            if (!CanEquipHelmet(trooperClass, helmetName))
            {
                throw new InvalidOperationException("Provided trooper can't equip this helmet.");
            }

            var trooper = armyState.Troopers.First(_ => _.Class == trooperClass);
            trooper.CurrentHelmet = helmetName;
        }

        public bool CanEquipArmor(TrooperClass trooperClass, ArmorName armorName)
        {
            var existingTrooper = armyState.Troopers.FirstOrDefault(_ => _.Class == trooperClass);
            var existingArmor = armyState.Armors.FirstOrDefault(_ => _.Name == armorName);

            return
                existingTrooper != null &&
                existingArmor != null &&
                existingTrooper.CurrentArmor != armorName;
        }

        public void EquipArmor(TrooperClass trooperClass, ArmorName armorName)
        {
            if (!CanEquipArmor(trooperClass, armorName))
            {
                throw new InvalidOperationException("Provided trooper can't equip this armor.");
            }

            var trooper = armyState.Troopers.First(_ => _.Class == trooperClass);
            trooper.CurrentArmor = armorName;
        }

        public bool CanLevelUpItem(int id)
        {
            var item = FindItem(id);
            return item != null && item.CanLevelUp();
        }

        public void LevelUpItem(int id)
        {
            if (!CanLevelUpItem(id))
            {
                throw new InvalidOperationException($"Can't level up item with id {id} now.");
            }

            FindItem(id).LevelUp();
        }

        public bool CanGradeUpItem(int id)
        {
            var item = FindItem(id);
            return item != null && item.CanGradeUp();
        }

        public void GradeUpItem(int id)
        {
            if (!CanGradeUpItem(id))
            {
                throw new InvalidOperationException($"Can't grade up item with id {id} now.");
            }

            FindItem(id).GradeUp();
        }

        public bool CanStartItemProgress(int itemId)
        {
            return
                !HasItemProgress
                && CanLevelUpItem(itemId); //TODO: YGR. Add CanGradeUpItem(itemId) check when grade up will have a progress.
        }

        public void StartItemProgress(int itemId)
        {
            if (!CanStartItemProgress(itemId))
            {
                throw new InvalidOperationException($"Can't start progress for item with id {itemId}");
            }

            var item = FindItem(itemId);

            armyState.ItemProgress = new ArmyItemProgressState
            {
                ItemId = item.Id,
                Timer = new SimpleTimerState
                {
                    //TODO: YGR. Add RegularGradeUpDuration when grade up will have a progress.
                    Duration = item.RegularLevelUpDuration,
                    StartTime = DateTimeProvider.UtcNow,
                }
            };

            OnItemProgressStarted();
        }

        public bool CanCompleteItemProgress()
        {
            if (!HasItemProgress)
            {
                return false;
            }

            var itemId = armyState.ItemProgress.ItemId;

            return CanLevelUpItem(itemId) || CanGradeUpItem(itemId);
        }

        public void CompleteItemProgress()
        {
            if (!CanCompleteItemProgress())
            {
                throw new InvalidOperationException("Can't complete item progress now.");
            }

            var itemId = armyState.ItemProgress.ItemId;
            if (CanLevelUpItem(itemId))
            {
                LevelUpItem(itemId);
            }
            else
            {
                GradeUpItem(itemId);
            }

            armyState.ItemProgress = null;
        }

        public bool CanLevelUpTrooper(TrooperClass trooperClass)
        {
            var trooper = FindTrooper(trooperClass);

            return trooper != null && trooper.CanLevelUp();
        }

        public void LevelUpTrooper(TrooperClass trooperClass)
        {
            if (!CanLevelUpTrooper(trooperClass))
            {
                throw new InvalidOperationException("Can't level up trooper now.");
            }

            FindTrooper(trooperClass).LevelUp();
        }

        public bool CanGradeUpTrooper(TrooperClass trooperClass)
        {
            var trooper = FindTrooper(trooperClass);

            return trooper != null && trooper.CanGradeUp();
        }

        public void GradeUpTrooper(TrooperClass trooperClass)
        {
            if (!CanGradeUpTrooper(trooperClass))
            {
                throw new InvalidOperationException("Can't grade up trooper now.");
            }

            FindTrooper(trooperClass).GradeUp();
        }

        public bool CanLevelUpWeapon(WeaponName weaponName)
        {
            var weapon = FindWeapon(weaponName);

            return weapon != null && weapon.CanLevelUp();
        }

        public void LevelUpWeapon(WeaponName weaponName)
        {
            if (!CanLevelUpWeapon(weaponName))
            {
                throw new InvalidOperationException("Can't level up weapon now.");
            }

            FindWeapon(weaponName).LevelUp();
        }

        public bool CanGradeUpWeapon(WeaponName weaponName)
        {
            var weapon = FindWeapon(weaponName);

            return weapon != null && weapon.CanGradeUp();
        }

        public void GradeUpWeapon(WeaponName weaponName)
        {
            if (!CanGradeUpWeapon(weaponName))
            {
                throw new InvalidOperationException("Can't grade up weapon now.");
            }

            FindWeapon(weaponName).GradeUp();
        }

        public bool CanLevelUpHelmet(HelmetName helmetName)
        {
            var helmet = FindHelmet(helmetName);

            return helmet != null && helmet.CanLevelUp();
        }

        public void LevelUpHelmet(HelmetName helmetName)
        {
            if (!CanLevelUpHelmet(helmetName))
            {
                throw new InvalidOperationException("Can't level up helmet now.");
            }

            FindHelmet(helmetName).LevelUp();
        }

        public bool CanGradeUpHelmet(HelmetName helmetName)
        {
            var helmet = FindHelmet(helmetName);

            return helmet != null && helmet.CanGradeUp();
        }

        public void GradeUpHelmet(HelmetName helmetName)
        {
            if (!CanGradeUpHelmet(helmetName))
            {
                throw new InvalidOperationException("Can't grade up helmet now.");
            }

            FindHelmet(helmetName).GradeUp();
        }

        public bool CanLevelUpArmor(ArmorName armorName)
        {
            var armor = FindArmor(armorName);

            return armor != null && armor.CanLevelUp();
        }

        public void LevelUpArmor(ArmorName armorName)
        {
            if (!CanLevelUpArmor(armorName))
            {
                throw new InvalidOperationException("Can't level up armor now.");
            }

            FindArmor(armorName).LevelUp();
        }

        public bool CanGradeUpArmor(ArmorName armorName)
        {
            var armor = FindArmor(armorName);

            return armor != null && armor.CanGradeUp();
        }

        public void GradeUpArmor(ArmorName armorName)
        {
            if (!CanGradeUpArmor(armorName))
            {
                throw new InvalidOperationException("Can't grade up armor now.");
            }

            FindArmor(armorName).GradeUp();
        }

        public bool HasContent(IContentIdentity content)
        {
            switch (content)
            {
                case TrooperIdentity ti:
                    return FindTrooper(ti.Class) != null;
                case WeaponIdentity wi:
                    return FindWeapon(wi.Name) != null;
                case HelmetIdentity hi:
                    return FindHelmet(hi.Name) != null;
                case ArmorIdentity ai:
                    return FindArmor(ai.Name) != null;
                default:
                    return false;
            }
        }

        public bool HasAnyContent(IEnumerable<IContentIdentity> content) =>
            content.Any(_ => HasContent(_));

        public bool HasItemProgress => armyState.ItemProgress != null;

        public abstract ArmyItem FindItem(int id);

        protected IDateTimeProvider DateTimeProvider { get; }

        protected abstract TrooperBase FindTrooper(TrooperClass trooperClass);

        protected abstract WeaponBase FindWeapon(WeaponName weaponName);

        protected abstract HelmetBase FindHelmet(HelmetName helmetName);

        protected abstract ArmorBase FindArmor(ArmorName armorName);

        protected abstract IEnumerable<IRunnable> GetRunnables();

        protected virtual void OnItemProgressStarted()
        {
        }

        IEnumerable<IRunnable> IHasRunnables.GetRunnables() => GetRunnables();

        private int GetNextItemId() => armyState.NextItemId++;
    }
}
