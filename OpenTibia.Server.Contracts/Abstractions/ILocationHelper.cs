namespace OpenTibia.Server.Contracts.Abstractions
{
    using OpenTibia.Server.Contracts.Structs;

    public interface ILocationHelper
    {
        bool CanThrowBetween(Location fromLocation, Location toLocation, bool checkLineOfSight = true);

        bool InLineOfSight(Location fromLocation, Location toLocation);
    }
}