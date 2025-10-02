namespace Elysium.Foundation.Serpentis.Core.Abstractions;

public interface IRandom
{
    int Next(int minInclusive, int maxExclusive);
}
