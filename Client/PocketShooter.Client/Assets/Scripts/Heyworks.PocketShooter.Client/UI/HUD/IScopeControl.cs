using System;

namespace Heyworks.PocketShooter.UI.HUD
{
    public interface IScopeControl
    {
        event Action Scope;

        CrosshairView CrosshairView { get; }
    }
}