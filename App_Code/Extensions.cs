﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Extensions
/// </summary>
public static class Extensions
{
    #region IEnumerable
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
    {
        return new HashSet<T>(items);
    }
    
    
    
    #endregion
}