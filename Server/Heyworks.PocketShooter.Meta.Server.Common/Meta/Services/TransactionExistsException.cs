using System;
using System.Runtime.Serialization;

namespace Heyworks.PocketShooter.Meta.Services
{
    /// <summary>
    /// The exception that is thrown when a payment or any other transaction is already exists in the system.
    /// </summary>
    [Serializable]
    public class TransactionExistsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionExistsException"/> class.
        /// </summary>
        public TransactionExistsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionExistsException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public TransactionExistsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionExistsException"/> class
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inner">The exception that is the cause of the current exception.
        /// If the innerException parameter is not a null reference (Nothing in Visual Basic), the current
        /// exception is raised in a catch block that handles the inner exception.</param>
        public TransactionExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionExistsException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected TransactionExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
