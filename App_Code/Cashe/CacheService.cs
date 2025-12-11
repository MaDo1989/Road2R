using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

public static class CacheService
{
    private static readonly object _lock = new object();

    // -------------------------
    //      BASIC CACHE
    // -------------------------

    /// <summary>
    /// Checks whether a cache entry exists.
    /// </summary>
    public static bool Exists(string key)
    {
        return HttpRuntime.Cache.Get(key) != null;
    }

    /// <summary>
    /// Gets a cache entry by key.
    /// </summary>
    public static T Get<T>(string key)
    {
        return (T)HttpRuntime.Cache.Get(key);
    }

    /// <summary>
    /// Sets a cache entry with expiration (absolute).
    /// </summary>
    public static void Set(string key, object data, int minutes = 60)
    {
        if (data == null) return;

        HttpRuntime.Cache.Insert(
            key,
            data,
            null,
            DateTime.Now.AddMinutes(minutes),
            Cache.NoSlidingExpiration
        );
    }

    /// <summary>
    /// Removes a key from cache.
    /// </summary>
    public static void Remove(string key)
    {
        HttpRuntime.Cache.Remove(key);
    }

    /// <summary>
    /// Gets a value from cache.  
    /// If it does not exist → runs factory(), inserts into cache, and returns the value.
    /// </summary>
    public static T GetOrInsert<T>(string key, Func<T> factory, int minutes = 60)
    {
        var existing = HttpRuntime.Cache.Get(key);
        if (existing != null)
            return (T)existing;

        lock (_lock)
        {
            existing = HttpRuntime.Cache.Get(key);
            if (existing != null)
                return (T)existing;

            var value = factory();
            Set(key, value, minutes);
            return value;
        }
    }


    // -------------------------
    //   LIST OPERATIONS
    // -------------------------

    /// <summary>
    /// Checks whether a specific item exists inside a cached list.
    /// </summary>
    public static bool ListExists<T>(string listKey, Func<T, bool> predicate)
    {
        var list = Get<List<T>>(listKey);
        if (list == null) return false;

        return list.Any(predicate);
    }

    /// <summary>
    /// Adds an item to a cached list only if the item does not already exist.
    /// </summary>
    public static bool ListAddIfNotExists<T>(string listKey, T item, Func<T, bool> predicate)
    {
        var list = Get<List<T>>(listKey);
        if (list == null) return false;

        lock (_lock)
        {
            if (list.Any(predicate))
                return false;

            list.Add(item);
            return true;
        }
    }

    /// <summary>
    /// Updates a specific item inside a cached list.
    /// </summary>
    public static bool ListUpdateItem<T>(string listKey, Func<T, bool> predicate, Action<T> updateAction)
    {
        var list = Get<List<T>>(listKey);
        if (list == null) return false;

        lock (_lock)
        {
            var item = list.FirstOrDefault(predicate);
            if (item == null)
                return false;

            updateAction(item);
            return true;
        }
    }

    /// <summary>
    /// Removes an item from a cached list.
    /// </summary>
    public static bool ListRemoveItem<T>(string listKey, Func<T, bool> predicate)
    {
        var list = Get<List<T>>(listKey);
        if (list == null) return false;

        lock (_lock)
        {
            var item = list.FirstOrDefault(predicate);
            if (item == null)
                return false;

            list.Remove(item);
            return true;
        }
    }

}
