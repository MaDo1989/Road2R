using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Caching;


/// <summary>
/// Summary description for CacheService
/// </summary>
public static class CacheService
{
    private static readonly MemoryCache _cache = MemoryCache.Default;


    // Generic method to set a list in cache with an expiration time
    public static void SetList<T>(string key, List<T> list, int expirationDays = 3)
    {
        var policy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddDays(expirationDays)
        };
        _cache.Set(key, list, policy);
    }

    // Generic method to get a list from cache
    public static List<T> GetList<T>(string key)
    {
        var cached = _cache.Get(key);
        return cached as List<T>;
    }



    // Generic method to update an item in a cached list
    public static bool UpdateItemInList<T>(string key, Func<T, bool> match, T updatedItem)
    {
        var list = GetList<T>(key);
        if (list == null)
            return false;

        int index = list.FindIndex(item => match(item));
        if (index == -1)
            return false;

        list[index] = updatedItem;
        return true;
    }

    // Generic method to add an item to a cached list
    public static bool AddItemToList<T>(string key, T newItem)
    {
        var list = GetList<T>(key);
        if (list == null)
            return false;

        list.Add(newItem);
        return true;
    }

    public static void Remove(string key)
    {
        _cache.Remove(key);
    }

}