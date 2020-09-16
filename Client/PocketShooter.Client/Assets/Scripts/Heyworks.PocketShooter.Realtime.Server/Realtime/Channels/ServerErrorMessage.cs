using System;
using Heyworks.PocketShooter.Realtime.Channels;

namespace Heyworks.PocketShooter.Realtime.Channels
{

    //[Union(typeof(IServiceMessage),typeof(object))]
    public enum ServerErrorKind
    {
        //[Case(typeof((ClientException Client)))]
        Client,

        //[Case(typeof((ServerException Server)))]
        Server,
    }

    internal sealed partial class ServerErrorMessage : IServiceMessage
    {
        private class Never { }
        private static Never never = new Never();

        private readonly object value;

        public object Client => Case != ServerErrorKind.Client ? never : value;

        public object Server => Case != ServerErrorKind.Server ? never : value;

        public ServerErrorKind Case { get; }

        // is send to specific channel from collection
        public ServerErrorMessage(ClientException value)
        {
            this.value = value;
            this.Case = ServerErrorKind.Client;
        }

        // is send to channel available at hand
        public ServerErrorMessage(ServerException value)
        {
            this.value = value;
            this.Case = ServerErrorKind.Server;
        }

        internal void Default() => Throw.InvalidProgram();
    }
}