using System.Collections.Generic;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Config.
    /// </summary>
    public interface ILoggerConfiguration
    {
        /// <summary>
        /// Gets the filters.
        /// </summary>
        List<LogFilterConfiguration> Filters { get; }

        /// <summary>
        /// Reload configuration.
        /// </summary>
        void Reload();

        /// <summary>
        /// Gets or sets the on changed.
        /// </summary>
        /// <value>The on changed.</value>
        System.Action<ILoggerConfiguration> OnChanged { get; set; }
    }
}
