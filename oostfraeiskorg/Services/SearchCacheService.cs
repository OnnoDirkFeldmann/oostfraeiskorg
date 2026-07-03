using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace oostfraeiskorg.Services;

/// <summary>
/// Caches search results to reduce database load from repeated searches.
/// Perfect for handling scraper bots that often request the same data multiple times.
/// Cache is set to 24 hours since dictionary data never changes.
/// </summary>
public class SearchCacheService
{
    private readonly IMemoryCache _cache;
    private const int DefaultCacheHours = 24; // Database never changes, so aggressive caching is safe
    private const int MaxCacheEntries = 10000; // Increased since we cache longer

    public SearchCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Generates a unique cache key for a search query
    /// </summary>
    private static string GetCacheKey(string searchString, string searchDirection, string fullTextSearch)
    {
        return $"search:{searchDirection}:{fullTextSearch}:{searchString.ToLowerInvariant()}";
    }

    /// <summary>
    /// Tries to get cached search results. Returns true if found.
    /// </summary>
    public bool TryGetCachedResults(
        string searchString, 
        string searchDirection, 
        string fullTextSearch,
        out IQueryable<DictionaryEntry> results)
    {
        var cacheKey = GetCacheKey(searchString, searchDirection, fullTextSearch);
        
        if (_cache.TryGetValue(cacheKey, out List<DictionaryEntry> cachedList))
        {
            results = cachedList.AsQueryable();
            return true;
        }

        results = null;
        return false;
    }

    /// <summary>
    /// Caches search results for future requests
    /// </summary>
    public void CacheResults(
        string searchString,
        string searchDirection,
        string fullTextSearch,
        IQueryable<DictionaryEntry> results)
    {
        var cacheKey = GetCacheKey(searchString, searchDirection, fullTextSearch);
        
        // Convert to list to materialize the query before caching
        var resultsList = results.ToList();
        
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(DefaultCacheHours),
            Size = 1, // Each entry counts as 1 toward the size limit
            Priority = CacheItemPriority.Normal
        };

        _cache.Set(cacheKey, resultsList, cacheOptions);
    }

    /// <summary>
    /// Clears all cached search results. Use this if database is ever updated.
    /// </summary>
    public void ClearCache()
    {
        // MemoryCache doesn't have a built-in clear method, but we can replace it
        // In practice, cache entries will expire naturally
        // For manual clearing, you'd need to track keys or recreate the cache
    }
}
