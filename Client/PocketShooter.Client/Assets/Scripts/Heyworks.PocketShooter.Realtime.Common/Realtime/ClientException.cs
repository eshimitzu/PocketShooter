using System;
using System.Runtime.Serialization;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Realtime
{
    // raised on realtime server and sent to client
    // does not prevents server from continue executing
    [Serializable]
    public class ClientException : Exception
    {
        private ClientException()
        {
        }

        public ClientException(ClientErrorCode errorCode, EntityId trooperId)
        {
            ErrorCode = errorCode;
            TrooperId = trooperId;
        }

        public ClientException(ClientErrorCode errorCode, EntityId trooperId, string message)
            : base(message)
        {
            ErrorCode = errorCode;
            TrooperId = trooperId;
        }

        public ClientException(ClientErrorCode errorCode, EntityId trooperId, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            TrooperId = trooperId;
        }

        public ClientErrorCode ErrorCode { get; private set; }

        public EntityId TrooperId { get; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }

    // know error
    [Serializable]
    public class ServerException : Exception
    {
        private ServerException() { }

        public ServerException(ClientErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public ServerException(ClientErrorCode errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public ServerException(ClientErrorCode errorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        public ClientErrorCode ErrorCode { get; private set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
