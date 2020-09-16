using UnityEngine;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Serializable filter configuration by provider-category-level.
    /// </summary>
    [System.Serializable]
    public class LogFilterConfiguration
    {
        [SerializeField]
        private string providerName;
        [SerializeField]
        private string category;
        [SerializeField]
        private LogLevel level;

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        public string ProviderName => providerName;

        /// <summary>
        /// Gets the category.
        /// </summary>
        public string Category => category;

        /// <summary>
        /// Gets the level.
        /// </summary>
        public LogLevel Level => level;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFilterConfiguration"/> class for serialized data.
        /// </summary>
        public LogFilterConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFilterConfiguration"/> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="logLevel">The log level.</param>
        public LogFilterConfiguration(string providerName, string categoryName, LogLevel logLevel)
        {
            this.providerName = providerName;
            this.category = categoryName;
            this.level = logLevel;
        }
    }
}