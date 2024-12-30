using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace HelseID.Standard.Tests.Mocks;

public sealed class MockCacheEntry : ICacheEntry
{
    public DateTimeOffset? AbsoluteExpiration { get; set; }
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

    public IList<IChangeToken> ExpirationTokens { get; set; } = new List<IChangeToken>();

    public object Key { get; set; } = new object();

    public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; set; } =
        new List<PostEvictionCallbackRegistration>();

    public CacheItemPriority Priority { get; set; }
    public long? Size { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
    public object? Value { get; set; }

    public void Dispose()
    {
    }
}
