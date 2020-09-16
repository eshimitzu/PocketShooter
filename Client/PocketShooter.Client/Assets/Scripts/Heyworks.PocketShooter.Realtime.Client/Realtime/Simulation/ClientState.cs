using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Entities;

namespace Heyworks.PocketShooter.Realtime.Simulation
{
    public static class ClientState
    {
        public static ClientState<T> Create<T>(IRefList<T> playerStates, Tick serverTick, Tick clientTick)
            where T : struct
            =>
            new ClientState<T>(playerStates, serverTick, clientTick);
    }


    public struct ClientState<T> : IRefIndex<T> where T : struct
    {
        public ClientState(IRefList<T> playerStates, Tick serverTick, Tick clientTick)
        {
            if (serverTick > clientTick)
                Throw.InvalidProgram("Server tick should be always less or equal to client tick");
            this.playerStates = playerStates;
            Server = serverTick;
            Client = clientTick;
        }

        private IRefList<T> playerStates;

        /// <summary>
        ///  The tick of local player state that is used for making diff and raising diff events.
        /// </summary>
        public Tick Server { get; }

        /// <summary>
        /// The tick of the local player ref in the local player state buffer. Predicted self. 
        /// </summary>
        public Tick Client { get; }

        public LocalRef<T> AtServerTick => new LocalRef<T>(playerStates, Server);
        public LocalRef<T> AtClientTick => new LocalRef<T>(playerStates, Client);

        public ref T this[int tick]
        {
            get
            {
                if (tick > Client || tick < Server) 
                    Throw.IndexOutOfRange("This class works only within allowed range");
                    
                return ref playerStates[tick];
            }
        }
    }
}