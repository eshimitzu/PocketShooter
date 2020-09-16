namespace Heyworks.PocketShooter.Realtime.Channels
{
    public interface IMessage
    {
    }

    public interface IServiceMessage: IMessage
    {
    }

    // messages to integrate meta and realtime, meta player with room
    public interface IManagementMessage: IMessage
    {}

}
