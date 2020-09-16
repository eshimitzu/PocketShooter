using System;
using System.Collections.Generic;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    /// <summary>
    /// Holds circular buffer of states with proper last state handling.
    /// </summary>
    /// <typeparam name="T">State type.</typeparam>
    public interface IStateProvider<T> : IRefList<T>
    {
        int LastTick { get; }

        void Start(int startWorldTick, in T initState);

        // gets state by LastTick
        ref T GetLastState { get; }
    }
}