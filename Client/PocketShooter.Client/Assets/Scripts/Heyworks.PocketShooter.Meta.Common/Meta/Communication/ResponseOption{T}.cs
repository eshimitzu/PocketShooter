namespace Heyworks.PocketShooter.Meta.Communication
{
    public class ResponseOption<TOkData>
    {
        private ResponseOption()
        {
        }

        public ResponseOption(ResponseOk<TOkData> ok)
        {
            Ok = ok;
        }

        public ResponseOption(ResponseError error)
        {
            Error = error;
        }

        public ResponseOk<TOkData> Ok { get; private set; }

        public ResponseError Error { get; private set; }

        public bool IsOk => Ok != null;

        public bool IsError => !IsOk;
    }
}
