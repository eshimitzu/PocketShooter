namespace Heyworks.PocketShooter.Meta.Communication
{
    public class ResponseOption
    {
        private ResponseOption()
        {
        }

        public ResponseOption(ResponseOk ok)
        {
            Ok = ok;
        }

        public ResponseOption(ResponseError error)
        {
            Error = error;
        }

        public ResponseOk Ok { get; private set; }

        public ResponseError Error { get; private set; }

        public bool IsOk => Ok != null;

        public bool IsError => Error != null;
    }
}
