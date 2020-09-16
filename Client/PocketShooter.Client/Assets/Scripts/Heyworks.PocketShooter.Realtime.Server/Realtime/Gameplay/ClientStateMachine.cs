using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Realtime.Channels;

namespace Heyworks.PocketShooter.Realtime
{
    public sealed class ClientStateMachine
    {
        private readonly IDictionary<ClientStateTransition, IClientStateFactory> transitions;

        public ClientStateMachine(IClientState initState)
        {
            CurrentState = initState ?? throw new ArgumentNullException(nameof(initState));

            transitions = new Dictionary<ClientStateTransition, IClientStateFactory>();
        }

        public IClientState CurrentState { get; private set; }

        public void AddTransition(string fromStateName, Type onMessageType, IClientStateFactory useStateFactory)
        {
            transitions.Add(new ClientStateTransition(fromStateName, onMessageType), useStateFactory);
        }

        public void OnMessage(IMessage message)
        {
            var transition = new ClientStateTransition(CurrentState.Name, message.GetType());

            if (transitions.TryGetValue(transition, out var nextStateFactory))
            {
                CurrentState = nextStateFactory.CreateState(message);
            }
        }

        private class ClientStateTransition : IEquatable<ClientStateTransition>
        {
            private readonly string currentStateName;
            private readonly object messageType;

            public ClientStateTransition(string currentStateName, Type messageType)
            {
                this.currentStateName = currentStateName;
                this.messageType = messageType;
            }

            /// <summary>
            /// Compares this <see cref="ClientStateTransition"/> object to another object.
            /// </summary>
            /// <param name="obj">The other object.</param>
            /// <returns>True if the other object is a <see cref="ClientStateTransition"/> and equal to this one.</returns>
            public override bool Equals(object obj) => Equals(this, obj as ClientStateTransition);

            /// <summary>
            /// Serves as a hash function for a particular type.
            /// </summary>
            /// <returns>
            /// A hash code for the current <see cref="ClientStateTransition"/>.
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = 17;

                    hashCode = (hashCode * 31) + currentStateName.GetHashCode();
                    hashCode = (hashCode * 31) + messageType.GetHashCode();

                    return hashCode;
                }
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
            bool IEquatable<ClientStateTransition>.Equals(ClientStateTransition other) => Equals(this, other);

            private static bool Equals(ClientStateTransition st1, ClientStateTransition st2)
            {
                if (ReferenceEquals(st1, st2))
                {
                    return true;
                }

                if (ReferenceEquals(st1, null))
                {
                    return ReferenceEquals(st2, null);
                }

                if (ReferenceEquals(st2, null))
                {
                    return false;
                }

                return
                    st1.currentStateName == st2.currentStateName &&
                    st1.messageType == st2.messageType;
            }
        }
    }
}
