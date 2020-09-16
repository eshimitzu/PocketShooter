using System.Diagnostics;

namespace Microsoft.Extensions.Logging
{
    // TODO: move out domain and application specific logs out of this tech library

    /// <inheritdoc/>
    public class NetLog : MLog<NetLog>, ICategoryLog
    {
        public static EventId Connected = new EventId(1, nameof(Connected));
        public static EventId Disconnected = new EventId(2, nameof(Disconnected));

        /// <summary>
        /// Debug only. Traces the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        [Conditional("DEBUG")]
        public static void Warning(string message, params object[] args) => Log.LogWarning(message, args);
    }

    /// <inheritdoc/>
    public sealed class SimulationLog : MLog<SimulationLog>, ICategoryLog
    {
    }

    /// <inheritdoc/>
    public sealed class AnalyticsLog : MLog<AnalyticsLog>, ICategoryLog
    {
        public static EventId PlayerRegistered = new EventId(1, nameof(PlayerRegistered));
    }

    /// <inheritdoc/>
    public sealed class AudioLog : MLog<AudioLog>, ICategoryLog
    {
    }

    /// <inheritdoc/>
    public sealed class InputLog : MLog<InputLog>, ICategoryLog
    {
    }

    /// <inheritdoc/>
    public sealed class GameLog : MLog<GameLog>, ICategoryLog
    {
    }

    /// <inheritdoc/>
    public sealed class GraphicsLog : MLog<GraphicsLog>, ICategoryLog
    {
    }

    /// <inheritdoc/>
    public sealed class SerializationLog : MLog<SerializationLog>, ICategoryLog
    {
    }

    /// <inheritdoc/>
    public sealed class ReconciliationLog : MLog<ReconciliationLog>, ICategoryLog
    {
    }

    /// <inheritdoc/>
    public sealed class PurchaseLog : MLog<PurchaseLog>, ICategoryLog
    {
    }

    public sealed class AuthLog : MLog<AuthLog>, ICategoryLog
    {
    }

    /// <inheritdoc/>
    public sealed class TestsLog : MLog<TestsLog>, ICategoryLog
    {
    }
}