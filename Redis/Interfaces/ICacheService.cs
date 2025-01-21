namespace Redis.Interfaces;

public interface ICacheService
{
    Task SetCacheAsync(string key, string value, TimeSpan expiration);
    Task<string> GetCacheAsync(string key);
}

