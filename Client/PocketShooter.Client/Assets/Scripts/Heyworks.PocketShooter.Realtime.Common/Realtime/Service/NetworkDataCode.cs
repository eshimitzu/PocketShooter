namespace Heyworks.PocketShooter.Realtime.Data
{
    /// <summary>
    /// The code of the type of network data.
    /// </summary>
    public enum NetworkDataCode : byte
    {
        // data send from server to client
        SimulationState = 1,
        ServerError = 42,
        GameJoined = 51,
        BotControlTaken = 53,
        GameEnded = 55,

        // commands send from client to server
        SimulationCommand = 2,
        SpawnTrooperCommand = 14,
        SpawnBotTrooperCommand = 15,
        JoinRoomCommand = 50,
        RequestBotCommand = 52,
        LeaveRoomCommand = 60,
    }
}