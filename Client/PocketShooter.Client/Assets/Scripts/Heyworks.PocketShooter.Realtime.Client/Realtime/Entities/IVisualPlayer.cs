namespace Heyworks.PocketShooter.Realtime.Entities
{
    public interface IVisualPlayer<TEvents> : IPlayer
        where TEvents : IPlayerEvents
    {
        TEvents Events { get; }
    }
}