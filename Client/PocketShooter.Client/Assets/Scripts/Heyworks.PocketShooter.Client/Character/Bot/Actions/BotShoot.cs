using BehaviorDesigner.Runtime.Tasks;
using Collections.Pooled;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    public class BotShoot : BotAction
    {
        public override TaskStatus OnUpdate()
        {
            var canAttack = Bot.Model.CanAttack();
            var weapon = Bot.Model.CurrentWeapon;

            if (Observer.IsEnemyInCrosshair)
            {
                if (Bot.Model.CurrentWeapon is IWarmingUpWeapon)
                {
                    var commandData = new WarmingUpCommandData(Bot.Model.Id);
                    Bot.AddCommand(commandData);
                }

                if (canAttack)
                {
                    var targetCharacter = Observer.EnemyInCrosshair;
                    var shots = new PooledList<ShotInfo>();
                    shots.Add(new ShotInfo(targetCharacter.Id, weapon.Name, UnityEngine.Random.Range(0, 100) > 80));
                    for (int i = 1; i < weapon.Info.Fraction; i++)
                    {
                        var shot = new ShotInfo(
                            UnityEngine.Random.Range(0, 100) > 30 ? targetCharacter.Id : (byte)0,
                            weapon.Name,
                            UnityEngine.Random.Range(0, 100) > 80);
                        shots.Add(shot);
                    }

                    var commandData = new AttackCommandData(Bot.Model.Id, shots);
                    Bot.AddCommand(commandData);
                }

                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }
    }
}