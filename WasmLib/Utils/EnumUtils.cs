using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace WasmLib.Utils
{
    public static class EnumUtils
    {
        private static readonly Dictionary<Type, IDictionary> CacheCollection = new Dictionary<Type, IDictionary>();
        
        public static string GetDescription<T>(T key) where T : Enum
        {
            // try get cache
            Dictionary<T, string> cache;
            if (CacheCollection.TryGetValue(typeof(T), out IDictionary? cacheNoCast)) {
                cache = (Dictionary<T, string>)cacheNoCast!;
            }
            else {
                CacheCollection[typeof(T)] = cache = new Dictionary<T, string>();
            }
            
            
            // try get string representation
            if (cache.TryGetValue(key, out string? s)) {
                return s!;
            }
            
            var name = Enum.GetName(typeof(T), key);

            if (name is null) {
                return key.ToString();
            }

            var description = typeof(T)
                .GetField(name)?
                .GetCustomAttribute<DescriptionAttribute>()?
                .Description;

            if (description is null) {
                return key.ToString();
            }

            return cache[key] = description;
        }
    }
}