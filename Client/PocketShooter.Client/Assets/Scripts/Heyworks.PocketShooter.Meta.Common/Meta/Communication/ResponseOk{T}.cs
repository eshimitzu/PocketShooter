using System;

namespace Heyworks.PocketShooter.Meta.Communication
{
    /// <summary>
    /// Represents a server response Ok class.
    /// </summary>
    public class ResponseOk : Response
    {
        public static ResponseOk Create() => new ResponseOk();

        public static ResponseOk<TData> Create<TData>(TData data) => new ResponseOk<TData>(data);

        public static ResponseOption<TOkData> CreateOption<TOkData>(TOkData okData) =>
            new ResponseOption<TOkData>(Create(okData));

        public static ResponseOption<TOkData> CreateOption<TOkData>(ResponseOk<TOkData> ok) =>
            new ResponseOption<TOkData>(ok);

        public static ResponseOption CreateOption() => new ResponseOption(Create());
    }

    /// <summary>
    /// Represents a server response Ok with data class.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    public class ResponseOk<T> : ResponseOk
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseOk{T}"/> class.
        /// </summary>
        /// <param name="data">The response data.</param>
        public ResponseOk(T data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets a data object.
        /// </summary>
        public T Data
        {
            get;
            set;
        }
    }
}
