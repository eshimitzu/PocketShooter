using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.UI;
using Heyworks.PocketShooter.UI.Core;
using Heyworks.PocketShooter.UI.Localization;
using Heyworks.PocketShooter.UI.Popups;

namespace Heyworks.PocketShooter.Authorization
{
    public class UIConnectionListener : IConnectionListener
    {
        private readonly HashSet<Type> disconnectedClients = new HashSet<Type>();

        public void Disconnected(Type client)
        {
            lock (this)
            {
                if (disconnectedClients.Count == 0)
                {
                    UnitySynchronizationContext.Current.Post(_ => ShowDisconnectPopup(), null);
                }

                disconnectedClients.Add(client);
            }
        }

        public void Connected(Type client)
        {
            lock (this)
            {
                if (disconnectedClients.Remove(client) && disconnectedClients.Count == 0)
                {
                    UnitySynchronizationContext.Current.Post(_ => ProgressHUD.Instance.Hide(), null);
                }
            }
        }

        private void ShowDisconnectPopup()
        {
            ScreensController.Instance.ShowMessageBox(
                LocKeys.MetaDisconnectTitle.Localized(),
                LocKeys.MetaDisconnectDescription.Localized(),
                new PopupOptionInfo(
                    LocKeys.MetaDisconnectButton.Localized(),
                    action: () =>
                    {
                        lock (this)
                        {
                            if (disconnectedClients.Count > 0)
                            {
                                ProgressHUD.Instance.Show(0f);
                            }
                        }
                    }));
        }
    }
}