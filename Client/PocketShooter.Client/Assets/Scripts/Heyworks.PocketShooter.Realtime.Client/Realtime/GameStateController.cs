using System;

namespace Heyworks.PocketShooter.Realtime
{
    /// <summary>
    /// Represents class for controlling game states.
    /// </summary>
    public class GameStateController
    {
        private IGameState currentState;

        /// <summary>
        /// Occurs when state changed.
        /// </summary>
        public event Action<IGameState> StateChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameStateController"/> class
        /// with current state set to <see cref="GameDisconnectedState"/>.
        /// </summary>
        public GameStateController() => currentState = new GameDisconnectedState(DisconnectReason.GameEnd);

        /// <summary>
        /// Sets the current state.
        /// </summary>
        /// <param name="state">The state.</param>
        public void SetCurrentState(IGameState state) => OnStateChanged(currentState = state.NotNull());

        public void UpdateCurrentState() => currentState.Update();

        private void OnStateChanged(IGameState state) => StateChanged?.Invoke(state);
    }
}
