using Redis.Interfaces;
using StackExchange.Redis;
using IDatabase = StackExchange.Redis.IDatabase;

namespace Customer.Services;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redisCon;
    private readonly IDatabase _cache;
    private TimeSpan ExpireTime => TimeSpan.FromDays(1);

    public RedisCacheService(IConnectionMultiplexer redisCon)
    {
        _redisCon = redisCon;
        _cache = redisCon.GetDatabase();
    }

    public async Task SetCacheAsync(string key, string value, TimeSpan expiration)
    {
        var db = _redisCon.GetDatabase();
        await db.StringSetAsync(key, value, expiration);
    }

    public async Task<string> GetCacheAsync(string key)
    {
        var db = _redisCon.GetDatabase();
        return await db.StringGetAsync(key);
    }
}
