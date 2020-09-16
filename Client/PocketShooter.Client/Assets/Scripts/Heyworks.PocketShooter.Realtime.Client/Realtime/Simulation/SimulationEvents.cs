using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Microsoft.Extensions.Logging;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    public class SimulationEvents
    {
        private readonly Queue<(PlayerEventsBase, IServerEvent)> serverEvents =
            new Queue<(PlayerEventsBase, IServerEvent)>(32);

        private readonly Queue<(PlayerEventsBase, IClientEvent)> clientEvents =
            new Queue<(PlayerEventsBase, IClientEvent)>(32);

        public void ProcessClientEvents()
        {
            while (clientEvents.Count > 0)
            {
                var (player, e) = clientEvents.Dequeue();
                switch (e)
                {
                    case WeaponStateChangedEvent wce:
                        player.WeaponStateChange.OnNext(wce);
                        break;
                    case AmmoChangeEvent ace:
                        ((ClientPlayerEvents)player).ChangeAmmo.OnNext(ace.Ammo);
                        break;
                    case MedKitUsedEvent mue:
                        player.MedKitUse.OnNext(mue.Healed);
                        break;
                    case HealChangeEvent hce:
                        player.HealChange.OnNext(hce.Healed);
                        break;
                    case WarmingUpProgressChangedEvent wpce:
                        player.WarmingUpProgressChange.OnNext(wpce.Progress);
                        break;
                    case SkillStateChangedEvent ssc:
                        player.SkillStateChange.OnNext(ssc);
                        break;
                    case SkillAimChangeEvent sac:
                        player.SkillAimChange.OnNext(sac);
                        break;
                    case SkillCastChangedEvent scc:
                        player.SkillCastChange.OnNext(scc);
                        break;
                    case StunChangeEvent sce:
                        player.StunChange.OnNext(sce.Stunned);
                        break;
                    case RootChangeEvent rce:
                        player.RootChange.OnNext(rce.Rooted);
                        break;
                    case InvisibilityChangeEvent ice:
                        player.InvisibleChange.OnNext(ice.IsInvisible);
                        break;
                    case LuckyChangeEvent lce:
                        player.LuckyChange.OnNext(lce.Lucky);
                        break;
                    case DashChangeEvent dce:
                        player.DashChange.OnNext(dce.IsDashing);
                        break;
                    default:
                        throw new Exception($"Event {e.GetType()} is not supported");
                }
            }
        }

        public void ProcessServerEvents()
        {
            while (serverEvents.Count > 0)
            {
                var (player, e) = serverEvents.Dequeue();
                SimulationLog.Trace("Server raised {ServerEvent}", e.GetType());
                switch (e)
                {
                    case AttackServerEvent ase:
                        if (player is RemotePlayerEvents remote)
                        {
                            remote.Attacks.OnNext(ase);
                        }

                        break;
                    case DamagedServerEvent ase:
                        player.Damage.OnNext(ase);
                        break;
                    case KilledServerEvent kse:
                        player.Kill.OnNext(kse);
                        break;
                    case MedKitUsedEvent mue:
                        player.MedKitUse.OnNext(mue.Healed);
                        break;
                    case HealChangeEvent hce:
                        player.HealChange.OnNext(hce.Healed);
                        break;
                    case HealingServerEvent hse:
                        player.Heal.OnNext(hse);
                        break;
                    case AmmoChangeEvent ace:
                        ((ClientPlayerEvents)player).ChangeAmmo.OnNext(ace.Ammo);
                        break;
                    case WeaponStateChangedEvent wse:
                        player.WeaponStateChange.OnNext(wse);
                        break;
                    case ExpendablesEvent ee:
                        player.ExpendablesChange.OnNext(player.Id);
                        break;
                    case WarmingUpProgressChangedEvent wpce:
                        player.WarmingUpProgressChange.OnNext(wpce.Progress);
                        break;
                    case StunChangeEvent sce:
                        player.StunChange.OnNext(sce.Stunned);
                        break;
                    case RootChangeEvent rce:
                        player.RootChange.OnNext(rce.Rooted);
                        break;
                    case InvisibilityChangeEvent ice:
                        player.InvisibleChange.OnNext(ice.IsInvisible);
                        break;
                    case ImmortalityChangeEvent imce:
                        player.ImmortalityChange.OnNext(imce);
                        break;
                    case SkillStateChangedEvent ssc:
                        player.SkillStateChange.OnNext(ssc);
                        break;
                    case SkillAimChangeEvent sac:
                        player.SkillAimChange.OnNext(sac);
                        break;
                    case SkillCastChangedEvent scc:
                        player.SkillCastChange.OnNext(scc);
                        break;
                    case RageChangedEvent rgce:
                        player.RageChange.OnNext(rgce.Progress);
                        break;
                    case LuckyChangeEvent lce:
                        player.LuckyChange.OnNext(lce.Lucky);
                        break;
                    case DashChangeEvent dce:
                        player.DashChange.OnNext(dce.IsDashing);
                        break;
                    default:
                        throw new NotImplementedException($"Event {e.GetType()} is not supported");
                }
            }
        }

        internal void EnqueueServer<T>((PlayerEventsBase playerEvents, T e) value)
            where T : struct, IServerEvent
        {
            serverEvents.Enqueue(value);
        }

        internal void EnqueueClient<T>((PlayerEventsBase playerEvents, T e) value)
            where T : struct, IClientEvent
        {
            clientEvents.Enqueue(value);
        }
    }
}