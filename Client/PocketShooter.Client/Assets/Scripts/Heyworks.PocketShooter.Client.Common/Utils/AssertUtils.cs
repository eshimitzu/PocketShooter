using System;
using System.IO;

namespace Heyworks.PocketShooter.Utils
{
    /// <summary>
    /// Assertion utility methods that simplify things such as argument checks.
    /// </summary>
    public class AssertUtils
    {
        /// <summary>
        /// Checks the value of the specified object and throws an exception if the object id null.
        /// </summary>
        /// <param name="argument"> The object to check. </param>
        /// <param name="argumentName"> The argument name. </param>
        /// <exception cref="ArgumentNullException"> Thrown if the assert is not correct. </exception>
        public static void NotNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("Argument is null: " + argumentName);
            }
        }

        /// <summary>
        /// Verifies the string argument against the not empty constraint.
        /// </summary>
        /// <param name="argument"> The argument. </param>
        /// <param name="argumentName"> Name of the argument. </param>
        /// <remarks>
        /// The verification includes not null constraint checking as well.
        /// </remarks>
        public static void NotEmpty(string argument, string argumentName)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException(argumentName + " can't be empty.", argumentName);
            }
        }

        /// <summary>
        /// Determines the correctness of the condition specified.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="argumentName">Name of the argument.</param>
        public static void IsValidArg(bool condition, string argumentName)
        {
            if (!condition)
            {
                var errorMsg = string.Format("Argument is not valid: {0}. ", argumentName);
                throw new ArgumentException(errorMsg, argumentName);
            }
        }

        /// <summary>
        /// Determines the correctness of the condition specified.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <param name="message">Error message </param>
        public static void IsValidArg(bool condition, string argumentName, string message)
        {
            if (!condition)
            {
                var errorMsg = string.Format("Argument is not valid: {0}. {1}", argumentName, message);
                throw new ArgumentException(errorMsg, argumentName);
            }
        }

        /// <summary>
        /// Checks if the resource object was loaded.
        /// </summary>
        /// <param name="resource">Resource object.</param>
        /// <param name="resourcePath">Path to resource object.</param>
        /// <param name="throwException">If set to true exception will be thrown if resource is not loaded.</param>
        /// <remarks>
        /// Use resourcePath without Resources folder.
        /// </remarks>
        public static void ResourceLoaded(object resource, string resourcePath, bool throwException = true)
        {
            // Second condition works with GameObjects that are not initialized.
            if (resource == null || resource.ToString() == "null")
            {
                UnityEngine.Debug.LogError("Resource was not loaded: " + resourcePath);

                if (throwException)
                {
                    throw new FileNotFoundException(string.Format("Resource file not found. Path: {0}", resourcePath));
                }
            }
        }
    }
}
