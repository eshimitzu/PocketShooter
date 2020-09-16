namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// code which propagate from realtime server to client.
    /// </summary>
    public enum ClientErrorCode : byte
    {
        /// <summary>
        /// must never ever happen.
        /// </summary>
        SomethingWrongHappenedAndWeDoNotKnowWhat = 0,

        /// <summary>
        /// room was not full, but became while joining (may be should create propert protocol to prevent issues when doing match making).
        /// </summary>
        RoomIsFull,

        // wrong client version or not implemented feature
        CommandNotSupported,

        /// <summary>
        /// must never happen on production and should be fixed.
        /// </summary>
        NotAReleaseFeature,

        /// <summary>
        /// client asks to crate entity above world capacity.
        /// </summary>
        EntityLimitReached,
    }
}
