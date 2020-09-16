using System;
namespace Heyworks.PocketShooter.Meta.Communication
{
    public class MatchRequest
    {
        private MatchRequest(){}
        
        public MatchRequest(MatchType type, MapNames? mapName = null)
        {
            if (type == MatchType.Domination && mapName.HasValue) 
                throw new InvalidOperationException($"Cannot choose map for {type}");
            this.Match = type;
            this.MapName = mapName;
        }

        public MatchType Match { get; private set;}
        public MapNames? MapName { get; private set;}        
    }
}