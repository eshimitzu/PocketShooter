namespace Heyworks.PocketShooter.Meta.Communication
{
    /// <summary>
    /// Data server response error class.
    /// </summary>
    public class ResponseError : Response
    {
        public ResponseError()
        {
        }

        public ResponseError(ApiErrorCode code)
            : this(code, string.Empty)
        {
        }

        public ResponseError(ApiErrorCode code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// Gets or sets the code of the error.
        /// </summary>
        public ApiErrorCode Code { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; }

        public static ResponseError Create(ApiErrorCode code, string message) => new ResponseError(code, message);

        public static ResponseError Create(ApiErrorCode code) => new ResponseError(code);

        public static ResponseOption<TOkData> CreateOption<TOkData>(ApiErrorCode code) =>
            new ResponseOption<TOkData>(Create(code));

        public static ResponseOption CreateOption(ApiErrorCode code) => new ResponseOption(Create(code));

        public static ResponseOption CreateOption(ApiErrorCode code, string message) =>
            new ResponseOption(Create(code, message));

        public static ResponseOption<TOkData> CreateOption<TOkData>(ApiErrorCode code, string message) =>
            new ResponseOption<TOkData>(Create(code, message));

        public static ResponseOption<TOkData> CreateOption<TOkData>(ResponseError error) =>
            new ResponseOption<TOkData>(error);
    }
}
